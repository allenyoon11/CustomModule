using UnityEngine;

namespace neuroears.allen.utils
{
    public class CustomTextureProvider
    {
        /// <summary>
        /// 중심을 기준으로 정사각형으로 크롭 (ratio = 1)
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public static void SquareCrop(RenderTexture src, ref Texture2D dst, float ratio = 0.5f)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                ratio = 1f;
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                ratio = 0.5f;
            }
            int width = src.width;
            int height = src.height;
            int size = Mathf.RoundToInt((Mathf.Min(width, height)) * ratio);

            int x = (width - size) / 2;
            int y = (height - size) / 2;

            RenderTexture rt = new RenderTexture(width, height, 0);
            Graphics.Blit(src, rt);

            RenderTexture.active = rt;
            if (dst == null || dst.width != size || dst.height != size)
            {
                dst = new Texture2D(size, size, TextureFormat.RGBA32, false);
            }
            dst.ReadPixels(new Rect(x, y, size, size), 0, 0);
            dst.Apply();

            RenderTexture.active = null;
            rt.Release();
        }
        public static void SquareCrop(RenderTexture src, ref RenderTexture dst, float ratio = 1f)
        {
            if (src == null || src.width == 0 || src.height == 0) return;

            if (Application.platform == RuntimePlatform.Android)
            {
                ratio = 1f;
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor ||  Application.platform == RuntimePlatform.WindowsPlayer)
            {
                ratio = 0.5f;
            }
            int width = src.width;
            int height = src.height;
            int size = Mathf.RoundToInt((Mathf.Min(width, height)) * ratio);

            int x = (width - size) / 2;
            int y = (height - size) / 2;

            Rect normRect = new Rect((float)x / width, (float)y / height, (float)size / width, (float)size / height);

            CropTexture(src, ref dst, normRect);
        }

        public static void CropTexture(RenderTexture src, ref RenderTexture dst, Rect normRect)
        {
            if (src == null || src.width == 0 || src.height == 0 || normRect.width == 0 || normRect.height == 0) return;

            Material cropMaterial = new Material(Shader.Find("Custom/CropShader"));

            cropMaterial.SetVector("_Rect", new Vector4(normRect.x, normRect.y, normRect.width, normRect.height));

            int size = Mathf.RoundToInt(normRect.width * src.width);
            if (dst == null || dst.width != size || dst.height != size)
            {
                dst?.Release();
                dst = new RenderTexture(size, size, 0, src.format);
            }

            Graphics.Blit(src, dst, cropMaterial);
        }
        public static void CropWithSize(RenderTexture src, ref Texture2D dst, Vector2 ratio)
        {
            int width = src.width;
            int height = src.height;

            // 비율을 기반으로 자를 영역 크기 계산
            int cropWidth = Mathf.Clamp(Mathf.RoundToInt(width * ratio.x), 1, width);
            int cropHeight = Mathf.Clamp(Mathf.RoundToInt(height * ratio.y), 1, height);

            // 중앙을 기준으로 크롭 영역 설정
            int startX = (width - cropWidth) / 2;
            int startY = (height - cropHeight) / 2;

            // 1. 원본을 RenderTexture로 변환 (전체 복사)
            RenderTexture rt = new RenderTexture(width, height, 0);
            Graphics.Blit(src, rt);

            // 2. RenderTexture에서 중심 영역만 ReadPixels()
            RenderTexture.active = rt;
            if (dst == null || dst.width != cropWidth || dst.height != cropHeight)
            {
                dst = new Texture2D(cropWidth, cropHeight, TextureFormat.RGBA32, false);
            }
            dst.ReadPixels(new Rect(startX, startY, cropWidth, cropHeight), 0, 0);
            dst.Apply();

            // 3. 메모리 정리
            RenderTexture.active = null;
            rt.Release();
        }
        /// <summary>
        /// 파이썬 Rect 를 유니티 Rect 로 변환
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rect MapToUnityRect(in Rect rect)
        {
            return new Rect(rect.x, 1.0f - (rect.y + rect.height), rect.width, rect.height);
        }
        /// <summary>
        /// 직사각형을 큰 길이 기준으로 정사각형 Rect 반환
        /// </summary>
        /// <param name="normalRect"></param>
        /// <returns></returns>
        public static Rect ConvertSquareRect(Rect normalRect)
        {
            float size = normalRect.width > normalRect.height ? normalRect.width : normalRect.height;
            float center_x = normalRect.x + normalRect.width / 2;
            float center_y = normalRect.y + normalRect.height / 2;
            return new Rect(center_x - size / 2, center_y - size / 2, size, size);
        }
        public static Vector3 MapToUnityKeypoint(in Vector3 normPoint)
        {
            return new Vector3(normPoint.x, 1 - normPoint.y, normPoint.z);
        }
        public static Rect ConvertRectByOffset(in Rect src, float top, float down, float left, float right)
        {
            float originalWidth = src.width;
            float originalHeight = src.height;

            // 새로운 크기 계산
            float newWidth = originalWidth * (1 + left + right);
            float newHeight = originalHeight * (1 + top + down);

            // 크기가 0보다 작아지는 경우 최소 0으로 보정
            if (newWidth < 0) newWidth = 0;
            if (newHeight < 0) newHeight = 0;

            // 새로운 위치 계산
            float newX = src.x - (originalWidth * left);
            float newY = src.y - (originalHeight * down); // down이 0이면 그대로 유지됨

            return new Rect(newX, newY, newWidth, newHeight);
        }
        public static Rect ConvertSquareRectByOffsetY(Rect src, float top, float down)
        {
            float originalHeight = src.height;
            float newHeight = originalHeight * (1 + top + down);
            if (newHeight < 0) newHeight = 0; // 최소 0 보정

            float newWidth = newHeight; // 정사각형 유지

            // 중심을 기준으로 x 조정
            float centerX = src.x + src.width / 2f;
            float newX = centerX - (newWidth / 2f);

            // y 위치 조정 (down이 0이면 원본 유지)
            float newY = src.y - (originalHeight * down);

            return new Rect(newX, newY, newWidth, newHeight);
        }
        public static Rect ConvertSquareRectByOffsetX(Rect src, float left, float right)
        {
            float originalWidth = src.width;
            float newWidth = originalWidth * (1 + left + right);
            if (newWidth < 0) newWidth = 0; // 최소 0 보정

            float newHeight = newWidth; // 정사각형 유지

            // 중심을 기준으로 y 조정
            float centerY = src.y + src.height / 2f;
            float newY = centerY - (newHeight / 2f);

            // x 위치 조정 (left이 0이면 원본 유지)
            float newX = src.x - (originalWidth * left);

            return new Rect(newX, newY, newWidth, newHeight);
        }
    }

}
