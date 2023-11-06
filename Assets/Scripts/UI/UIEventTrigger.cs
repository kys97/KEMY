using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventTrigger : MonoBehaviour
{
    EventTrigger eventTrigger;

    protected void init()
    {
        eventTrigger = gameObject.GetComponent<EventTrigger>();

        EventTrigger.Entry entry_PointerDown = new EventTrigger.Entry();
        entry_PointerDown.eventID = EventTriggerType.PointerDown;
        entry_PointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerDown);

        EventTrigger.Entry entry_Drag = new EventTrigger.Entry();
        entry_Drag.eventID = EventTriggerType.Drag;
        entry_Drag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_Drag);

        EventTrigger.Entry entry_EndDrag = new EventTrigger.Entry();
        entry_EndDrag.eventID = EventTriggerType.EndDrag;
        entry_EndDrag.callback.AddListener((data) => { OnEndDrag((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_EndDrag);

        EventTrigger.Entry entry_Click = new EventTrigger.Entry();
        entry_Click.eventID = EventTriggerType.PointerClick;
        entry_Click.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_Click);

        EventTrigger.Entry entry_BeginDrag = new EventTrigger.Entry();
        entry_BeginDrag.eventID = EventTriggerType.BeginDrag;
        entry_BeginDrag.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_BeginDrag);

    }
    protected virtual void OnPointerDown(PointerEventData data) { }
    protected virtual void OnDrag(PointerEventData data) { }
    protected virtual void OnEndDrag(PointerEventData data) { }
    protected virtual void OnPointerClick(PointerEventData data) { }
    protected virtual void OnBeginDrag(PointerEventData data) { }
}
