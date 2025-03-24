using Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;

public class GraphLineModel
{
    public StringReactiveProperty LegendRx = new("head");

    public BoolReactiveProperty IsActivateRx = new(true);

    public ColorReactiveProperty ColorRx = new(Color.black);
    public FloatReactiveProperty ThicknessRx = new(0.25f);

    public List<PolylinePoint> ListLineData = new();

    public Vector2 LastData;

    public bool IsChangedThisFrame = false;

    public IObservable<PolylinePoint[]> OnLineDataChanged => onLineDataChanged;

    private Subject<PolylinePoint[]> onLineDataChanged = new Subject<PolylinePoint[]>();

    public GraphLineModel(
        string name,
        Color color,
        float thickness
        )
    {
        LegendRx.Value = name;
        ColorRx.Value = color;
        ThicknessRx.Value = thickness;
    }

    public void NotifyLineChange()
    {
        onLineDataChanged.OnNext(ListLineData.ToArray());
    }
}
