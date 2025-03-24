using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphAxisView : MonoBehaviour
{
    public TMP_Text textAxisX;
    public TMP_Text textAxisY;

    private Vector2 marginFactor = new Vector2(5, 2);
    private void Start()
    {
        var rectX = textAxisX.GetComponent<RectTransform>();
        var rectY = textAxisY.GetComponent<RectTransform>();
        StartCoroutine(UpdatePosition());
    }
    public void SetAxisXName(string name, string unit, Color color)
    {
        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(unit))
        {
            textAxisX.gameObject.SetActive(false);
        }
        else
        {
            textAxisX.gameObject.SetActive(true);
            textAxisX.text = $"{name} ({unit})";
            textAxisX.color = color;
        }
    }
    public void SetAxisYName(string name, string unit, Color color)
    {
        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(unit))
        {
            textAxisY.gameObject.SetActive(false);
        }
        else 
        {
            textAxisY.gameObject.SetActive(true);
            textAxisY.text = $"{name} ({unit})";
            textAxisY.color = color;
        }
    }

    IEnumerator UpdatePosition()
    {
        yield return new WaitForEndOfFrame();
        var rectX = textAxisX.GetComponent<RectTransform>();
        var rectY = textAxisY.GetComponent<RectTransform>();

        rectX.anchorMin = new Vector2(1, 0);
        rectX.anchorMax = new Vector2(1, 0);
        rectX.anchoredPosition = new Vector2(rectX.rect.width / 2 * -1 - marginFactor.x, rectX.rect.height / 2 + marginFactor.y);
        
        rectY.anchorMin = new Vector2(0, 1);
        rectY.anchorMax = new Vector2(0, 1);
        rectY.anchoredPosition = new Vector2(rectY.rect.width / 2 + marginFactor.x, rectY.rect.height / 2 * -1 - marginFactor.y);
    }

}
