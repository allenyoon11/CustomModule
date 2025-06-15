using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace neuroears.allen.utils.webcam2
{
    public class WebcamFPSMonitor : MonoBehaviour
    {
        public WebcamPlayer player;
        public TMP_Text fpsText;          // 측정된 FPS를 표시할 Text

        private WebCamTexture webCamTexture => player.WebcamTex;
        private int frameCount = 0;
        private float elapsedTime = 0f;
        private float updateInterval = 1f; // FPS를 업데이트하는 간격 (초)

        void Update()
        {
            if (webCamTexture != null && webCamTexture.isPlaying)
            {
                frameCount++;
                elapsedTime += Time.deltaTime;

                if (elapsedTime >= updateInterval)
                {
                    float fps = frameCount / elapsedTime;
                    if (fpsText != null)
                    {
                        fpsText.text = $"FPS: {fps:F2}";
                    }
                    frameCount = 0;
                    elapsedTime = 0f;
                }
            }
        }

    }
}