using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace neuroears.allen.utils
{
    public static class MathHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digit"></param>
        /// <param name="type">0:floor, 1:round, 2:ceil</param>
        /// <returns></returns>
        public static float GetDecimalByDigit(float value, int digit, int type)
        {
            var pow = Mathf.Pow(10f, digit);
            switch (type)
            {
                case 0:
                    return Mathf.Floor(value * pow) / pow;
                case 1:
                default:
                    return Mathf.Round(value * pow) / pow;
                case 2:
                    return Mathf.Ceil(value * pow) / pow;
            }
        }
        public static float Convert180Angle(float angle)
        {
            float _angle = angle % 360;
            if (_angle > 180) _angle -= 360;
            else if (_angle < -180) _angle += 360;
            return _angle;
        }
        public static Vector3 Convert180Angle(Vector3 angle)
        {
            return new Vector3(Convert180Angle(angle.x), Convert180Angle(angle.y), Convert180Angle(angle.z));
        }
        public static Vector2 CalcMaxSizeWithRatio(int dstWidth, int dstHeight, int targetWidth, int targetHeight, out int resizeWidth, out int resizeHeight)
        {
            Vector2 resize = Vector2.one;
            resizeWidth = 0;
            resizeHeight = 0;

            float _resizeWidth = 0f;
            float _resizeHeight = 0f;

            float dstRatio = (float)dstWidth / dstHeight;
            float targetRatio = (float)targetWidth / targetHeight;

            float scaleFactor = 1f;

            if (dstRatio > targetRatio)
            {
                scaleFactor = dstHeight / targetHeight;
                float newWidth = targetWidth * scaleFactor;

                if (newWidth > dstWidth)
                {
                    _resizeWidth = dstWidth;
                    _resizeHeight = _resizeWidth / targetRatio;
                }
                else
                {
                    _resizeWidth = newWidth;
                    _resizeHeight = dstHeight;
                }
            }
            else if (dstRatio < targetRatio)
            {
                scaleFactor = dstWidth / targetWidth;
                float newHeight = targetHeight * scaleFactor;

                if (newHeight > dstHeight)
                {
                    _resizeHeight = dstHeight;
                    _resizeWidth = _resizeHeight * targetRatio;
                }
                else
                {
                    _resizeHeight = newHeight;
                    _resizeWidth = dstWidth;
                }
            }
            else
            {
                if (dstWidth >= targetWidth)
                {
                    _resizeWidth = targetWidth;
                    _resizeHeight = targetHeight;
                }
                else
                {
                    _resizeWidth = dstWidth;
                    _resizeHeight = dstHeight;
                }
            }

            if (_resizeWidth > 0f && _resizeHeight > 0f)
            {
                resizeWidth = Mathf.FloorToInt(_resizeWidth);
                resizeHeight = Mathf.FloorToInt(_resizeHeight);
                return resize = new Vector2(_resizeWidth, _resizeHeight);
            }
            else
            {
                return Vector2.zero;
            }
        }
    }

}
