using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace neuroears.allen.utils
{
    public class FileIOHelper
    {
        public static void CheckOrCreateDirectory(string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                throw;
            }
        }
        public static void CheckOrCreateFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    File.Create(path);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                throw;
            }
        }
        public static string GetUniquePathWithNumber(string path, bool deleteExistingFile = false)
        {
            var dir = Path.GetDirectoryName(path);
            var filename = Path.GetFileNameWithoutExtension(path);
            var ext = Path.GetExtension(path);
            return GetUniquePathWithNumber(dir, filename, ext, deleteExistingFile);
        }
        public static string GetUniquePathWithNumber(string dir, string filename, string ext, bool deleteExistingFile = false)
        {
            ext = ext.StartsWith(".") ? ext.Substring(1) : ext;
            //Logger.b($"{dir} | {filename} | {ext}");
            var path = Path.Combine(dir, $"{filename}.{ext}");
            if (deleteExistingFile && File.Exists(path))
            {
                File.Delete(path);
                return path;
            }
            if (File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    var _filename = string.Format("{0} ({1})", filename, i++);
                    path = Path.Combine(dir, $"{_filename}.{ext}");

                    if (!File.Exists(path))
                    {
                        break;
                    }
                }
            }
            return path;
        }
        public static async UniTask WriteBinaryAsync<T>(string dir, string filename, T data)
        {
            try
            {
                CheckOrCreateDirectory(dir);
                await UniTask.RunOnThreadPool(async () =>
                {
                    using (FileStream fs = new FileStream(Path.Combine(dir, filename), FileMode.Create))
                    {
                        string json = JsonUtility.ToJson(data);
                        byte[] binaryData = Encoding.UTF8.GetBytes(json);
                        await fs.WriteAsync(binaryData);
                    }
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                throw;
            }
        }
        public static async UniTask<T> ReadBinaryAsync<T>(string dir, string filename)
        {
            try
            {
                using (FileStream fs = new FileStream(Path.Combine(dir, filename), FileMode.Open))
                {
                    byte[] binaryData = new byte[fs.Length];
                    await fs.ReadAsync(binaryData);
                    string json = Encoding.UTF8.GetString(binaryData);
                    T data = JsonUtility.FromJson<T>(json);
                    return data;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                throw;
            }
        }
        public static void GetImage(string path)
        {
            byte[] byteTexture = File.ReadAllBytes(path);
            if (byteTexture.Length <= 0) return;
            Texture2D texture2D = new Texture2D(0, 0);
        }
        public static void Save(string path, byte[] bytes)
        {
            Debug.Log($"<color=yellow>Success export image\n{path}</color>");
            File.WriteAllBytes(path, bytes);
        }
        public static async UniTask SaveAsync(string path, byte[] bytes)
        {
            Debug.Log($"<color=yellow>Success export image\n{path}</color>");
            await File.WriteAllBytesAsync(path, bytes);
        }

        public static async UniTask SaveFileAsync(string path, byte[] bytes)
        {
            FileStream stream = new FileStream(path, FileMode.Create);
            try
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
                stream.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                throw;
            }
            finally
            {
                stream.Close();
            }
        }
        public static async UniTask<byte[]> LoadFileAsync(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            try
            {
                byte[] data = new byte[stream.Length];
                await stream.ReadAsync(data, 0, data.Length);
                stream.Close();
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                throw;
            }
            finally
            {
                stream.Close();
            }
        }
        public static void DeleteOldDirectories(string folderPath, int daysThreshold, string? nameFilter)
        {
            if (!Directory.Exists(folderPath))
            {
                Debug.LogError($"not exist path: {folderPath}");
                return;
            }

            var directories = Directory.GetDirectories(folderPath);
            DateTime now = DateTime.Now;

            foreach (var dir in directories)
            {
                string dirName = Path.GetFileName(dir);
                if (!string.IsNullOrEmpty(nameFilter) && !dirName.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                DateTime lastWrite = Directory.GetLastWriteTime(dir);
                if ((now - lastWrite).TotalDays > daysThreshold)
                {
                    try
                    {
                        Directory.Delete(dir, recursive: true);
                        Debug.Log($"success delete: {dir}");
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"fail delete: {dir} - {ex.Message}");
                    }
                }
            }
        }
        public static void DeleteOldFiles(string folderPath, int daysThreshold, string? nameFilter)
        {
            if (!Directory.Exists(folderPath))
            {
                Debug.LogError($"not exist path: {folderPath}");
                return;
            }

            var files = Directory.GetFiles(folderPath);
            DateTime now = DateTime.Now;

            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);

                if (!string.IsNullOrEmpty(nameFilter) &&
                    !fileName.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                DateTime lastWrite = File.GetLastWriteTime(file);
                if ((now - lastWrite).TotalDays > daysThreshold)
                {
                    try
                    {
                        File.Delete(file);
                        Debug.Log($"success delete: {file}");
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"fail delete: {file} - {ex.Message}");
                    }
                }
            }
        }

        public static bool HasWritePermission(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                    return false;

                string testFilePath = Path.Combine(directoryPath, Path.GetRandomFileName());

                // 파일 생성 시도
                using (FileStream fs = File.Create(testFilePath, 1, FileOptions.DeleteOnClose))
                {
                    // 생성 성공 → 권한 있음
                }

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (Exception)
            {
                // 다른 예외 (디스크 문제, 경로 오류 등)
                return false;
            }
        }
    }
}

