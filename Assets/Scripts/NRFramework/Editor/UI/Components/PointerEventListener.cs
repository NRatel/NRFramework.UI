using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerEventListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IInitializePotentialDragHandler
{
    public event Action<PointerEventData> onInitializePotentialDrag;
    public event Action<PointerEventData> onPointerClick;
    public event Action<PointerEventData> onPointerDown;
    public event Action<PointerEventData> onPointerEnter;
    public event Action<PointerEventData> onPointerExit;
    public event Action<PointerEventData> onPointerUp;

    public static PointerEventListener GetOrAdd(GameObject go)
    {
        var t = go.GetComponent<PointerEventListener>();
        return t != null ? t : go.AddComponent<PointerEventListener>();
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        onInitializePotentialDrag?.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onPointerClick?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke(eventData);
    }

    private void OnDestroy()
    {
        onInitializePotentialDrag = null;
        onPointerClick = null;
        onPointerDown = null;
        onPointerEnter = null;
        onPointerExit = null;
        onPointerUp = null;
    }
}
