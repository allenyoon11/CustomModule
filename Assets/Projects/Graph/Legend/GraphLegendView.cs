using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Toggle))]
public class GraphLegendView : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] TMP_Text label;
    [SerializeField] Image background;

    public Subject<bool> OnToggleChanged { get; private set; } = new Subject<bool>();
    public bool IsOn => toggle.isOn;

    private void Start()
    {
        toggle.OnValueChangedAsObservable().Subscribe(isOn => OnToggleChanged.OnNext(isOn));
    }
    public void SetColor(Color color)
    {
        label.color = color;
        background.color = color;
    }

    public void SetName(string name)
    {
        label.text = name;
    }

    public void SetIsOn(bool isOn, bool isNotify = false)
    {
        if (isNotify)
        {
            toggle.isOn = isOn;
        }
        else
        {
            toggle.SetIsOnWithoutNotify(isOn);
        }
    }

    public class Factory : PlaceholderFactory<GraphLegendModel, GraphLegendView> { }

}
