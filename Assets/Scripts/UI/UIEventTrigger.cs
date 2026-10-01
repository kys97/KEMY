using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventTrigger : MonoBehaviour, IInitializePotentialDragHandler, IPointerDownHandler, IPointerUpHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler, IPointerClickHandler
{
    bool initialized;
    protected void init() { initialized = true; }
    void EnsureInitialized() { if (!initialized) init(); }

    void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData data) { EnsureInitialized(); OnInitializePotentialDrag(data); }
    void IPointerDownHandler.OnPointerDown(PointerEventData data) { EnsureInitialized(); OnPointerDown(data); }
    void IPointerUpHandler.OnPointerUp(PointerEventData data) { EnsureInitialized(); OnPointerUp(data); }
    void IBeginDragHandler.OnBeginDrag(PointerEventData data) { EnsureInitialized(); OnBeginDrag(data); }
    void IDragHandler.OnDrag(PointerEventData data) { EnsureInitialized(); OnDrag(data); }
    void IEndDragHandler.OnEndDrag(PointerEventData data) { EnsureInitialized(); OnEndDrag(data); }
    void IScrollHandler.OnScroll(PointerEventData data) { EnsureInitialized(); OnScroll(data); }
    void IPointerClickHandler.OnPointerClick(PointerEventData data) { EnsureInitialized(); OnPointerClick(data); }

    protected virtual void OnInitializePotentialDrag(PointerEventData data) { }
    protected virtual void OnPointerDown(PointerEventData data) { }
    protected virtual void OnPointerUp(PointerEventData data) { }
    protected virtual void OnBeginDrag(PointerEventData data) { }
    protected virtual void OnDrag(PointerEventData data) { }
    protected virtual void OnEndDrag(PointerEventData data) { }
    protected virtual void OnScroll(PointerEventData data) { }
    protected virtual void OnPointerClick(PointerEventData data) { }
}
