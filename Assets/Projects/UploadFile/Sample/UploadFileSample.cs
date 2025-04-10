using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UniRx;
using System.IO;

public class UploadFileSample : MonoBehaviour
{
    [System.Serializable]
    public class Metadata
    {
        public string dirName;
        public string fileName;
    }

    public string uploadUrl = "http://211.254.137.218:16000/v1/uploads";
    public string fileName = "test.txt";
    private string defaultDir = "";
    private void Awake()
    {
        defaultDir = Application.persistentDataPath;
    }
    private void Start()
    {
        OpenFolder(Application.persistentDataPath);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyUp(KeyCode.A))
            .Subscribe(_ =>
            {
                var path = Path.Combine(defaultDir, fileName);
                Debug.Log($"Upload {path}");
                Metadata metadata = new Metadata
                {
                    dirName = "",
                    fileName = ""
                };
                RequestUpload(metadata, path);
            }).AddTo(this);
    }
    public void RequestUpload(Metadata metadata, string srcPath)
    {


        StartCoroutine(UploadFile(metadata, srcPath));
    }
    public void OpenFolder(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError($"path is wrong. path: {path}");
            return;
        }

        if (!System.IO.Directory.Exists(path))
        {
            Debug.LogError($"not exist: {path}");
            return;
        }
        if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            path = path.Replace("/", "\\");
        }
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = $"\"{path}\"",
            UseShellExecute = true
        };
        Process.Start(psi);
    }
    private IEnumerator UploadFile(Metadata metadata, string srcPath)
    {
        if (!System.IO.File.Exists(srcPath))
        {
            Debug.LogError($"file not exist : {srcPath}");
            yield break;
        }

        byte[] fileData = System.IO.File.ReadAllBytes(srcPath);
        byte[] jsonData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(metadata));

        // 폼 생성
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, System.IO.Path.GetFileName(srcPath));
        form.AddField("metadata", JsonUtility.ToJson(metadata));

        UnityWebRequest www = UnityWebRequest.Post(uploadUrl, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("업로드 성공: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("업로드 실패: " + www.error);
        }
    }
}
