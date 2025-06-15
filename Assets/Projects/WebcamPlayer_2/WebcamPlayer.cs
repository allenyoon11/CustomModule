using System;
using UnityEngine;
using UnityEngine.UI;

namespace neuroears.allen.utils.webcam2
{
    [RequireComponent(typeof(RawImage), typeof(AspectRatioFitter))]
    public class WebcamPlayer : MonoBehaviour
    {
        public bool processItSelf = true;
        public int webcamIndex;
        public int width = 640;
        public int height = 360;
        public int fps = 100;
        //
        private RawImage rawImage;
        private AspectRatioFitter arf;
        private WebCamDevice device;
        private bool isLoaded = false;
        //
        private WebCamTexture wTex;
        //
        public WebCamTexture WebcamTex => wTex;
        public bool IsLoaded => isLoaded;


        protected virtual void Awake()
        {
            if (processItSelf)
            {
                InitWebcam(width, height);
            }
        }
        protected virtual void OnEnable()
        {
            if (processItSelf)
            {
                LoadWebcam(webcamIndex, width, height, fps);
            }
        }
        private void OnDestroy()
        {
            wTex?.Stop();
        }
        private void InitWebcam(int width, int height)
        {
            rawImage = GetComponent<RawImage>();
            arf = GetComponent<AspectRatioFitter>();
            arf.aspectRatio = (float)width / height;
        }
        private void LoadWebcam(int webcamIndex, int width, int height, int fps)
        {
            var devices = WebCamTexture.devices;
            if (devices.Length == 0) throw new Exception("not found diveces");
            device = WebCamTexture.devices[webcamIndex];
            wTex = new WebCamTexture(device.name, width, height, fps);
            rawImage.texture = wTex;
            wTex.Play();
            ResetTextureValue();
            isLoaded = true;
        }

        private void ResetTextureValue()
        {
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
            Debug.Log($"{width}x{height} | {fps}");
        }
    }

}
