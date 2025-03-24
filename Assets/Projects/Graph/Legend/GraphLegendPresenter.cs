using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using System;
using UniRx.Triggers;
using System.Linq;

public class GraphLegendPresenter : IInitializable
{
    private GraphLegendView view;
    private GraphLegendModel model;
    private GraphModel graphModel;

    public GraphLegendPresenter(
        GraphLegendView view
        , GraphLegendModel model
        , GraphModel graphModel

        )
    {
        this.view = view;
        this.model = model;
        this.graphModel = graphModel;

    }
    public void Initialize()
    {
        SetSubscribe();
    }

    private void SetSubscribe()
    {
        view.OnEnableAsObservable().Subscribe(_ => SetInitView()).AddTo(view);
        view.OnToggleChanged.Skip(1).Subscribe(isOn => ChangeToggleValue(isOn)).AddTo(view);
        model.OnToggleChanged.Subscribe(isOn => view.SetIsOn(isOn)).AddTo(view);
    }

    private void ChangeToggleValue(bool isOn)
    {
        //INFO::라인이 없으면 동작하지 않음 / 초기값은 통제하지 않음
        //if (graphModel.ListLineModelsRx.Count == 0)
        //{
        //    view.SetIsOn(!isOn);
        //    return;
        //}
        //INFO::타겟라인이 없으면 체크 해제 / 초기값은 통제하지 않음
        //var targetLines = graphModel.ListLineModelsRx.Where(line => line.ColorRx.Value == model.color).ToList();
        //if (targetLines.Count == 0)
        //{
        //    view.SetIsOn(false);
        //    return;
        //}
        model.IsOnRx.Value = isOn;
        graphModel.OnLegendToggleChanged.OnNext(model);
    }

    private void SetInitView()
    {
        view.SetColor(model.ColorRx.Value);
        view.SetName(model.LegendRx.Value);
        view.SetIsOn(model.IsOnRx.Value);
    }
}
