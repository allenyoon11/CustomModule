using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace neuroears.allen.uitk
{
    public abstract class UitkView: IUitkView
    {
        protected VisualElement root;
        public abstract string uxmlPath { get; }
        public abstract string ussPath { get; }
        public virtual string globalUssPath { get; } = "UI/Global/uss/Global";
        public Subject<Unit> OnViewLoaded { get; } = new Subject<Unit>();
        public virtual void LoadView()
        {
            OnViewLoaded.OnNext(Unit.Default);
        }
    }

}
