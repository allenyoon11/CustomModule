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
                throw new System.Exception($"[LoadElement] fail load uxml");
            }
            StyleSheet style = Resources.Load<StyleSheet>(ussPath);
            if (style == null)
            {
                throw new System.Exception($"[LoadElement] fail load uss");
            }
            if (string.IsNullOrEmpty(name))
            {
                //TemplateContainer
                VisualElement el = asset.Instantiate();
                el.styleSheets.Add(style);
                el.style.flexGrow = 1;
                return el;
            }
            else
            {
                VisualElement el = asset.Instantiate().Q<VisualElement>(name);
                el.styleSheets.Add(style);
                return el;
            }
        }
    }

}
