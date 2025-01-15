using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace neuroears.allen.utils.webcam
{
    /// <summary>
    /// v1.0.03
    /// </summary>
    public class WebcamPlayer : MonoBehaviour
    {
        public RawImage rawImage;
        public bool mirrorMode = true;
        public bool processItSelf = true;
        public bool showLog = true;
        public int webcamIndex = 0;
        public int width = 640;
        public int height = 360;
        public int fps = 100;
        //
        private WebCamTexture wTex;
        private RenderTexture rTex;
        private DynamicImageResizer resizer;
        private IWebcamDataStore store;
        private bool isActive = true;
        //for record 
        private bool isRecording = false;
        private long nextTimeInterval = 0;
        private long timeInterval = 10; //10ms = 100fps
        private CancellationTokenSource cts = null;
        private Subject<(long timestamp, WebCamTexture wTex)> OnTextureUpdated = new Subject<(long timestamp, WebCamTexture wTex)>();
        //External
        public int WebcamIndex => webcamIndex;
        public int Width => width;
        public int Height => height;
        public int Fps => fps;
        public bool IsRecording => isRecording;
        public WebCamTexture webcamTex => wTex;

        //INFO::when you need
        //public Subject<(long timestamp, Color32[] frame)> OnWebcamFrameUpdated { get; private set; } = new Subject<(long timestamp, Color32[] frame)>();

        private void Awake()
        {
            if (processItSelf)
            {
                InitWebcam();
            }
        }
        private void OnEnable()
        {
            if (processItSelf)
            {
                LoadWebcam();
            }
        }
        private void Start()
        {
            SubscribeUpdateTexture();
        }
        #region UI
        public void InitWebcam(int idx = -1, int width = -1, int height = -1, int fps = -1)
        {
            this.webcamIndex = idx < 0 ? this.webcamIndex : idx;
            this.width = width < 0 ? this.width : width;
            this.height = height < 0 ? this.height : height;
            this.fps = fps < 0 ? this.fps : fps;
            if (resizer == null) resizer = GetComponent<DynamicImageResizer>();
            ChangeMirrorMode(mirrorMode);
        }
        public void LoadWebcam(IWebcamDataStore store = null)
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            if (devices.Length > 0 && webcamIndex < devices.Length)
            {
                string deviceName = devices[webcamIndex].name;
                wTex = new WebCamTexture(deviceName, width, height, fps);
                rawImage.texture = wTex;
                wTex.Play();
                
                //INFO::After Play, cam switches to a supported resolution.
                if (width != wTex.width || height != wTex.height)
                {
                    width = wTex.width;
                    height = wTex.height;
                }
                
                resizer?.Resize();
                if(store == null)
                {
                    this.store = new WebcamDataStore(webcamIndex, deviceName, width, height, fps);
                }
                else
                {
                    this.store = store;
                    this.store.SetInit(webcamIndex, deviceName, width, height, fps);
                }
                if (showLog) Debug.Log($"<color=yellow>[WebcamPlayer-{webcamIndex}] Connected (idx: {webcamIndex} | name: {deviceName} | resolution: {width}x{height} | fps: {fps} | mirrorMode: {mirrorMode})</color>");
            }
            else
            {
                Debug.LogWarning($"[WebcamPlayer-{webcamIndex}] No webcam found or invalid webcam index.");
            }
        }
        public void ChangeMirrorMode(bool mirrorMode)
        {
            transform.localRotation = Quaternion.Euler(0, mirrorMode ? 180 : 0, 0);
        }
        public void Show()
        {
            rawImage.gameObject.SetActive(true);
            isActive = true;
        }
        public void Hide()
        {
            rawImage.gameObject.SetActive(false);
            isActive = false;
        }
        #endregion
        #region Record
        public void StartRecord()
        {
            if (showLog) Debug.Log($"[WebcamPlayer-{webcamIndex}] Start Record");
            store.ReadyToRecord();

            rTex = new RenderTexture(width, height, 0); // RenderTexture 생성
            nextTimeInterval = 0;
            timeInterval = 10;
            cts = new CancellationTokenSource();
            isRecording = true;
            CaptureFramesAsync(cts.Token).Forget();
        }
        public void StopRecord()
        {
            if (cts.Token != null) cts.Cancel();
            isRecording = false;
            store.StopRecord(out long startTimestamp, out long endTimestamp, out long duration, out int frameCount, out float fps);
            if (showLog) Debug.Log($"[WebcamPlayer-{webcamIndex}] Stop Record | duration: {duration} | frameCount: {frameCount} | fps: {fps}");
        }
        public WebcamData GetWebcamData() => store.GetWebcamData();
        public List<FrameData<Color32[]>> GetFrameData() => store.GetFrameData();
        public List<Color32[]> GetRawData(out float fps) => store.GetRawData(out fps);
        private async UniTaskVoid CaptureFramesAsync(CancellationToken token)
        {
            await UniTask.RunOnThreadPool(() =>
            {
                while (isRecording)
                {
                    if (cts.IsCancellationRequested)
                    {
                        break;
                    }
                    
                    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                    if (nextTimeInterval <= 0)
                    {
                        nextTimeInterval = timestamp;
                    }
                    if (timestamp >= nextTimeInterval)
                    {
                        OnTextureUpdated.OnNext((timestamp, wTex));
                        nextTimeInterval = timestamp + timeInterval;
                    }
                }
            });
        }
        private void SubscribeUpdateTexture()
        {
            OnTextureUpdated
                .ObserveOnMainThread()
                .Subscribe(async tuple =>
                {
                    var (timestamp, wTex) = tuple;

                    // RenderTexture에 WebcamTexture 블리팅
                    Graphics.Blit(wTex, rTex);

                    // AsyncGPUReadback 비동기 처리
                    var request = await AsyncGPUReadback.Request(rTex, 0);
                    if (request.hasError)
                    {
                        Debug.LogError("AsyncGPUReadback request failed.");
                        return;
                    }

                    // Add data to store
                    Color32[] frame = request.GetData<Color32>().ToArray();
                    store.AddFrame(timestamp, frame);
                    //OnWebcamFrameUpdated.OnNext((timestamp, frame)); //INFO::when you need; (for external)
                }).AddTo(this);
        }
        #endregion

    }
    public interface IWebcamDataStore
    {
        public WebcamData GetWebcamData();
        public List<FrameData<Color32[]>> GetFrameData();
        public List<Color32[]> GetRawData(out float fps);
        public void SetInit(int webcamIndex, string deviceName, int width, int height, int fps);
        public void ReadyToRecord();
        public void AddFrame(long timestamp, Color32[] frame);
        public void StopRecord();
        public void StopRecord(out long startTimestamp, out long endTimestamp, out long duration, out int frameCount, out float fps);
    }
    public class WebcamDataStore : IWebcamDataStore
    {
        private WebcamData data;
        private bool showLog = true;
        public WebcamDataStore(int idx, string name, int width, int height, int fps)
        {
            this.data = new WebcamData(idx, name, width, height, fps);
        }
        public WebcamDataStore() { this.data = null; }
        public WebcamData GetWebcamData() => data;
        public List<FrameData<Color32[]>> GetFrameData() => data.GetFrameData();
        public List<Color32[]> GetRawData(out float fps) => data.GetRawData(out fps);
        
        public void SetInit(int idx, string name, int width, int height, int fps)
        {
            if(this.data == null) this.data = new WebcamData(idx, name, width, height, fps);
            else
            {
                this.data.idx = idx;
                this.data.name = name;
                this.data.width = width;
                this.data.height = height;
                this.data.fps = fps;
            }
        }
        public void ReadyToRecord()
        {
            if (data == null)
            {
                Debug.LogError($"not ready to data");
                return;
            }
            data.startTimestamp = 0;
            data.endTimestamp = 0;
            if (data.frameDataList == null) data.frameDataList = new List<FrameData<Color32[]>>();
            else data.frameDataList.Clear();
            data.lockData = false;
        }
        public void AddFrame(long timestamp, Color32[] frame)
        {
            if (data == null)
            {
                Debug.LogError($"not ready to data");
                return;
            }
            if (data.lockData)
            {
                if(showLog) Debug.LogWarning("isLocked");
                return;
            }
            var frameData = new FrameData<Color32[]>()
            {
                timestamp = timestamp,
                frame = frame
            };
            if (data.startTimestamp == 0) data.startTimestamp = timestamp;
            data.endTimestamp = timestamp;
            data.frameDataList.Add(frameData);
        }
        public void StopRecord()
        {
            data.lockData = true;
            var startTimestamp = data.startTimestamp;
            var endTimestamp = data.endTimestamp;
            var duration = endTimestamp - startTimestamp;
            var frameCount = data.frameDataList.Count;
            var fps = (float)frameCount / duration * 1000;

            data.realFps = fps;
        }
        public void StopRecord(out long startTimestamp, out long endTimestamp, out long duration, out int frameCount, out float fps)
        {
            data.lockData = true;
            startTimestamp = data.startTimestamp;
            endTimestamp = data.endTimestamp;
            duration = endTimestamp - startTimestamp;
            frameCount = data.frameDataList.Count;
            fps = (float)frameCount / duration * 1000;

            data.realFps = fps;
        }
    }
    public class WebcamData
    {
        public int idx;
        public string name;
        public int width;
        public int height;
        public int fps;
        public long startTimestamp;
        public long endTimestamp;
        public float realFps;
        public bool lockData;
        public List<FrameData<Color32[]>> frameDataList;
        public WebcamData(int idx, string name, int width, int height, int fps)
        {
            this.idx = idx;
            this.name = name;
            this.width = width;
            this.height = height;
            this.fps = fps;
            this.startTimestamp = -1;
            this.endTimestamp = -1;
            this.realFps = 0;
            this.lockData = false;
            this.frameDataList = new List<FrameData<Color32[]>>();
        }
        public WebcamData()
        {
            this.idx = -1;
        }
        public List<FrameData<Color32[]>> GetFrameData()
        {
            return frameDataList;
        }
        public List<FrameData<Color32[]>> GetFrameData(out float fps)
        {
            fps = realFps;
            return frameDataList;
        }
        public List<Color32[]> GetRawData(out float fps)
        {
            fps = realFps;
            return (System.Linq.Enumerable.Select(frameDataList, el => el.frame)).ToList();
        }
    }
    public class FrameData<T>
    {
        public long timestamp;
        public T frame;
    }
}
