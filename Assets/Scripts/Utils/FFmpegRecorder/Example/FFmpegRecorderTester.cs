using allen.components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace allen.utils
{
    public class FFmpegRecorderTester : MonoBehaviour
    {
        public TMP_Text textProgress;
        public WebcamPlayer player;
        private FFmpegRecorder recorder;
        private List<FrameData> frameDataList => player.FrameDataList;
        private bool isRecording => player.IsRecording;
        public string dir => Path.Combine(Application.persistentDataPath, "data");
        public string filename => "ffmpegTestVideo.mp4";
        private int width = 640;
        private int height = 360;
        private void Awake()
        {
        }
        public void Start()
        {
            Init();
            SubscribeKeypress();
        }
        private void Init() => player.Init();
        private void StopRecord() => player.StopRecord();
        private void StartRecord() => player.StartRecord();
        private List<Color32[]> GetFrameList(out float fps) => player.GetFrameList(out fps);
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
        }

    }

}
