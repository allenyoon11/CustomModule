using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Disc))]
public class GraphDotView : MonoBehaviour
{
    [SerializeField] Disc dot;

    public Color DotColor
    {
        get { return dot.Color; }
        set { dot.Color = value; }
    }
    public float Radius
    {
        get { return dot.Radius; }
        set { dot.Radius = value; }
    }
    public Vector2 Position
    {
        get { return GetComponent<RectTransform>().anchoredPosition; }
        set { GetComponent<RectTransform>().anchoredPosition = value; }
    }
    public bool Active
    {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }
    public class Factory : PlaceholderFactory<GraphDotModel, GraphDotView> { }
}
