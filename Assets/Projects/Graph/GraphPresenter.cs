using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using System.Linq;
using System;

public class GraphPresenter : IInitializable
{
    private readonly GraphModel graph;
    private readonly GraphView graphView;

    public GraphPresenter(
        GraphModel model,
        GraphView view
        )
    {
        this.graph = model;
        this.graphView = view;
    }

    public virtual void Initialize()
    {
        SetSubscribes();
    }

    private void SetSubscribes()
    {
        // View 사이즈가 변할때 Model의 Size 변경
        graphView.OnRectTransformDimensionsChangeAsObservable().Subscribe(_ => ResetViewSize()).AddTo(graphView);

        // Drag Event
        graphView.EventTrigger.OnBeginDragAsObservable().Subscribe(eventData => Debug.Log("drag start")).AddTo(graphView);
        graphView.EventTrigger.OnEndDragAsObservable().Subscribe(eventData => Debug.Log("drag end")).AddTo(graphView);
        graphView.EventTrigger.OnDragAsObservable().Subscribe(eventData => OnDrag(eventData)).AddTo(graphView);

        // Subscribe
        graphView.EventTrigger.OnScrollAsObservable().Subscribe(eventData => OnScroll(eventData)).AddTo(graphView);

        // Graph Range가 변경됐을 때 View 갱신
        graph.RangeXRx.Subscribe(rangeX => OnRangeXChanged(rangeX)).AddTo(graphView);
        graph.RangeYRx.Subscribe(rangeY => OnRangeYChanged(rangeY)).AddTo(graphView);


        graph.ListLineModelsRx.ObserveAdd().Subscribe(lineModel => { graphView.AddLineView(lineModel.Value); }).AddTo(graphView);
        graph.ListLineModelsRx.ObserveRemove().Subscribe(lineModel => { graphView.RemoveLineView(lineModel.Value); }).AddTo(graphView);

        //INFO::Active line; by allen
        graph.OnLineActiveChanged.Subscribe(tuple => ActiveLine(tuple.lineModel, tuple.active)).AddTo(graphView);

        //INFO::Legend; by allen
        graph.ListLegendModelRx.ObserveAdd().Subscribe(legendModel => { graphView.AddLegendView(legendModel.Value); }).AddTo(graphView);
        graph.ListLegendModelRx.ObserveRemove().Subscribe(legendModel => { graphView.RemoveLegendView(legendModel.Value); }).AddTo(graphView);
        graph.OnLegendToggleChanged.Subscribe(legend => ActiveLinesByLegend(legend)).AddTo(graphView);

        //INFO::Dot; by allen
        graph.ListDotModelRx.ObserveAdd().Subscribe(dot => { graphView.AddDot(dot.Value); }).AddTo(graphView);
        graph.ListDotModelRx.ObserveRemove().Subscribe(dot => { graphView.RemoveDot(dot.Value); }).AddTo(graphView);

        //INFO::Axis info; by allen
        graph.AxisXRx.Subscribe(axis => { graphView.SetAxisXInfo(axis); }).AddTo(graphView);
        graph.AxisYRx.Subscribe(axis => { graphView.SetAxisYInfo(axis); }).AddTo(graphView);
    }

    private void ActiveLine(GraphLineModel lineModel, bool active)
    {
        graphView.ActiveLineView(lineModel, active);
    }

    //INFO::active by colors by line legend toggle
    private void ActiveLinesByLegend(GraphLegendModel legend)
    {
        if (graph.ListLineModelsRx.Count == 0)
        {
            Debug.Log("line list empty");
            return;
        }
        var targetLines = graph.ListLineModelsRx.Where(line => line.ColorRx.Value == legend.ColorRx.Value).ToList();
        if (targetLines.Count == 0)
        {
            Debug.Log("target line list empty");
            return;
        }
        foreach (var line in targetLines)
        {
            graphView.ActiveLineView(line, legend.IsOnRx.Value);
        }
    }

    private void ResetViewSize()
    {
        if (graphView == null) return;

        graphView.ResetBorder();

        OnRangeXChanged(graph.RangeXRx.Value);
        OnRangeYChanged(graph.RangeYRx.Value);
        //graphView.SetGridView(graph.xTicks)
    }


    private void OnRangeXChanged(Vector2 rangeX)
    {
        graphView.ResetX(rangeX, graph.XTicks, graph.XTickInterval, graph.axisX, graph.overflowType);
    }

    private void OnRangeYChanged(Vector2 rangeY)
    {
        graphView.ResetY(rangeY, graph.YTicks, graph.YTickInterval, graph.axisY);
    }

    private void OnDrag(PointerEventData eventData)
    {
        //Debug.Log($"drag: {eventData.delta}");
        Vector2 _pivotDelta = eventData.delta / graphView.RectTransform.rect.size;
        if (Mathf.Abs(_pivotDelta.x) > float.Epsilon)
            graph.DragX(_pivotDelta.x);

        if (Mathf.Abs(_pivotDelta.y) > float.Epsilon)
            graph.DragY(_pivotDelta.y);
    }

    private void OnScroll(PointerEventData eventData)
    {
        //Debug.Log($"scroll: {eventData.scrollDelta}");
        Vector2 _rtSize = graphView.RectTransform.rect.size;

        Vector2 _mousePosition = eventData.position;
        Vector2 _scrollDelta = eventData.scrollDelta;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(graphView.RectTransform, _mousePosition, graphView.Cam, out Vector2 _rectPoint);
        Vector2 _scrollPivot = _rectPoint / _rtSize; // -0.5 ~ 0.5

        //Debug.Log($"mousePoint:{_mousePosition}, rectPoint:{_rectPoint}, scrollPivot:{_scrollPivot}");

        //INFO::update zoom; by allen;
        if (Mathf.Abs(_scrollDelta.y) > float.Epsilon && Input.GetKey(KeyCode.LeftControl)) //INFO::zoom x,y
        {
            graph.ScrollX(_scrollDelta.y, _scrollPivot.x);
            graph.ScrollY(_scrollDelta.y, _scrollPivot.y);
        }
        else if (Mathf.Abs(_scrollDelta.y) > float.Epsilon && Input.GetKey(KeyCode.LeftShift)) //INFO::zomm y only
            graph.ScrollY(_scrollDelta.y, _scrollPivot.y);
        else if (Mathf.Abs(_scrollDelta.y) > float.Epsilon) //INFO::zoom x only
            graph.ScrollX(_scrollDelta.y, _scrollPivot.x);
        else if (Mathf.Abs(_scrollDelta.x) > float.Epsilon) //INFO::zoom y only by side scroll
            graph.ScrollY(_scrollDelta.x, _scrollPivot.y);
    }
}
