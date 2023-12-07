using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KampBtn : UIEventTrigger
{
    ScrollRect Scroll;

    public string KampName;
    public int PlayerCount;
    public int MaxCount;

    void Start()
    {
        init();

        Scroll = transform.parent.parent.parent.GetComponent<ScrollRect>();
    }

    protected override void OnPointerDown(PointerEventData data)
    {
        GameManager.Instance.Netmanager.JoinKamp(KampName);
    }

    protected override void OnBeginDrag(PointerEventData data)
    {
        Scroll.OnBeginDrag(data);
    }
    protected override void OnDrag(PointerEventData data)
    {
        Scroll.OnDrag(data);
    }
    protected override void OnEndDrag(PointerEventData data)
    {
        Scroll.OnEndDrag(data);
    }
}
