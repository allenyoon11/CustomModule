using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GraphDotModel
{
    public IntReactiveProperty NoRx = new(0);
    public StringReactiveProperty LegendRx = new("dot-0");
    public ColorReactiveProperty ColorRx = new(Color.red);
    public FloatReactiveProperty RadiusRx = new(0.2f);
    public Vector2ReactiveProperty PositionRx = new(Vector2.zero);
    public BoolReactiveProperty ActiveRx = new(true);

    public GraphDotModel(int no, string name, Color color, float radius, Vector2 pos)
    {
        NoRx.Value = no;
        LegendRx.Value = name;
        ColorRx.Value = color;
        RadiusRx.Value = radius;
        PositionRx.Value = pos;
        ActiveRx.Value = true;
    }
}
