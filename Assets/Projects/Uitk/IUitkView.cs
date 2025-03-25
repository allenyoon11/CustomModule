using UniRx;
using UnityEngine;

namespace neuroears.allen.uitk
{
    public interface IUitkView
    {
        public string uxmlPath { get; }
        public string ussPath { get; }
        public string globalUssPath { get; }
        public Subject<Unit> OnViewLoaded { get; }
        public void LoadView();
    }

}
