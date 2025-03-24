using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace neuroears.allen.utils
{
    public class ResizeImageCalculator
    {
        public static bool CalcSizeWithAspect(Vector2 parentSize, Vector2 targetSize, out Vector2 resize)
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

            if (parentRatio > targetRatio)
            {
                scaleFactor = parentHeight / targetHeight;
                float newWidth = targetWidth * scaleFactor;

                if (newWidth > parentWidth)
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
            else if (parentRatio < targetRatio)
            {
                scaleFactor = parentWidth / targetWidth;
                float newHeight = targetHeight * scaleFactor;

                if (newHeight > parentHeight)
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
                if (parentWidth >= targetWidth)
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

            if (resizeWidth > 0f && resizeHeight > 0f)
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
