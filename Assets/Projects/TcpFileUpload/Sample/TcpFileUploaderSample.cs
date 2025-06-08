using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.LightTransport;

public class TcpFileUploaderSample : DevTester
{
    public string filename = "";
    public string serverIp = "211.254.137.218";
    public int port = 17001;
    public string key = "mobiledatacollection";
    private void Awake()
    {
        Debug.Log("<color=yellow>Press 'O' to open directory.</color>");

    }
    protected override void Test1()
    {
        base.Test1();
        Upload();
    }
    private async void Upload()
    {
        string path = Path.Combine(Application.persistentDataPath, filename);
        Debug.Log($"try upload file: {path}");
        await UploadFileAsync(path);
    }
    public async UniTask UploadFileAsync(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"file not found: {path}");
            return;
        }

        string fileName = Path.GetFileName(path);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key + "\n");
        byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName + "\n");
        byte[] fileData = await File.ReadAllBytesAsync(path);
        byte[] endFlag = Encoding.UTF8.GetBytes("__END__");
        try
        {
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(serverIp, port);
                using (NetworkStream stream = client.GetStream())
                {
                    stream.ReadTimeout = 10000;
                    Debug.Log("connect tcp");
                    await stream.WriteAsync(keyBytes, 0, keyBytes.Length);
                    await stream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length);
                    await stream.WriteAsync(fileData, 0, fileData.Length);
                    await stream.WriteAsync(endFlag, 0, endFlag.Length);
                    Debug.Log($"upload: {fileName} ({fileData.Length} bytes)");

                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        Debug.LogWarning("server ended");
                    }
                    else
                    {
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Debug.Log($"server response: {response}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Upload failed: {ex.Message}");
        }
    }
}