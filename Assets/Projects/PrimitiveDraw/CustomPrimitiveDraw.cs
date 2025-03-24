using UnityEngine;
using UnityEngine.UI;

namespace PrimitiveDraw
{
    public class CustomPrimitiveDraw
    {
        private PrimitiveDraw draw;

        public CustomPrimitiveDraw(Camera camera = null, int layer = 0)
        {
            draw = new PrimitiveDraw(camera, layer);
        }
        public void DrawRectOnRawImage(in RawImage rawImage, params Rect[] normRects)
        {
            draw.Clear();

            RectTransform rawImageTransform = rawImage.GetComponent<RectTransform>();
            Canvas canvas = rawImage.canvas;
            Camera canvasCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            foreach (var rect in normRects)
            {
                var previewRect = rawImageTransform.rect;
                float rawWidth = previewRect.width;
                float rawHeight = previewRect.height;

                // 정규화된 좌표를 RawImage의 로컬 좌표계로 변환 (UI 좌표)
                float x = (rect.x * rawWidth) - (rawWidth * 0.5f);
                float y = (rect.y * rawHeight) - (rawHeight * 0.5f);
                float width = rect.width * rawWidth;
                float height = rect.height * rawHeight;

                Vector3 localMin = new Vector3(x, y, 0);
                Vector3 localMax = new Vector3(x + width, y + height, 0);
                Vector3 localTopLeft = new Vector3(x, y + height, 0);
                Vector3 localBottomRight = new Vector3(x + width, y, 0);

                // RectTransform의 UI 로컬 좌표를 월드 좌표로 변환
                Vector3 worldMin = rawImageTransform.TransformPoint(localMin);
                Vector3 worldMax = rawImageTransform.TransformPoint(localMax);
                Vector3 worldTopLeft = rawImageTransform.TransformPoint(localTopLeft);
                Vector3 worldBottomRight = rawImageTransform.TransformPoint(localBottomRight);

                // PrimitiveDraw로 사각형 그리기
                draw.Quad(worldMin, worldBottomRight, worldMax, worldTopLeft, thickness: 2f);

                //Debug.Log($"Rect: {rect}, WorldMin: {worldMin}, WorldMax: {worldMax}");
            }

            draw.Apply();
        }
        public void DrawPointOnRawImage(in RawImage rawImage, float thickness, params Vector3[] normVec)
        {
            draw.Clear();

            RectTransform rawImageTransform = rawImage.GetComponent<RectTransform>();
            Canvas canvas = rawImage.canvas;
            Camera canvasCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            foreach (var vec in normVec)
            {
                var previewRect = rawImageTransform.rect;
                float rawWidth = previewRect.width;
                float rawHeight = previewRect.height;

                // 정규화된 좌표를 RawImage 내부 좌표로 변환 (UI 좌표)
                float x = (vec.x * rawWidth) - (rawWidth * 0.5f);
                float y = (vec.y * rawHeight) - (rawHeight * 0.5f);

                Vector3 localPoint = new Vector3(x, y, 0);
                Vector3 worldPoint;

                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    // Overlay 모드일 때 TransformPoint만 사용
                    worldPoint = rawImageTransform.TransformPoint(localPoint);
                }
                else
                {
                    // Screen Space - Camera 또는 World 모드일 때는 Screen 좌표를 거쳐야 함
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(
                        rawImageTransform,
                        RectTransformUtility.WorldToScreenPoint(canvasCamera, rawImageTransform.TransformPoint(localPoint)),
                        canvasCamera,
                        out worldPoint
                    );
                }

                // PrimitiveDraw로 점 그리기
                draw.Point(worldPoint, thickness);

                //Debug.Log($"NormVec: {vec}, WorldPoint: {worldPoint}");
            }

            draw.Apply();
        }

    }

}
