using Shapes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Line))]
public class GraphGrid : MonoBehaviour
{
    [SerializeField]
    private Line line;

    public bool IsActivate
    {
        get{ return gameObject.activeInHierarchy; }
        set{ gameObject.SetActive(value);}
    }

    public Vector2 Start
    {
        get { return line.Start; }
        set { line.Start = value; }
    }
    public Vector2 End
    {
        get { return line.End; }
        set { line.End = value; }
    }

    public Color Color
    {
        get { return line.Color; }
        set { line.Color = value; }
    }

    public float Thickness
    {
        get { return line.Thickness; }
        set { line.Thickness = value; }
    }

    public bool IsDashed
    {
        get { return line.Dashed; }
        set { line.Dashed = value; }
    }

    public float DashOffset
    {
        get { return line.DashOffset; }
        set { line.DashOffset = value; }
    }

    public float DashSize
    {
        get { return line.DashSize; }
        set { line.DashSize = value; }
    }

    //[SerializeField]
    //private TMP_Text text;
    //public float TextValue
    //{
    //    set { text.text = $"{value:F1}"; }
    //}

    //public Color TextColor
    //{
    //    get { return text.color; }
    //    set { text.color = value; }
    //}

    //public Vector2 TextSize
    //{
    //    get { return text.rectTransform.sizeDelta; }
    //    set { text.rectTransform.sizeDelta = value; }
    //}

    //public Vector2 TextPosition
    //{
    //    get { return text.rectTransform.localPosition; }
    //    set { text.rectTransform.localPosition = value; }
    //}

    //public Vector2 TextPivot
    //{
    //    get { return text.rectTransform.pivot; }
    //    set { text.rectTransform.pivot = value; }
    //}

    //public TextAlignmentOptions TextAlign
    //{
    //    get { return text.alignment; }
    //    set { text.alignment = value; }
    //}

    public class Pool : MonoMemoryPool<GraphGrid>
    { }
}
