using UnityEngine;
using Zenject;

public class GraphDotInstaller : MonoInstaller
{
    [Inject] GraphDotModel graphDot;
    public override void InstallBindings()
    {
        Container.BindInstance(graphDot);
        Container.BindInterfacesAndSelfTo<GraphDotPresenter>().AsSingle().NonLazy();
    }
}