﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Rendering;
using System.Linq;

namespace allen.components
{
    public class WebcamPlayer : MonoBehaviour
    {
        //ui
        public RawImage rawImage;
        public bool devLog = false;
        //basic field
        private WebCamTexture wTex;
        private RenderTexture rTex; // 복사를 위한 재사용 목적
        private List<FrameData> store = new List<FrameData>();
        //changable field
        private int webcamIndex = 0;
        private int width = 680;
        private int height = 480;
        private int fps = 100;
        //control-record field
        private bool isRecording = false;
        private long startTime = -1;
        private long nextTime = -1;
        private long timeInterval = 10;
        private CancellationTokenSource cts;
        //for external
        public List<FrameData> frameDataList => store;
        private void Start()
        {
            cts = new CancellationTokenSource();
            RecordFrame(cts.Token).Forget();
            ConnectWebcam(webcamIndex, width, height, fps, ref wTex);
            SetSubscribe();
        }
        private void OnApplicationQuit()
        {
            cts.Cancel();
        }
        private void SetSubscribe()
        {
            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.S))
                .Subscribe(_ =>
                {
                    if (isRecording) StopRecord();
                    else StartRecord();
                }).AddTo(this);
        }

        private void StopRecord()
        {
            isRecording = false;
            if (rTex != null)
            {
                rTex.Release();
                rTex = null;
            }
            if (devLog)
            {
                int _cnt = store.Count;
                float _duration = (float)(store.Last().timestamp - store.First().timestamp) / 1000;
                float _fps = _cnt / _duration;
                Debug.Log($"[StopRecord] frame count : {_cnt} | duration : {_duration} | fps: {_fps}");
            }
        }

        private void StartRecord()
        {
            startTime = -1;
            nextTime = -1;
            timeInterval = 1000 / fps;
            store.Clear();
            isRecording = true;
            if (devLog) Debug.Log("[StartRecord]");
        }

        private async UniTaskVoid RecordFrame(CancellationToken token)
        {

            while (true)
            {
                if(token.IsCancellationRequested) return;
                await UniTask.WaitUntil(() => isRecording);
                var curTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                if (startTime < 0)
                {
                    startTime = curTime;
                    nextTime = curTime;
                }
                if(curTime >= nextTime)
                {
                    nextTime += timeInterval;
                    AddFrame(curTime);
                }
            }
        }

        private async void AddFrame(long timestamp)
        {
            try
            {
                InitRenderTexture(wTex.width, wTex.height, ref rTex);
                Graphics.Blit(wTex, rTex);
                var req = await AsyncGPUReadback.Request(rTex, 0);
                if (req.hasError)
                {
                    Debug.LogError("AsyncGPUReadback request failed.");
                    return;
                }
                var buffer = req.GetData<Color32>();
                Color32[] frame = buffer.ToArray();
                FrameData frameData = new FrameData()
                {
                    timestamp = timestamp,
                    frame = frame
                };
                store.Add(frameData);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }
        private void InitRenderTexture(int width, int height, ref RenderTexture rTex)
        {
            if(rTex == null || rTex.width != width || rTex.height != height)
            {
                if (rTex != null) rTex.Release();
                rTex = new RenderTexture(width, height, 0);
            }
        }
        private void ConnectWebcam(int wIdx, int width, int height, int fps, ref WebCamTexture wTex)
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            if (devices.Length == 0)
            {
                Debug.LogError("No webcam devices found!");
                return;
            }

            if (wIdx >= devices.Length)
            {
                Debug.LogError($"Invalid webcam index: {wIdx}. Max available: {devices.Length - 1}");
                return;
            }
            string selectedDeviceName = devices[wIdx].name;
            wTex = new WebCamTexture(selectedDeviceName, width, height, fps);

            if (wTex == null)
            {
                Debug.LogError($"Failed to initialize webcam: {selectedDeviceName}");
                return;
            }

            wTex.Play();
            if (devLog) Debug.Log($"Connected {selectedDeviceName} | size: {wTex.width}x{wTex.height} | fps: {wTex.requestedFPS}");
            rawImage.texture = wTex;
        }

    }
    public class FrameData
    {
        public long timestamp;
        public Color32[] frame;
    }

}