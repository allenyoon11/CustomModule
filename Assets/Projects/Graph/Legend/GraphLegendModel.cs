using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GraphLegendModel
{
    public StringReactiveProperty LegendRx = new();
    public ColorReactiveProperty ColorRx = new();
    public BoolReactiveProperty IsOnRx = new();

    public Subject<bool> OnToggleChanged { get; private set; } = new Subject<bool>(); //INFO::graphModel 로부터 호출되어 뷰만 변경하는 목적; by allen
    public GraphLegendModel(string name, Color color, bool isOn)
    {
        this.LegendRx.Value = name;
        this.ColorRx.Value = color;
        this.IsOnRx.Value = isOn;
    }
}
