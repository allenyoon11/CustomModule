using allen.utils;
using Cysharp.Threading.Tasks;
using FFmpegOut;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Presets;
using UnityEngine;

namespace allen.utils
{
    /// <summary>
    /// v1.0.02
    /// </summary>
    public class FFmpegRecorder : IDisposable
    {
        private FFmpegSession session;
        private string path = null;
        private int width = 0;
        private int height = 0;
        private int fps = 0;
        private FFmpegPreset preset = FFmpegPreset.H264Default_Custom;
        //DEV
        public bool DevLog { get; set; } = false;

        public FFmpegRecorder(string path)
        {
            if (!CheckSrcFile()) throw new Exception("Not found ffmpeg.exe in StreamingAssets");
            if (Path.GetExtension(path) != FFmpegPresetExtensions.GetSuffix(preset)) Debug.LogError($"Wrong extension and forced change");
            CheckOrCreateDirectory(Path.GetDirectoryName(path));
            CheckOrGetFilename(path, out string _path, out string _filename);
            this.path = _path;
        }
        public FFmpegRecorder(string path, int width, int height, int fps) : this(path)
        {
            this.width = width;
            this.height = height;
            this.fps = fps;
        }
        ~FFmpegRecorder()
        {
            Dispose();
        }
        public void Dispose()
        {
            session?.Dispose();
        }
        public async UniTask<bool> Export(List<Color32[]> frameList, Action cb = null, Action<float> progress = null)
        {
            try
            {

                if (frameList == null) throw new Exception("frameList is null.");
                if (frameList.Count == 0) throw new Exception("frameList count is 0.");

                session = FFmpegSession.CreateWithOutputPath(path, width, height, fps, preset);
                int temp_max_count = frameList.Count;
                int temp_cur_count = 0;

                Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
                int yieldInterval = 2; // 매 n번째 루프마다 Yield
                int loopCount = 0;

                for (int i = 0; i < frameList.Count; i++)
                {
                    //Debug.Log($"{width}, {height}, {frame.Length}");
                    tex.SetPixels32(frameList[i]);
                    tex.Apply();
                    session.PushFrameDirect(tex);

                    temp_cur_count++;
                    loopCount++;
                    float percentage = (float)temp_cur_count / temp_max_count * 100;
                    if (DevLog) Debug.Log($"export progress: {temp_cur_count}/{temp_max_count} | {percentage.ToString("F1")}");

                    if (progress != null) progress(percentage);
                    if (cb != null) cb();

                    if (loopCount % yieldInterval == 0)
                    {
                        await UniTask.Yield();
                    }
                }
                session.CompletePushFrames();
                session.Close();
                if (DevLog) Debug.Log($"<color=yellow>[FFmpegRecorder] Finish export : duration: {width}x{height} | fps: {fps} | path: {path}</color>");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                return false;
            }
            finally
            {
                session?.Dispose();
            }
        }
        #region Private
        private bool CheckOrCreateDirectory(string dir)
        {
            try
            {
                if (Directory.Exists(dir))
                {
                    if (DevLog) Debug.Log($"Directory exist already dir: {dir}");
                    return true;
                }
                else
                {
                    Directory.CreateDirectory(dir);
                    if (DevLog) Debug.Log($"new Directory created dir: {dir}");
                    return true;
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                return false;
            }
        }
        private bool CheckOrGetFilename(string path, out string _path, out string _filename)
        {
            string dir = Path.GetDirectoryName(path);
            string filenameWithoutExt = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);

            _path = path;
            _filename = filenameWithoutExt;
            try
            {
                if (File.Exists(path))
                {
                    int i = 1;
                    while (true)
                    {
                        _filename = string.Format("{0} ({1})", filenameWithoutExt, i++);
                        _path = Path.Combine(dir, $"{_filename}{ext}");

                        if (!File.Exists(_path)) break;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                return false;
            }
        }
        private bool CheckSrcFile()
        {
            string srcPath = Path.Combine(Application.streamingAssetsPath, "FFmpegOut/Windows/ffmpeg.exe");
            return File.Exists(srcPath);
        }
        #endregion
    }

}
