using UnityEngine;
using Zenject;

public class GraphLegendInstaller : MonoInstaller
{
    [Inject]
    private GraphLegendModel graphLegend;
    public override void InstallBindings()
    {
        Container.BindInstance(graphLegend);
        Container.BindInterfacesAndSelfTo<GraphLegendPresenter>().AsSingle().NonLazy();
    }
}