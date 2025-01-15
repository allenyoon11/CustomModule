using neuroears.allen.utils.webcam;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace neuroears.allen.utils.webcam
{
    public class MultiWebcamPlayer : MonoBehaviour
    {
        public WebcamPlayer[] players;
        private WebcamData[] webcamData;
        private bool isRecording = false;
        public bool IsRecording => isRecording;
        public bool IsRecordingPlayers => players.All(el => el.IsRecording);
        private void Awake()
        {
            webcamData = new WebcamData[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                players[i].InitWebcam(i);
            }
        }
        private void OnEnable()
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].LoadWebcam();
            }
        }
        private void Start()
        {
            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ =>
                {
                    if (isRecording) StopRecord();
                    else StartRecord();
                }).AddTo(this);
        }
        public void StartRecord()
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].StartRecord();
            }
            isRecording = true;
        }
        public void StopRecord()
        {

            for (int i = 0; i < players.Length; i++)
            {
                players[i].StopRecord();
                webcamData[i] = players[i].GetWebcamData();
            }
            isRecording = false;
            TestLog();
        }

        private void TestLog()
        {
            if (webcamData.Length > 0)
            {
                for (int i = 0; i < webcamData.Length; i++)
                {
                    Debug.Log($"webcam_{i} {webcamData[i].frameDataList.Count}");
                }
            }
            
        }
    }

}
