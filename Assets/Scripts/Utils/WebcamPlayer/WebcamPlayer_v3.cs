using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;
namespace neuroears.allen.utils.webcam
{
    #region Summary
    /// <summary>
    /// v1.0.00::run thread using cpu
    /// </summary>
    #endregion
    public class WebcamPlayer_v3 : WebcamPlayer
    {
        public int recordFps = 100;
        //
        protected IWebcamDataStore store;
        protected bool isRecording = false;

        private long startUnixTimestamp = 0; //mircosecond
        private long nextFrameTime = 0;
        protected Stopwatch stopwatch;
        private CancellationTokenSource cts = null;
        //
        private Subject<(long timestamp, byte[] frame)> OnTextureUpdated = new Subject<(long timestamp, byte[] frame)>();

        //External
        public bool IsRecording => isRecording;
        protected override void Awake()
        {
            if (processItSelf)
            {
                base.Awake();
            }
        }
        protected override void OnEnable()
        {
            if (processItSelf)
            {
                base.LoadWebcam();
                InitRecorder();
            }
        }
        protected override void Start()
        {
            if (processItSelf)
            {
                SubscribeUpdateTexture();
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                stopwatch.Stop();
            }
        }
        public async void InitRecorder()
        {
            await UniTask.WaitUntil(() => IsLoaded);
            if (store == null)
            {
                this.store = new WebcamDataStore(WebcamIdx, DeviceName, Width, Height, Fps);
            }
            else
            {
                this.store.SetInit(WebcamIdx, DeviceName, Width, Height, Fps);
            }
        }

        public void StartRecord()
        {
            if (showLog) Debug.Log($"[WebcamPlayer-{WebcamIdx}] Start Record");
            store.ReadyToRecord();
            cts = new CancellationTokenSource();
            CaptureFramesAsync(cts.Token).Forget();

            startUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000;
            nextFrameTime = 0;
            stopwatch.Start();
            isRecording = true;
        }
        public void StopRecord()
        {
            if (cts.Token != null) cts.Cancel();
            isRecording = false;
            stopwatch.Stop();
            store.StopRecord(out long startTimestamp, out long endTimestamp, out long duration, out int frameCount, out float fps);
            if (showLog) Debug.Log($"[WebcamPlayer-{WebcamIdx}] Stop Record | duration: {duration} | frameCount: {frameCount} | fps: {fps}");
        }
        public WebcamData GetWebcamData() => store.GetWebcamData();
        public List<FrameData<Color32[]>> GetFrameData() => store.GetFrameData();
        public List<Color32[]> GetRawData(out float fps) => store.GetRawData(out fps);
        private async UniTaskVoid CaptureFramesAsync(CancellationToken token)
        {
            long frameInterval = 1000000 / fps; //microsecond

            while (!token.IsCancellationRequested)
            {
                if (!isRecording)
                {
                    await UniTask.Yield();
                    continue;
                }

                long currentTime = stopwatch.ElapsedTicks * 1_000_000L / Stopwatch.Frequency;
                if (nextFrameTime == 0) nextFrameTime = currentTime;
                if (currentTime < nextFrameTime)
                {
                    await UniTask.Delay((int)(nextFrameTime - currentTime), cancellationToken: token);
                    continue;
                }

                // 프레임 동기화 (프레임 드롭 방지)
                nextFrameTime += frameInterval;
                //long drift = stopwatch.ElapsedMilliseconds - nextFrameTime;
                //if (drift > frameInterval) // 프레임 드롭 감지
                //{
                //    nextFrameTime = stopwatch.ElapsedMilliseconds; // 즉시 동기화
                //}

                // 메인 스레드에서 WebCamTexture 데이터 가져오기
                await UniTask.SwitchToMainThread();
                var pixels = wTex.GetPixels32();

                // 백그라운드 스레드에서 변환 및 전송
                await UniTask.RunOnThreadPool(() =>
                {
                    byte[] frameData = ConvertPixelsToByteArray(pixels);
                    long timestamp = startUnixTimestamp + currentTime;
                    OnTextureUpdated.OnNext((timestamp, frameData));
                });
            }
        }
        private void SubscribeUpdateTexture()
        {
            OnTextureUpdated
                .ObserveOnMainThread()
                .Subscribe(tuple =>
                {
                    var (timestamp, frame) = tuple;
                    //TODO::process bytes

                }).AddTo(this);
        }
        private byte[] ConvertPixelsToByteArray(Color32[] pixels)
        {
            int size = pixels.Length * 4; // Color32는 4바이트(RGBA)
            byte[] bytes = new byte[size];

            GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                Marshal.Copy(ptr, bytes, 0, size);
            }
            finally
            {
                handle.Free();
            }

            return bytes;
        }

    }
    
}
