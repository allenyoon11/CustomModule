using UnityEngine;
using Zenject;

public class GraphLineInstaller : MonoInstaller
{
    [Inject]
    private GraphLineModel graphLine;

    public override void InstallBindings()
    {
        Container.BindInstance(graphLine);
        Container.BindInterfacesAndSelfTo<GraphLinePresenter>().AsSingle().NonLazy();
    }
}