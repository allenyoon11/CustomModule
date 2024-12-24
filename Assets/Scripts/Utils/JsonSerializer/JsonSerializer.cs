using System;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

public class JsonSerializer
{
    public static string SerializeJsonUtility(object obj)
    {
        return JsonUtility.ToJson(obj);
    }
    public static string SerializeJsonUtilityArr<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }
    public static T DeserializeJsonUtility<T>(string jsonData)
    {
        return JsonUtility.FromJson<T>(jsonData);
    }
    public static T[] DeserializeJsonUtilityArr<T>(string jsonData)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(jsonData);
        return wrapper.Items;
    }
    public static string SerializeNewton(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public static T DeserializeNewton<T>(string jsonData)
    {
        return JsonConvert.DeserializeObject<T>(jsonData);
    }

    public static void CreateJsonFile(string createPath, string fileName, string jsonData)
    {
        FileStream stream = new FileStream(string.Format("{0}/{1}", createPath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        stream.Write(data, 0, data.Length);
        stream.Close();
    }
    public static void CreateJsonFile(string path, string jsonData)
    {
        FileStream stream = new FileStream(path, FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        stream.Write(data, 0, data.Length);
        stream.Close();
    }
    public static T LoadJsonFile<T>(string loadPath, string fileName)
    {
        FileStream stream = new FileStream(string.Format("{0}/{1}", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);
        stream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<T>(jsonData);
    }
    public static T LoadJsonFile<T>(string path)
    {
        FileStream stream = new FileStream(path, FileMode.Open);
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);
        stream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<T>(jsonData);
    }
    public static T LoadJsonFileNewton<T>(string loadPath, string fileName)
    {
        FileInfo jsonFileInfo = new FileInfo(string.Format("{0}/{1}", loadPath, fileName));
        if (!jsonFileInfo.Exists)
        {
            Debug.Log("Not found " + fileName);
        }
        FileStream stream = new FileStream(string.Format("{0}/{1}", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);
        stream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<T>(jsonData);
    }
    public static T LoadJsonFileNewton<T>(string path)
    {
        FileInfo jsonFileInfo = new FileInfo(path);
        if (!jsonFileInfo.Exists)
        {
            Debug.Log("Not found file");
        }
        FileStream stream = new FileStream(path, FileMode.Open);
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);
        stream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<T>(jsonData);
    }
    public static string ReadJsonFile(string loadPath, string fileName)
    {
        FileStream stream = new FileStream(string.Format("{0}/{1}", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);
        stream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return jsonData;
    }
    public static string ReadJsonFile(string path)
    {
        FileStream stream = new FileStream(path, FileMode.Open);
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);
        stream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return jsonData;
    }
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
