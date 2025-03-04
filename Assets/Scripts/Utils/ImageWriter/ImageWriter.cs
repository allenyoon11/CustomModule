using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace neuroears.allen.utils
{
    public class ImageWriter
    {
        public string dir = Path.Combine(Application.persistentDataPath, "data");
        public string filenameWithoutExt;
        public string ext;
        //
        private string[] supportedExt = { "png", "jpg" };
        private bool isReadToWrite = false;
        public ImageWriter(string dir, string filenameWithoutExt, string ext)
        {
            if(!string.IsNullOrEmpty(dir))
            {
                this.dir = dir;
            }
            this.filenameWithoutExt = filenameWithoutExt;
            if (!supportedExt.Contains(ext))
            {
                Debug.LogError($"not supported {ext}");
            }
            this.ext = ext.ToLower();

            // 디렉토리 존재 여부 확인 후 생성
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            isReadToWrite = true;
        }

        public void Save(Texture texture)
        {
            if(!isReadToWrite)
            {
                Debug.LogError("not ready");
                return;
            }
            if (texture == null)
            {
                Debug.LogError("Texture is null! Cannot save.");
                return;
            }

            // RenderTexture -> Texture2D 변환
            Texture2D tex2D = ConvertToTexture2D(texture);
            if (tex2D == null)
            {
                Debug.LogError("Failed to convert Texture to Texture2D.");
                return;
            }

            // 중복되지 않는 파일명 생성
            string uniqueFilename = CheckDuplicateFilename(filenameWithoutExt, ext);
            string filePath = Path.Combine(dir, uniqueFilename);

            // 확장자에 따라 인코딩
            byte[] bytes;
            if (ext == "png")
            {
                bytes = tex2D.EncodeToPNG();
            }
            else if (ext == "jpg" || ext == "jpeg")
            {
                bytes = tex2D.EncodeToJPG();
            }
            else
            {
                Debug.LogError($"Unsupported image format: {ext}");
                return;
            }

            // 파일 저장
            File.WriteAllBytes(filePath, bytes);
            Debug.Log($"Image saved to: {filePath}");
        }

        private Texture2D ConvertToTexture2D(Texture texture)
        {
            if (texture is Texture2D tex2D)
            {
                return tex2D;
            }
            else if (texture is RenderTexture renderTex)
            {
                // RenderTexture -> Texture2D 변환
                Texture2D newTex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGB24, false);
                RenderTexture activeRT = RenderTexture.active;
                RenderTexture.active = renderTex;
                newTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
                newTex.Apply();
                RenderTexture.active = activeRT;
                return newTex;
            }
            return null;
        }

        private string CheckDuplicateFilename(string filenameWithoutExt, string ext)
        {
            string basePath = Path.Combine(dir, filenameWithoutExt);
            string filePath = basePath + "." + ext;
            int count = 1;

            // 파일이 존재하면 (1), (2), (3) ... 형식으로 파일명 변경
            while (File.Exists(filePath))
            {
                filePath = $"{basePath} ({count}).{ext}";
                count++;
            }
            return Path.GetFileName(filePath);
        }
    }

}
