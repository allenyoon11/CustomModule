using System;
using System.Collections.Concurrent;
using System.Threading;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using TMPro;

namespace neuroears.allen.utils.webcam2.record
{
    [RequireComponent(typeof(WebcamPlayer))]
    public class WebcamRecorder : MonoBehaviour
    {
        [Range(1, 30)]
        public int recordTime = 3;
        public TMP_Text textTime;
        //
        private WebcamPlayer player;
        private ConcurrentQueue<FrameData> frameQueue;
        private CancellationTokenSource cts;
        //
        private long startTime = 0;
        private long currentTime = 0;
        private bool isRecording = false;
        public WebCamTexture webCamTexture => player.WebcamTex;

        private void Awake()
        {
            Application.targetFrameRate = 120;
            player = GetComponent<WebcamPlayer>();
            frameQueue = new ConcurrentQueue<FrameData>();
        }
        private void Start()
        {
            SubscribeKeypress();
        }
        private void Update()
        {
            if (isRecording)
            {
                var time = (float)(currentTime - startTime) / 1000;
                if (time >= recordTime) StopRecord();
                textTime.text = $"{time}";
            }
        }
        private void OnDestroy()
        {
            cts?.Cancel();
        }
        public void StartRecord()
        {
            Debug.Log($"[StartRecord] {3}");
            cts = new CancellationTokenSource();
            frameQueue.Clear();
            startTime = 0;
            currentTime = 0;
            isRecording = true;
            CaptureLoopAsync(cts.Token).Forget();
            //EncodeLoopAsync(cts.Token).Forget();
        }
        public void StopRecord()
        {
            cts.Cancel();
            isRecording = false;
            //
            var lastFrame = frameQueue.Last();
            long duration = lastFrame.timestampMs - startTime;
            int count = frameQueue.Count;
            float fps = (float)count / duration * 1000;

            Debug.Log($"[StopRecord] count: {count} | duration: {duration} | fps: {fps}");
        }


        private async UniTaskVoid CaptureLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.WaitForEndOfFrame();

                if (webCamTexture.didUpdateThisFrame)
                {
                    // 1. Immediately record timestamp
                    currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    if(startTime == 0) startTime = currentTime;

                    // 2. Copy texture
                    Texture2D frameTexture = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
                    frameTexture.SetPixels32(webCamTexture.GetPixels32());
                    frameTexture.Apply();

                    // 3. Enqueue FrameData
                    frameQueue.Enqueue(new FrameData(frameTexture, currentTime));
                }

                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
            }
        }

        private async UniTaskVoid EncodeLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (frameQueue.TryDequeue(out FrameData frameData))
                {
                    // TODO: Push frameData.texture and frameData.timestampMs into FFmpegSession
                    // Example:
                    // ffmpegSession.PushFrame(frameData.texture, frameData.timestampMs);

                    // After pushing, you can destroy the texture to free memory
                    UnityEngine.Object.Destroy(frameData.texture);
                }
                else
                {
                    await UniTask.Delay(1, cancellationToken: token);
                }
            }
        }

        private void SubscribeKeypress()
        {
            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.S))
                .Subscribe(_ =>
                {
                    if(!player.IsLoaded)
                    {
                        Debug.LogError("Player is not ready");
                        return;
                    }
                    if (isRecording) StopRecord();
                    else StartRecord();
                }).AddTo(this);
        }
    }
    public class FrameData
    {
        public Texture2D texture;
        public long timestampMs;

        public FrameData(Texture2D texture, long timestampMs)
        {
            this.texture = texture;
            this.timestampMs = timestampMs;
        }
    }
}
