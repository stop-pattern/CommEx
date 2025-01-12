using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CommEx.Serial.ViewModel;
using System.Diagnostics;

namespace CommEx.Serial.Common
{
    class SaveSettings
    {
        /// <summary>
        /// 保存先のファイルパスを動的に取得
        /// このdllのファイルパス - ".dll" + ".Settings.xml"
        /// </summary>
        /// <returns>保存先のファイルパス</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static string GetSettingsFilePath()
        {
            string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string directory = Path.GetDirectoryName(assemblyLocation) ?? throw new InvalidOperationException("アセンブリのディレクトリを取得できません。");
            string fileName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.Combine(directory, $"{fileName}.Settings.xml");
        }

        /// <summary>
        /// <see cref="PortViewModel"/> のプロパティをXMLファイルに保存
        /// </summary>
        /// <param name="viewModel">保存する <see cref="PortViewModel"/> インスタンス</param>
        public static void Save(PortViewModel viewModel)
        {
            try
            {
                string filePath = GetSettingsFilePath();

                using (var writer = new StreamWriter(filePath))
                {
                    var serializer = new XmlSerializer(typeof(PortViewModel));
                    serializer.Serialize(writer, viewModel);
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"Error saving settings: {ex.Message}");
            }
        }

        /// <summary>
        /// XMLファイルから <see cref="PortViewModel"/> のプロパティを読み込み
        /// </summary>
        /// <returns>読み込まれた <see cref="PortViewModel"/> インスタンス</returns>
        public static PortViewModel Load()
        {
            try
            {
                string filePath = GetSettingsFilePath();

                if (!File.Exists(filePath)) return new PortViewModel();

                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(PortViewModel));
                    var serializableObject = (PortViewModel)serializer.Deserialize(stream);
                    return serializableObject;
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"Error loading settings: {ex.Message}");
                return new PortViewModel();
            }
        }
    }
}
