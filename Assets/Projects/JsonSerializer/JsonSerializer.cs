using System;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace neuroears.allen.utils
{
    public class JsonSerializer
    {
        public static string ToJson(object obj)
        {
            return JsonUtility.ToJson(obj);
        }
        public static string ToJsonFromArray<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }
        public static T FromJson<T>(string jsonData)
        {
            return JsonUtility.FromJson<T>(jsonData);
        }
        public static T[] FromJsonToArray<T>(string jsonData)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(jsonData);
            return wrapper.Items;
        }
        public static string ToJsonWithNewton(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJsonWithNewton<T>(string jsonData)
        {
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
        // file IO
        public static void SaveJsonToFile(string path, string jsonData)
        {
            File.WriteAllText(path, jsonData, Encoding.UTF8);
        }
        public static string LoadJsonFromFile(string path)
        {
            return File.ReadAllText(path, Encoding.UTF8);
        }

        public static T LoadFromJsonFile<T>(string path)
        {
            string json = LoadJsonFromFile(path);
            return FromJson<T>(json);
        }

        public static T LoadFromJsonFileWithNewton<T>(string path)
        {
            string json = LoadJsonFromFile(path);
            return FromJsonWithNewton<T>(json);
        }
        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

}
