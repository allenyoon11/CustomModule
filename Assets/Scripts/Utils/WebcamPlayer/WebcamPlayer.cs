using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

namespace neuroears.allen.utils.webcam
{
    #region Summary
    /// <summary>
    /// v1.0.03::find device by name or idx;
    /// v1.0.02::set change info after load webcam
    /// v1.0.01::add resizer
    /// v1.0.00::basic
    /// </summary>
    #endregion
    public class WebcamPlayer : MonoBehaviour
    {
        public RawImage rawImage;
        public bool mirrorMode = true;
        public bool processItSelf = true;
        public bool showLog = true;
        public int webcamIndex = -1;
        public string deviceName = ""; // Arducam OV9782 USB Camera
        public int width = 640;
        public int height = 360;
        public int fps = 100;
        //
        protected bool isActive = true;
        protected bool isLoaded = false;
        protected WebCamTexture wTex;
        protected DynamicImageResizer resizer;
        //
        //External
        public int WebcamIdx => webcamIndex;
        public int Width => width;
        public int Height => height;
        public int Fps => fps;
        public bool IsLoaded => isLoaded;
        public string DeviceName => deviceName;
        public WebCamTexture WebcamTex => wTex;

        protected virtual void Awake()
        {
            if (processItSelf)
            {
                InitWebcam(webcamIndex, width, height, fps);
            }
        }
        protected virtual void OnEnable()
        {
            if (processItSelf)
            {
                LoadWebcam();
            }
        }
        protected virtual void OnDestroy()
        {
            if (rawImage != null)
            {
                rawImage.texture = null;
            }
            if(wTex != null)
            {
                wTex.Stop();
                wTex = null;
            }
            isLoaded = false;
        }
        protected virtual void Start()
        {

        }
        public virtual void InitWebcam(int idx, int width, int height, int fps)
        {
            if (resizer == null) resizer = GetComponent<DynamicImageResizer>();
            ChangeMirrorMode(mirrorMode);
        }
        public virtual void LoadWebcam()
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            WebCamDevice targetDevice = default;
            //v1.0.03::find device by name or idx;
            if (devices.Length == 0)
            {
                throw new Exception("not found any devices");
            }
            else if (devices.Length > 0)
            {
                if (string.IsNullOrEmpty(deviceName) && webcamIndex < 0) throw new Exception("enter device name or webcamIndex");
                if (webcamIndex > 0)
                {
                    targetDevice = devices[webcamIndex];
                    if (string.IsNullOrEmpty(deviceName))
                    {
                        deviceName = targetDevice.name;
                    }
                    else
                    {
                        if (targetDevice.name != deviceName) throw new Exception($"found device '{targetDevice.name}' by idx '{webcamIndex}'. but not match name '{deviceName}'");
                    }
                }
                else
                {
                    webcamIndex = Array.FindIndex(devices, el => el.name == deviceName);
                    if (webcamIndex < 0) throw new Exception($"not found device by name '{targetDevice.name}'");
                    targetDevice = devices[webcamIndex];
                }

                wTex = new WebCamTexture(deviceName, width, height, fps);
                rawImage.texture = wTex;
                wTex.Play();

                //v1.0.02::set change info after load webcam
                if (this.width != wTex.width)
                {
                    width = wTex.width;
                }
                if (this.height != wTex.height)
                {
                    height = wTex.height;
                }
                if (this.fps != (int)wTex.requestedFPS)
                {
                    this.fps = (int)wTex.requestedFPS;
                }
                //v1.0.01::add resizer
                resizer?.Resize();
                if (showLog) Debug.Log($"<color=yellow>[WebcamPlayer-{webcamIndex}] Connected (idx: {webcamIndex} | name: {deviceName} | resolution: {width}x{height} | fps: {fps} | mirrorMode: {mirrorMode})</color>");
                isLoaded = true;
            }
            else
            {
                Debug.LogWarning($"[WebcamPlayer-{webcamIndex}] No webcam found or invalid webcam index.");
            }
        }
        public virtual void ChangeMirrorMode(bool mirrorMode)
        {
            transform.localRotation = Quaternion.Euler(0, mirrorMode ? 180 : 0, 0);
        }

        public virtual void Show()
        {
            rawImage.gameObject.SetActive(true);
            isActive = true;
        }
        public virtual void Hide()
        {
            rawImage.gameObject.SetActive(false);
            isActive = false;
        }
    }

}
