using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CommEx.Serial.ViewModels;
using System.Diagnostics;

namespace CommEx.Serial.Common
{
    class SaveSettings
    {
        #region Static Methods

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
        /// <see cref="ListViewModel"/> のプロパティをXMLファイルに保存
        /// </summary>
        /// <param name="viewModel">保存する <see cref="ListViewModel"/> インスタンス</param>
        public static void Save(ListViewModel viewModel)
        {
            try
            {
                string filePath = GetSettingsFilePath();

                using (var writer = new StreamWriter(filePath))
                {
                    var serializer = new XmlSerializer(typeof(ListViewModel));
                    serializer.Serialize(writer, viewModel);
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"Error saving settings: {ex.Message}");
            }
        }

        /// <summary>
        /// XMLファイルから <see cref="ListViewModel"/> のプロパティを読み込み
        /// </summary>
        /// <returns>読み込まれた <see cref="ListViewModel"/> インスタンス</returns>
        public static ListViewModel Load()
        {
            try
            {
                string filePath = GetSettingsFilePath();

                if (!File.Exists(filePath)) return new ListViewModel();

                using (var reader = new StreamReader(filePath))
                {
                    var serializer = new XmlSerializer(typeof(ListViewModel));
                    var serializableObject = (ListViewModel)serializer.Deserialize(reader);
                    return serializableObject;
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"Error loading settings: {ex.Message}");
                return new ListViewModel();
            }
        }

        #endregion
    }
}
