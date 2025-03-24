using Shapes;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Polyline))]
public class GraphLineView : MonoBehaviour
{
    public Color LineColor
    {
        get{ return polyline.Color; }
        set { polyline.Color = value; }
    }

    [SerializeField]
    private Polyline polyline;

    public List<PolylinePoint> Points
    {
        get { return polyline.points; }
        set { 
            polyline.points = value;
            polyline.meshOutOfDate = true;
        }
    }
    public void SetThickness(float thickness)
    {
        polyline.Thickness = thickness;
    }

    public class Factory : PlaceholderFactory<GraphLineModel, GraphLineView> { }
}
