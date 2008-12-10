using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;

namespace EGGEngine.Utils
{
    public class SerializeUtils<T>
    {
        private T data;
        public T Data
        {
            get { return data; }
            set { data = value; }
        }

        /// <summary>
        /// Loads the data of the specified file.
        /// </summary>
        /// <param name="storageDevice">Storage device that contains the data</param>
        /// <param name="fileName">Name of the file being loaded</param>
        public void LoadData(StorageDevice storageDevice, string fileName)
        {
            StorageContainer container = storageDevice.OpenContainer("Data");
            string filePath = Path.Combine(container.Path, fileName);
            if (File.Exists(filePath))
            {
                FileStream saveFile = File.Open(filePath, FileMode.Open);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                data = (T)xmlSerializer.Deserialize(saveFile);
                saveFile.Close();
            }
        }

        /// <summary>
        /// Saves data to the specified file.
        /// </summary>
        /// <param name="storageDevice">The storage device containing the data</param>
        /// <param name="fileName">Name of the file being saved</param>
        public void SaveData(StorageDevice storageDevice, string fileName)
        {
            StorageContainer container = storageDevice.OpenContainer("Data");
            string filePath = Path.Combine(container.Path, fileName);

            FileStream saveFile = File.Open(filePath, FileMode.Create);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            xmlSerializer.Serialize(saveFile, data);
            saveFile.Close();
        }

    }
}
