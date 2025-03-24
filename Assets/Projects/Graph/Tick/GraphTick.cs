using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(TMP_Text))]
public class GraphTick : MonoBehaviour
{
    private string preFix;
    private string postFix;

    [SerializeField]
    private RectTransform rectTransform;
    [SerializeField]
    private TMP_Text text;

    //private float value;

    //[Inject]
    //public void Construct()
    //{
    //    SetValue(0);
    //}
    public void SetActive(bool value) => gameObject.SetActive(value);
    public void SetLocalPosition(Vector2 localPosition) => rectTransform.localPosition = localPosition;
    public void SetColor(Color color) => text.color = color;
    public void SetPreFix(string preFix) => this.preFix = preFix;
    public void SetPostFix(string postFix) => this.postFix = postFix;

    public HorizontalAlignmentOptions HorizontalAlign
    {
        get { return text.horizontalAlignment; }
        set { text.horizontalAlignment = value; }
    }

    public VerticalAlignmentOptions VerticalAlgin
    {
        get { return text.verticalAlignment; }
        set { text.verticalAlignment = value; }
    }

    public Vector2 Pivot
    {
        get { return rectTransform.pivot; }
        set { rectTransform.pivot = value; }
    }

    public void SetValue(float value, int digit = 3, string format="F1")
    {
        float _camp = value % Mathf.Pow(10, digit);
        string formattedValueStr = string.Format("{0:" + format + "}", _camp); //INFO::Æ÷¸Ë ÁöÁ¤; fix by allen
        text.text = $"{preFix}{formattedValueStr}{postFix}";
    }

    public class Pool: MonoMemoryPool<GraphTick>
    {
        //protected override void Reinitialize(GraphTick item)
        //{
        //    base.Reinitialize(item);
        //}
    }
}
