using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using neuroears.allen.utils.webcam;

namespace neuroears.allen.utils
{
    public class FFmpegRecorderTester : MonoBehaviour
    {
        public TMP_Text textTime;
        public TMP_Text textProgress;
        public WebcamPlayer player;
        private FFmpegRecorder recorder;
        private List<FrameData<Color32[]>> frameDataList => player.GetFrameData();
        private bool isRecording => player.IsRecording;
        public string dir => Path.Combine(Application.persistentDataPath, "data");
        public string filename => "ffmpegTestVideo.mp4";
        //
        private int idx = 0;
        private int width = 1280;
        private int height = 800;
        private int fps = 100;
        //
        private float time = 0f;
        private void Awake()
        {
        }
        public void Start()
        {
            Init();
            SubscribeKeypress();
        }
        private void Update()
        {
            if (isRecording)
            {
                time += Time.deltaTime;
                SetTimeText(time);
            }
        }
        private void SetTimeText(float time)
        {
            textTime.text = time.ToString("F2");
        }
        private void Init() => player.InitWebcam(idx, width, height, fps);
        private void StartRecord()
        {
            time = 0f;
            player.StartRecord();
        }
        private void StopRecord() => player.StopRecord();
        private List<Color32[]> GetFrameList(out float fps) => player.GetRawData(out fps);
        private async void Export()
        {
            string path = Path.Combine(dir, filename);
            List<Color32[]> frameList = GetFrameList(out float fps);
            recorder = new FFmpegRecorder(path, width, height, (int)fps);
            recorder.DevLog = true;

            await recorder.Export(frameList, null, (progress) =>
            {
                textProgress.text = $"{progress.ToString("F2")}%";
            });

        }
        private void OpenDir()
        {
            string path = Path.Combine(Application.persistentDataPath, "data");
            System.Diagnostics.Process process = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = path.Replace("/", "\\"),
                    UseShellExecute = true,
                }
            };
            process.Start();
        }
        private void SubscribeKeypress()
        {
            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.S))
                .Subscribe(_ =>
                {
                    if (isRecording) StopRecord();
                    else StartRecord();
                }).AddTo(this);
            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.A))
                .Subscribe(_ =>
                {
                    Export();
                }).AddTo(this);
            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.O))
                .Subscribe(_ =>
                {
                    OpenDir();
                }).AddTo(this);
        }

    }

}
