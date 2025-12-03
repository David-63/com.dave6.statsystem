using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SaveSystem
{
    public class FileManager
    {
        public static void SaveToBinaryFile(string path, Dictionary<string, object> data)
        {
            try
            {
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fileStream, data);
                }
            }
            catch (Exception)
            {
                Debug.LogError("Failed to save data to binary file at: " + path);
            }
        }

        public static void LoadFromBinaryFile(string path, out Dictionary<string, object> data)
        {
            try
            {
                if (File.Exists(path))
                {
                    using (var fileStream = new FileStream(path, FileMode.Open))
                    {
                        var binaryFormatter = new BinaryFormatter();
                        // 강제 캐스팅 해서 문제 생기면 오류 확인
                        data = (Dictionary<string, object>)binaryFormatter.Deserialize(fileStream);
                    }
                }
                else
                {
                    Debug.LogWarning("File not found at: " + path);
                    data = new Dictionary<string, object>();
                }
            }
            catch (Exception)
            {
                Debug.LogError("Failed to load data from binary file at: " + path);
                data = new Dictionary<string, object>();
            }
        }
    }
}
