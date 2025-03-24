using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphAxis
{
    public string name; //EXAMPLE::angular velocity, angle, time
    public string unit; //EXAMPLE::¡Æ, ¡Æ/sec, sec
    //public Vector2 range; // INFO::graphModel.domain;
    public string positivePrefix; //EXAMPLE::R
    public string positivePostfix; //
    public string negativePrefix; //EXAMPLE::L
    public string negativePostfix; //
    public string normalPrefix;
    public string normalPostfix;
    public Color positiveColor;
    public Color negativeColor;
    public Color normalColor;

    public GraphAxis(string name, string unit, string positivePrefix, string positivePostfix, string negativePrefix, string negativePostfix, string normalPrefix, string normalPostfix, Color positiveColor, Color negativeColor, Color normalColor)
    {
        this.name = name;
        this.unit = unit;
        this.positivePrefix = positivePrefix;
        this.positivePostfix = positivePostfix;
        this.negativePrefix = negativePrefix;
        this.negativePostfix = negativePostfix;
        this.normalPrefix = normalPrefix;
        this.normalPostfix = normalPostfix;
        this.positiveColor = positiveColor;
        this.negativeColor = negativeColor;
        this.normalColor = normalColor;
    }
    public GraphAxis() : this(name: "", unit: "", positivePrefix: "R", positivePostfix: "", negativePrefix: "L", negativePostfix: "", normalPrefix: "", normalPostfix: "", positiveColor: Color.red, negativeColor: Color.blue, normalColor: Color.black) { }
}
