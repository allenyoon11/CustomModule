using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using Shapes;
using System;
using UniRx.Triggers;

public class GraphDotPresenter : IInitializable
{
    private GraphDotView dotView;
    private GraphDotModel dot;
    private GraphModel graph;
    private GraphView graphView;

    public GraphDotPresenter(
        GraphDotView dotView
        , GraphDotModel dot
        , GraphModel graph
        , GraphView graphView
        )
    {
        this.dotView = dotView;
        this.dot = dot;
        this.graph = graph;
        this.graphView = graphView;
    }

    public void Initialize()
    {
        SetSubscribe();
    }

    private void SetSubscribe()
    {
        dot.PositionRx.Subscribe(pos => DrawDot()).AddTo(dotView);
        dot.ColorRx.Subscribe(color => dotView.DotColor = color).AddTo(dotView);
        dot.RadiusRx.Subscribe(radius => dotView.Radius = radius).AddTo(dotView);
        dot.ActiveRx.Subscribe(active => dotView.Active = active).AddTo(dotView);

        Observable.Merge(graph.RangeXRx, graph.RangeYRx)
            .Subscribe(_ => DrawDot()).AddTo(dotView);
        graphView.OnRectTransformDimensionsChangeAsObservable()
            .Subscribe(_ => DrawDot()).AddTo(dotView);
    }

    private void DrawDot()
    {
        Vector2 _rangeX = graph.RangeXRx.Value;
        Vector2 _rangeY = graph.RangeYRx.Value;
        float _lengthX = _rangeX.y - _rangeX.x;
        float _lengthY = _rangeY.y - _rangeY.x;

        Rect _rect = graphView.RectTransform.rect;
        float _width = _rect.width;
        float _height = _rect.height;

        Vector2 point = dot.PositionRx.Value;
        if ((point.x >= _rangeX.x && point.x <= _rangeX.y) && (point.y >= _rangeY.x && point.y <= _rangeY.y))
        {
            float _x = _width * Mathf.Clamp01((point.x - _rangeX.x) / _lengthX);
            float _y = _height * Mathf.Clamp01((point.y - _rangeY.x) / _lengthY);
            dotView.Position = new Vector2(_x, _y);
        }
    }
}
