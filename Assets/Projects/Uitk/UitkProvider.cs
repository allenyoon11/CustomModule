using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace neuroears.allen.uitk
{
    public class UitkProvider
    {
        public static T LoadElement<T>(string uxmlPath, string ussPath, string name) where T : VisualElement
        {
            VisualTreeAsset asset = LoadAsset(uxmlPath);
            StyleSheet style = LoadStyleSheet(ussPath);

            if (string.IsNullOrEmpty(name))
            {
                //TemplateContainer
                var el = asset.Instantiate();
                if (style != null) el.styleSheets.Add(style);
                el.style.flexGrow = 1;
                return el as T;
            }
            else
            {
                var el = asset.Instantiate().Q<T>(name);
                if (style != null) el.styleSheets.Add(style);
                return el as T;
            }
        }
        public static VisualTreeAsset LoadAsset(string uxmlPath)
        {
            VisualTreeAsset asset = Resources.Load<VisualTreeAsset>(uxmlPath);
            CheckValidateAsset(asset, uxmlPath);
            return asset;
        }
        public static StyleSheet LoadStyleSheet(string ussPath)
        {
            StyleSheet style = Resources.Load<StyleSheet>(ussPath);
            CheckValidateStyle(style, ussPath);
            return style;
        }
        public static void AddStyleSheet<T>(ref T target, string ussPath) where T : VisualElement
        {
            StyleSheet style = LoadStyleSheet(ussPath);
            target.styleSheets.Add(style);
        }
        public static T1 FindAsset<T1, T2>(T2 src, string name) where T1 : VisualElement where T2 : VisualElement
        {
            return src.Q<T1>(name);
        }
        public static void AddClassName<T>(ref T target, string className) where T : VisualElement
        {
            if (target == null || string.IsNullOrEmpty(className)) return;
            target.AddToClassList(className);
        }
        public static void RemoveClassName<T>(ref T target, string className) where T : VisualElement
        {
            if (target == null || string.IsNullOrEmpty(className)) return;
            target.RemoveFromClassList(className);
        }
        public static void OnButtonClick(ref Button target, Action cb)
        {
            if (target == null || cb == null) return;
            target.clicked += cb;
        }
        public static void OnClick<T>(ref T target, Action cb) where T : VisualElement
        {
            if (target == null || cb == null) return;
            target.RegisterCallback<ClickEvent>(evt =>
            {
                cb?.Invoke();
            });
        }
        public static void OnPointerUp<T>(ref T target, Action cb) where T : VisualElement
        {
            if (target == null || cb == null) return;
            target.RegisterCallback<PointerUpEvent>(evt =>
            {
                cb?.Invoke();
            });
        }
        public static void OnPointerDown<T>(ref T target, Action cb) where T : VisualElement
        {
            if (target == null || cb == null) return;
            target.RegisterCallback<PointerDownEvent>(evt =>
            {
                cb?.Invoke();
            });
        }
        public static void OnScroll(ScrollView target, Action cb, Vector2 speed)
        {
            target.RegisterCallback<WheelEvent>(evt =>
            {
                cb?.Invoke();

                var delta = evt.delta;
                var offset = target.scrollOffset + new Vector2(delta.x * speed.x, delta.y * speed.y);

                // 선택적: 스크롤 범위 제한
                offset.y = Mathf.Clamp(offset.y, 0, target.contentContainer.layout.height - target.layout.height);
                offset.x = Mathf.Clamp(offset.x, 0, target.contentContainer.layout.width - target.layout.width);

                target.scrollOffset = offset;
                evt.StopPropagation();
            });
        }
        #region private
        private static void CheckValidateAsset(VisualTreeAsset asset, string uxmlPath)
        {
            if (asset == null)
            {
                throw new System.Exception($"[CheckValidateAsset] asset is null (path: {uxmlPath})");
            }
            if (uxmlPath.Split("/").Last() != asset.name)
            {
                Debug.LogError($"[CheckValidateAsset] fail load uxml ");
            }
        }
        private static void CheckValidateStyle(StyleSheet style, string ussPath)
        {
            if (style == null)
            {
                throw new System.Exception($"[CheckValidateStyle] style is null (path: {ussPath})");
            }
            if (ussPath.Split("/").Last() != style.name)
            {
                Debug.LogError($"[CheckValidateStyle] fail load uxml");
            }
        }
        #endregion

    }
}
