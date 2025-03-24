using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace neuroears.allen.uitk
{
    [RequireComponent(typeof(UIDocument))]
    public class UitkRootView : MonoBehaviour
    {
        protected VisualElement root;
        public Subject<VisualElement> OnViewLoaded { get; private set; } = new Subject<VisualElement>();
        public VisualElement Root => root;
        protected virtual void Awake()
        {
            if (root == null)
            {
                root = GetComponent<UIDocument>().rootVisualElement;
            }
            root.style.flexGrow = 1;
        }
        protected virtual void Start()
        {
            OnViewLoaded.OnNext(root);
        }
    }
}
