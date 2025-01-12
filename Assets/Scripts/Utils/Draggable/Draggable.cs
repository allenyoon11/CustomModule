using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public DraggableController controller;

    public void OnBeginDrag(PointerEventData eventData)
    {
        controller.BeginDrag(this);
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        controller.Drag(this);
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        controller.EndDrag(this);
    }
    public void SetTransparent(bool isOn)
    {
        var img = GetComponent<Image>();
        if (isOn)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0.5f);
        }
        else
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        }
    }
}
