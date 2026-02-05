using System.IO;
using Match3.Scripts.Utils;
using Newtonsoft.Json;
using UnityEngine;
namespace Match3.Scripts.Persistence
{
    public static class SaveSystem
    {
        private static string GetPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName + ".json");
        }

        public static void Save<T>(T data, string fileName, bool encrypted = false)
        {
            var json = JsonConvert.SerializeObject(data,  Formatting.Indented);

            // if (encrypted)
            //     json = EncryptionUtility.Encrypt(json);

            File.WriteAllText(GetPath(fileName), json);
        }

        public static T Load<T>(string fileName, bool encrypted = false)
        {
            string path = GetPath(fileName);

            if (!File.Exists(path))
            {
                Debug.LogWarning("File dont founded: " + path);
                return default;
            }

            string json = File.ReadAllText(path);

            // if (encrypted)
            //     json = EncryptionUtility.Decrypt(json);

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static bool Exists(string fileName)
        {
            return File.Exists(GetPath(fileName));
        }

        public static void Delete(string fileName)
        {
            var path = GetPath(fileName);
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}