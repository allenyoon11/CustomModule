using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace neuroears.allen.uitk
{
    public class UitkProvider
    {
        public static VisualElement LoadElement(string uxmlPath, string ussPath, string name = "")
        {
            VisualTreeAsset asset = Resources.Load<VisualTreeAsset>(uxmlPath);
            if (asset == null)
            {
                throw new System.Exception($"[LoadElement] fail load uxml {uxmlPath}");
            }
            StyleSheet style = null;
            if (!string.IsNullOrEmpty(ussPath))
            {
                style = Resources.Load<StyleSheet>(ussPath);
                if (style == null)
                {
                    throw new System.Exception($"[LoadElement] fail load uss {ussPath}");
                }
            }
            if (string.IsNullOrEmpty(name))
            {
                //TemplateContainer
                VisualElement el = asset.Instantiate();
                if (style != null) el.styleSheets.Add(style);
                el.style.flexGrow = 1;
                return el;
            }
            else
            {
                VisualElement el = asset.Instantiate().Q<VisualElement>(name);
                if (style != null) el.styleSheets.Add(style);
                return el;
            }
        }
    }

}
