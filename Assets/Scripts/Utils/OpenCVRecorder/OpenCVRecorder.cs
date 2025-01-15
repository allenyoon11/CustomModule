using Cysharp.Threading.Tasks;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.VideoioModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

namespace neuroears.allen.utils
{
    //ADVANCED::Unity API 를 사용하지 않으므로 전과정 백그라운드동작
    //ADVANCED::병렬처리도 가능한지
    public class OpenCVRecorder : IDisposable
    {
        private VideoWriter videoWriter = null;
        private string path = null;
        private int width = 0;
        private int height = 0;
        private int fps = 0;
        //int fourcc = VideoWriter.fourcc('M', 'P', '4', 'V'); // H.264 코덱
        int fourcc = VideoWriter.fourcc('X', 'V', 'I', 'D'); // XVID 코덱
        //DEV
        public bool DevLog { get; set; } = false;
        public OpenCVRecorder(string path)
        {
            CheckOrCreateDirectory(Path.GetDirectoryName(path));
            CheckOrGetFilename(path, out string _path, out string _filename);
            this.path = _path;
        }

        public OpenCVRecorder(string path, int width, int height, int fps) : this(path)
        {
            this.width = width;
            this.height = height;
            this.fps = fps;
        }
        ~OpenCVRecorder()
        {
            Dispose();
        }
        public void Dispose()
        {
            videoWriter?.release();
        }
        public async UniTask<bool> Export(List<Color32[]> frameList, Action cb = null, Action<float> progress = null)
        {
            try
            {
                if (frameList == null) throw new Exception("frameList is null.");
                if (frameList.Count == 0) throw new Exception("frameList count is 0.");

                videoWriter = new VideoWriter();
                Size size = new Size(width, height);
               
                bool isOpen = videoWriter.open(path, fourcc, fps, size, true);
                if (!isOpen) throw new Exception($"Failed to open video file for writing: {path}");

                int temp_max_count = frameList.Count;
                int temp_cur_count = 0;

                int yieldInterval = 10; // 매 n번째 루프마다 Yield
                int loopCount = 0;
                for (int i = 0; i < frameList.Count; i++)
                {
                    Mat matFrame = ConvertColor32ToMat(frameList[i], width, height, flip: true, flipCode: 0);
                    videoWriter.write(matFrame);
                    matFrame.Dispose();

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
                if(DevLog) Debug.Log($"<color=yellow>[OpenCVRecorder] Finish export : duration: {width}x{height} | fps: {fps} | path: {path}</color>");
                return true;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.ToString());
                return false;
            }
            finally
            {
                videoWriter?.release();
            }
        }
        #region Private
        private Mat ConvertColor32ToMat(Color32[] frame, int width, int height, bool flip, int flipCode = 0)
        {
            Mat mat = new Mat(height, width, CvType.CV_8UC3); // OpenCV에서 BGR 포맷으로 생성
            byte[] data = new byte[frame.Length * 3];

            for (int i = 0; i < frame.Length; i++)
            {
                data[i * 3 + 0] = frame[i].b; // Blue 채널
                data[i * 3 + 1] = frame[i].g; // Green 채널
                data[i * 3 + 2] = frame[i].r; // Red 채널
            }

            mat.put(0, 0, data);
            if (flip)
            {
                Core.flip(mat, mat, flipCode);
            }
            return mat;
        }
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
        #endregion
    }

}
