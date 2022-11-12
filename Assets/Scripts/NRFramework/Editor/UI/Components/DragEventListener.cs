using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragEventListener : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public event Action<PointerEventData> onBeginDrag;
    public event Action<PointerEventData> onDrag;
    public event Action<PointerEventData> onEndDrag;

    public static DragEventListener GetOrAdd(GameObject go)
    {
        var t = go.GetComponent<DragEventListener>();
        return t != null ? t : go.AddComponent<DragEventListener>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(eventData);
    }

    private void OnDestroy()
    {
        onBeginDrag = null;
        onDrag = null;
        onEndDrag = null;
    }
}