using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CommEx.Testing
{
    /// <summary>Checks file mappings in a Windows process without modifying or suspending it.</summary>
    public static class ProcessFileMappings
    {
        /// <summary>Requests process metadata and read-only virtual-memory inspection.</summary>
        private const uint InspectionAccess = 0x0400 | 0x0010;

        /// <summary>Identifies committed virtual memory.</summary>
        private const uint MemoryCommitted = 0x1000;

        /// <summary>Identifies mappings backed by an executable image.</summary>
        private const uint MemoryImage = 0x1000000;

        /// <summary>Identifies mappings backed by a file section.</summary>
        private const uint MemoryMapped = 0x40000;

        /// <summary>Identifies sections for which Windows reports no valid backing file.</summary>
        private const int ErrorFileInvalid = 1006;

        /// <summary>Caps the number of queried virtual-memory regions.</summary>
        private const int MaximumRegions = 100000;

        /// <summary>Bounds inspection duration between native calls.</summary>
        private const int MaximumMilliseconds = 10000;

        /// <summary>Provides a bounded buffer for Windows device and mapped-file names.</summary>
        private const int PathCapacity = 32768;

        /// <summary>Determines whether a process has a committed mapping of the specified file.</summary>
        /// <param name="processId">The positive identifier of the process to inspect.</param>
        /// <param name="expectedPath">An absolute local drive path to the expected mapped file.</param>
        /// <returns>True when an exact device-path match is observed; false after a complete scan.</returns>
        /// <exception cref="ArgumentException">The file path is not an absolute local drive path.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The process identifier is not positive.</exception>
        /// <exception cref="PlatformNotSupportedException">The inspector is not a 64-bit process.</exception>
        /// <exception cref="Win32Exception">Windows denies or fails an inspection operation, except an invalid backing file for a non-image mapped section.</exception>
        /// <exception cref="InvalidOperationException">A native result prevents a complete bounded scan.</exception>
        /// <exception cref="TimeoutException">The inspection reaches its time or region limit.</exception>
        public static bool Contains(int processId, string expectedPath)
        {
            if (processId <= 0)
            {
                throw new ArgumentOutOfRangeException("processId");
            }

            if (IntPtr.Size != 8)
            {
                throw new PlatformNotSupportedException("File-map inspection requires a 64-bit inspector.");
            }

            string devicePath = GetDevicePath(expectedPath);
            IntPtr process = OpenProcess(InspectionAccess, false, processId);
            if (process == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Cannot open the process for read-only inspection.");
            }

            try
            {
                SystemInformation system;
                GetNativeSystemInfo(out system);
                ulong address = (ulong)system.MinimumApplicationAddress.ToInt64();
                ulong maximum = (ulong)system.MaximumApplicationAddress.ToInt64();
                UIntPtr structureSize = new UIntPtr((uint)Marshal.SizeOf(typeof(MemoryInformation)));
                Stopwatch elapsed = Stopwatch.StartNew();
                int regions = 0;
                StringBuilder fileName = new StringBuilder(PathCapacity);

                while (address <= maximum)
                {
                    if (++regions > MaximumRegions || elapsed.ElapsedMilliseconds >= MaximumMilliseconds)
                    {
                        throw new TimeoutException("Process file-map inspection exceeded its bounded scan budget.");
                    }

                    MemoryInformation region;
                    UIntPtr queried = VirtualQueryEx(process, new IntPtr((long)address), out region, structureSize);
                    if (queried == UIntPtr.Zero)
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "Cannot inspect a process memory region.");
                    }

                    if (queried.ToUInt64() != structureSize.ToUInt64())
                    {
                        throw new InvalidOperationException("Windows returned incomplete memory-region metadata.");
                    }

                    if (region.State == MemoryCommitted && (region.Type == MemoryImage || region.Type == MemoryMapped))
                    {
                        fileName.Clear();
                        uint length = GetMappedFileName(process, region.BaseAddress, fileName, PathCapacity);
                        if (length == 0)
                        {
                            int error = Marshal.GetLastWin32Error();
                            // Anonymous pagefile-backed sections have no file to compare. Image failures remain fatal.
                            if (region.Type != MemoryMapped || error != ErrorFileInvalid)
                            {
                                throw new Win32Exception(error, "Cannot inspect a mapped file name.");
                            }
                        }

                        if (length >= PathCapacity - 1)
                        {
                            throw new InvalidOperationException("A mapped file name exceeded the inspection buffer.");
                        }

                        if (length > 0 && string.Equals(fileName.ToString(), devicePath, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }

                    ulong regionBase = (ulong)region.BaseAddress.ToInt64();
                    ulong size = region.RegionSize.ToUInt64();
                    if (size == 0 || regionBase > ulong.MaxValue - size || regionBase + size <= address)
                    {
                        throw new InvalidOperationException("Windows returned a non-advancing memory region.");
                    }

                    address = regionBase + size;
                }

                return false;
            }
            finally
            {
                CloseHandle(process);
            }
        }

        /// <summary>Converts an absolute local drive path to the native device name used for mappings.</summary>
        /// <param name="path">The local drive path to normalize.</param>
        /// <returns>The device prefix followed by the normalized path within that drive.</returns>
        /// <exception cref="ArgumentException">The path is not a fully qualified local drive path.</exception>
        /// <exception cref="Win32Exception">The drive device cannot be resolved.</exception>
        private static string GetDevicePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path.Length < 3 ||
                !char.IsLetter(path[0]) || path[1] != ':' || (path[2] != '\\' && path[2] != '/'))
            {
                throw new ArgumentException("An absolute local drive path is required.", "path");
            }

            string normalized = Path.GetFullPath(path).Replace('/', '\\');
            StringBuilder device = new StringBuilder(PathCapacity);
            if (QueryDosDevice(normalized.Substring(0, 2), device, PathCapacity) == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Cannot resolve the expected file's drive device.");
            }

            return device.ToString() + normalized.Substring(2);
        }

        /// <summary>Represents the native 64-bit MEMORY_BASIC_INFORMATION layout.</summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct MemoryInformation
        {
            /// <summary>Gets the start of this memory region.</summary>
            public IntPtr BaseAddress;
            /// <summary>Gets the start of the original allocation.</summary>
            public IntPtr AllocationBase;
            /// <summary>Gets the original allocation protection flags.</summary>
            public uint AllocationProtect;
            /// <summary>Gets the size of this contiguous region in bytes.</summary>
            public UIntPtr RegionSize;
            /// <summary>Gets the allocation state of the region.</summary>
            public uint State;
            /// <summary>Gets the current page protection flags.</summary>
            public uint Protect;
            /// <summary>Gets the region type, including image or file mapping.</summary>
            public uint Type;
        }

        /// <summary>Represents SYSTEM_INFO with native pointer alignment.</summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct SystemInformation
        {
            /// <summary>Contains the processor architecture and reserved union storage.</summary>
            public uint ProcessorArchitectureAndReserved;
            /// <summary>Contains the system page size.</summary>
            public uint PageSize;
            /// <summary>Contains the first accessible application address.</summary>
            public IntPtr MinimumApplicationAddress;
            /// <summary>Contains the last accessible application address.</summary>
            public IntPtr MaximumApplicationAddress;
            /// <summary>Contains the processor affinity mask.</summary>
            public UIntPtr ActiveProcessorMask;
            /// <summary>Contains the logical processor count.</summary>
            public uint NumberOfProcessors;
            /// <summary>Contains the processor type.</summary>
            public uint ProcessorType;
            /// <summary>Contains the virtual-allocation granularity.</summary>
            public uint AllocationGranularity;
            /// <summary>Contains the processor architecture level.</summary>
            public ushort ProcessorLevel;
            /// <summary>Contains the processor revision.</summary>
            public ushort ProcessorRevision;
        }

        /// <summary>Opens a process with the requested inspection rights.</summary>
        /// <param name="access">The requested access mask.</param>
        /// <param name="inheritHandle">Whether child processes may inherit the handle.</param>
        /// <param name="processId">The target process identifier.</param>
        /// <returns>A process handle or zero on failure.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint access, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle, int processId);

        /// <summary>Releases an acquired native handle.</summary>
        /// <param name="handle">The handle to release.</param>
        /// <returns>Whether Windows closed the handle successfully.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        /// <summary>Obtains native system address limits and layout information.</summary>
        /// <param name="information">Receives the native system information.</param>
        [DllImport("kernel32.dll")]
        private static extern void GetNativeSystemInfo(out SystemInformation information);

        /// <summary>Reads metadata for a process memory region without reading or writing its contents.</summary>
        /// <param name="process">The process inspection handle.</param>
        /// <param name="address">An address within the region.</param>
        /// <param name="information">Receives the region metadata.</param>
        /// <param name="length">The metadata structure size.</param>
        /// <returns>The number of metadata bytes returned, or zero on failure.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern UIntPtr VirtualQueryEx(IntPtr process, IntPtr address, out MemoryInformation information, UIntPtr length);

        /// <summary>Reads the native file name backing a mapped region.</summary>
        /// <param name="process">The process inspection handle.</param>
        /// <param name="address">An address within the mapped region.</param>
        /// <param name="fileName">Receives the mapped file name.</param>
        /// <param name="capacity">The output buffer capacity in characters.</param>
        /// <returns>The copied character count, or zero on failure.</returns>
        [DllImport("psapi.dll", EntryPoint = "GetMappedFileNameW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint GetMappedFileName(IntPtr process, IntPtr address, StringBuilder fileName, int capacity);

        /// <summary>Resolves a DOS drive name to its native device target.</summary>
        /// <param name="deviceName">The drive name including its colon.</param>
        /// <param name="targetPath">Receives the device target.</param>
        /// <param name="capacity">The output buffer capacity in characters.</param>
        /// <returns>The returned character count, or zero on failure.</returns>
        [DllImport("kernel32.dll", EntryPoint = "QueryDosDeviceW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint QueryDosDevice(string deviceName, StringBuilder targetPath, int capacity);
    }
}
