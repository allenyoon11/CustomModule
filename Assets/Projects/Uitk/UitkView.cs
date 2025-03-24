using UnityEngine;
using UnityEngine.UIElements;

namespace neuroears.allen.uitk
{
    public abstract class UitkView: IUitkView
    {
        protected VisualElement root;
        public abstract string uxmlPath { get; }
        public abstract string ussPath { get; }
        public abstract void LoadView();
    }

}
