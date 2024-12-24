using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace allen.utils
{

    [RequireComponent(typeof (RectTransform))]
    public class DynamicImageResizer : MonoBehaviour
    {
        public RectTransform parent;
        public RawImage child;

        private RectTransform childRect;
        private Vector2 targetSize = Vector2.zero;
        private void Awake()
        {
            CheckValidation();
        }
        private void Start()
        {
            Resize();
        }
        private void CheckValidation()
        {
            if (parent == null) parent = GetComponent<RectTransform>();
            if (child == null) Debug.LogError("child is null");
            childRect = child.GetComponent<RectTransform>();
        }

        private void OnRectTransformDimensionsChange()
        {
            Resize();
        }
        public void Resize()
        {
            if (parent == null || child == null || childRect == null || child.texture == null) return;

            var parentWidth = parent.rect.width;
            var parentHeight = parent.rect.height;
            targetSize = new Vector2(child.texture.width, child.texture.height);

            if (parentWidth == 0 || parentHeight == 0 || targetSize == Vector2.zero) return;

            if (child != null && targetSize != Vector2.zero)
            {
                bool result = CalcSize(new Vector2(parentWidth, parentHeight), targetSize, out Vector2 resize);
                if (result)
                {
                    childRect.sizeDelta = resize;
                    childRect.anchorMin = Vector2.one * 0.5f;
                    childRect.anchorMax = Vector2.one * 0.5f;
                    childRect.anchoredPosition = Vector2.zero;
                }
            }
        }
        private bool CalcSize(Vector2 parentSize, Vector2 targetSize, out Vector2 resize)
        {
            resize = Vector2.zero;

            float parentWidth = parentSize.x;
            float parentHeight = parentSize.y;
            float targetWidth = targetSize.x;
            float targetHeight = targetSize.y;
        
            float parentRatio = parentWidth / parentHeight;
            float targetRatio = targetWidth / targetHeight;

            float resizeWidth = 0f;
            float resizeHeight = 0f;
            float scaleFactor = 1f;

            if(parentRatio > targetRatio)
            {
                scaleFactor = parentHeight / targetHeight;
                float newWidth = targetWidth * scaleFactor;

                if(newWidth > parentWidth)
                {
                    resizeWidth = parentWidth;
                    resizeHeight = resizeWidth / targetRatio;
                }
                else
                {
                    resizeWidth = newWidth;
                    resizeHeight = parentHeight;
                }
            }
            else if(parentRatio < targetRatio)
            {
                scaleFactor = parentWidth / targetWidth;
                float newHeight = targetHeight * scaleFactor;

                if(newHeight > parentHeight)
                {
                    resizeHeight = parentHeight;
                    resizeWidth = resizeHeight * targetRatio;
                }
                else
                {
                    resizeHeight = newHeight;
                    resizeWidth = parentWidth;
                }
            }
            else
            {
                if(parentWidth >= targetWidth)
                {
                    resizeWidth = targetWidth;
                    resizeHeight = targetHeight;
                }
                else
                {
                    resizeWidth = parentWidth;
                    resizeHeight = parentHeight;
                }
            }

            if(resizeWidth > 0f && resizeHeight > 0f)
            {
                resize = new Vector2(resizeWidth, resizeHeight);
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
