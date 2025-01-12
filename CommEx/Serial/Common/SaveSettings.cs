using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CommEx.Serial.ViewModel;

namespace CommEx.Serial.Common
{
    class SaveSettings
    {
        private const string FilePath = "PortViewModelSettings.xml";

        /// <summary>
        /// PortViewModelのプロパティをXMLファイルに保存します。
        /// </summary>
        /// <param name="viewModel">保存するPortViewModelインスタンス</param>
        public static void Save(PortViewModel viewModel)
        {
            try
            {
                var serializableObject = new SerializablePortViewModel(viewModel);
                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    var serializer = new XmlSerializer(typeof(SerializablePortViewModel));
                    serializer.Serialize(stream, serializableObject);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        /// <summary>
        /// XMLファイルからPortViewModelのプロパティを読み込みます。
        /// </summary>
        /// <returns>読み込まれたPortViewModelインスタンス</returns>
        public static PortViewModel Load()
        {
            try
            {
                if (!File.Exists(FilePath)) return new PortViewModel();

                using (var stream = new FileStream(FilePath, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(SerializablePortViewModel));
                    var serializableObject = (SerializablePortViewModel)serializer.Deserialize(stream);
                    return serializableObject.ToPortViewModel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
                return new PortViewModel();
            }
        }
    }

    /// <summary>
    /// <see cref="PortViewModel"/> を保存するためのラップクラス
    /// </summary>
    [Serializable]
    public class SerializablePortViewModel
    {
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public bool DtrEnable { get; set; }
        public string EncodingName { get; set; }
        public Handshake Handshake { get; set; }
        public string NewLine { get; set; }
        public Parity Parity { get; set; }
        public string PortName { get; set; }
        public StopBits StopBits { get; set; }
        public bool IsAutoConnent { get; set; }

        // Parameterless constructor for XmlSerializer
        public SerializablePortViewModel() { }

        public SerializablePortViewModel(PortViewModel viewModel)
        {
            BaudRate = viewModel.BaudRate;
            DataBits = viewModel.DataBits;
            DtrEnable = viewModel.DtrEnable;
            EncodingName = viewModel.Encoding.WebName;
            Handshake = viewModel.Handshake;
            NewLine = viewModel.NewLine;
            Parity = viewModel.Parity;
            PortName = viewModel.PortName;
            StopBits = viewModel.StopBits;
            IsAutoConnent = viewModel.IsAutoConnent;
        }

        public PortViewModel ToPortViewModel()
        {
            return new PortViewModel
            {
                BaudRate = BaudRate,
                DataBits = DataBits,
                DtrEnable = DtrEnable,
                Encoding = Encoding.GetEncoding(EncodingName),
                Handshake = Handshake,
                NewLine = NewLine,
                Parity = Parity,
                PortName = PortName,
                StopBits = StopBits,
                IsAutoConnent = IsAutoConnent
            };
        }
    }
}
