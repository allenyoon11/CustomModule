using Zenject;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Linq;
using Shapes;

public class GraphLinePresenter : IInitializable
{
    private GraphModel graph;
    private GraphView graphView;
    private GraphLineModel line;
    private GraphLineView lineView;

    public GraphLinePresenter(
        GraphModel graph,
        GraphView graphView,
        GraphLineModel line,
        GraphLineView lineView
        )
    {
        this.graph = graph;
        this.graphView = graphView;
        this.line = line;
        this.lineView = lineView;
    }

    public void Initialize()
    {
        SetSubscribes();
    }

    private void SetSubscribes()
    {
        //line.ColorRx.Subscribe(color => lineView.LineColor = color).AddTo(lineView);

        Observable.EveryUpdate()
            .Where(_ => line.IsChangedThisFrame)
            .Subscribe(_ =>
            {
                DrawLine();
            }).AddTo(lineView);
        //INFO::for scale line; by allen
        Observable.Merge(graph.RangeXRx, graph.RangeYRx)
            .Subscribe(_ => DrawLine()).AddTo(lineView);
        //INFO::for graph extend; by allen
        graphView.OnRectTransformDimensionsChangeAsObservable()
            .Subscribe(_ => DrawLine()).AddTo(lineView);

        //line.OnLineDataChanged
        //    //.ThrottleFrame(0)
        //    .Subscribe(
        //    polyPoints => {
        //        Vector2 _rangeX = graph.RangeXRx.Value;
        //        Vector2 _rangeY = graph.RangeYRx.Value;
        //        float _lengthX = _rangeX.y - _rangeX.x;
        //        float _lengthY = _rangeY.y - _rangeY.x;

        //        Rect _rect = graphView.RectTransform.rect;
        //        float _width = _rect.width;
        //        float _height = _rect.height;

        //        lineView.Points = polyPoints
        //        .Where(polyPoint => (_rangeX.x <= polyPoint.point.x && polyPoint.point.x <= _rangeX.y))
        //        .Select(
        //            polypoint =>
        //            {
        //                float _x = _width * Mathf.Clamp01((polypoint.point.x - _rangeX.x) / _lengthX);
        //                float _y = _height * Mathf.Clamp01((polypoint.point.y - _rangeY.x) / _lengthY);
        //                return new PolylinePoint(new Vector2(_x, _y), polypoint.color);
        //            }).ToList();
        //    }).AddTo(lineView);
    }
    private void DrawLine()
    {
        Vector2 _rangeX = graph.RangeXRx.Value;
        Vector2 _rangeY = graph.RangeYRx.Value;
        float _lengthX = _rangeX.y - _rangeX.x;
        float _lengthY = _rangeY.y - _rangeY.x;

        Rect _rect = graphView.RectTransform.rect;
        float _width = _rect.width;
        float _height = _rect.height;

        lineView.Points = line.ListLineData
        .Where(polyPoint => (_rangeX.x <= polyPoint.point.x && polyPoint.point.x <= _rangeX.y))
        .Select(
            polypoint =>
            {
                float _x = _width * Mathf.Clamp01((polypoint.point.x - _rangeX.x) / _lengthX);
                float _y = _height * Mathf.Clamp01((polypoint.point.y - _rangeY.x) / _lengthY);
                return new PolylinePoint(new Vector2(_x, _y), polypoint.color);
            }).ToList();

        line.IsChangedThisFrame = false;
    }

    //DEV::test reverse; by allen
    private void DrawLine(bool reverseX, bool reverseY)
    {
        int factorX = reverseX ? -1 : 1;
        int factorY = reverseY ? -1 : 1;

        Vector2 _rangeX = graph.RangeXRx.Value;
        Vector2 _rangeY = graph.RangeYRx.Value;
        float _lengthX = _rangeX.y - _rangeX.x;
        float _lengthY = _rangeY.y - _rangeY.x;

        Rect _rect = graphView.RectTransform.rect;
        float _width = _rect.width;
        float _height = _rect.height;

        lineView.Points = line.ListLineData
        .Where(polyPoint => (_rangeX.x <= polyPoint.point.x && polyPoint.point.x <= _rangeX.y))
        .Select(
            polypoint =>
            {
                float _x = _width * Mathf.Clamp01((polypoint.point.x - _rangeX.x) / _lengthX) * factorX;
                float _y = _height * Mathf.Clamp01((polypoint.point.y - _rangeY.x) / _lengthY) * factorY;
                return new PolylinePoint(new Vector2(_x, _y), polypoint.color);
            }).ToList();

        line.IsChangedThisFrame = false;
    }
}
