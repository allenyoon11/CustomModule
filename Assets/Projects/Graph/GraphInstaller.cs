using UnityEngine;
using Zenject;

public class GraphInstaller : MonoInstaller
{
    [Inject]
    private GraphModel graphModel;

    [SerializeField]
    private GraphGrid GraphGridPrefab;
    [SerializeField]
    private GraphTick GraphTickPrefab;
    [SerializeField]
    private GraphLineInstaller GraphLinePrefab;
    [SerializeField]
    private GraphLegendInstaller GraphLegendPrefab;
    [SerializeField]
    private GraphDotView GraphDotPrefab;
    public override void InstallBindings()
    {
        Container.Bind<GraphModel>()
            .FromInstance(graphModel);
        
        Container.BindInterfacesAndSelfTo<GraphPresenter>()
            .AsSingle()
            .NonLazy();

        Container.BindMemoryPool<GraphGrid, GraphGrid.Pool>()
            .WithInitialSize(20)
            .FromComponentInNewPrefab(GraphGridPrefab)
            .UnderTransformGroup("Grids");

        Container.BindMemoryPool<GraphTick, GraphTick.Pool>()
            .WithInitialSize(20)
            .FromComponentInNewPrefab(GraphTickPrefab)
            .UnderTransformGroup("Ticks");

        Container.BindFactory<GraphLineModel, GraphLineView, GraphLineView.Factory>()
            .FromSubContainerResolve()
            .ByNewContextPrefab<GraphLineInstaller>(GraphLinePrefab)
            .UnderTransformGroup("Lines")
            .AsSingle();

        Container.BindFactory<GraphLegendModel, GraphLegendView, GraphLegendView.Factory>()
            .FromSubContainerResolve()
            .ByNewContextPrefab<GraphLegendInstaller>(GraphLegendPrefab)
            .UnderTransformGroup("Legends")
            .AsSingle();

        Container.BindFactory<GraphDotModel, GraphDotView, GraphDotView.Factory>()
            .FromSubContainerResolve()
            .ByNewContextPrefab<GraphDotInstaller>(GraphDotPrefab)
            .UnderTransformGroup("Dots")
            .AsSingle();
    }
}