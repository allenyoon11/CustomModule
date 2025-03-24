using UnityEngine;
using UnityEngine.UI;

namespace neuroears.allen.utils
{
    [RequireComponent (typeof(RawImage), (typeof(AspectRatioFitter)))]
    public class CustomRawImage : MonoBehaviour
    {
        private RawImage rawImage;
        private AspectRatioFitter aspectRatioFitter;
        public RawImage RawImage => rawImage;
        private void Awake()
        {
            rawImage = GetComponent<RawImage>();
            aspectRatioFitter = GetComponent<AspectRatioFitter>();
            aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            if(rawImage.texture == null)
            {
                SetRatio(1f);
            }
            else
            {
                SetRatio(rawImage.texture.width, rawImage.texture.height);
            }
        }
        public void SetTexture(Texture tex)
        {
            if (tex == null) return;
            rawImage.texture = tex;
            SetRatio(tex.width, tex.height);
        }
        private void SetRatio(float ratio)
        {
            aspectRatioFitter.aspectRatio = ratio;
        }
        private void SetRatio(int width, int height) => SetRatio((float)width / height); 
    }

}
