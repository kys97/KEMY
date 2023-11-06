using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSelectBtn : UIEventTrigger
{
    public Define.item_type type;

    private ScrollRect Scroll;

    void Start()
    {
        Scroll = transform.parent.parent.parent.GetComponent<ScrollRect>();
    }

    public void SetUI()
    {
        //TODO
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
