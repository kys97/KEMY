using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSelectBtn : UIEventTrigger
{
    public Define.item_type type;
    protected string item_name;

    [SerializeField] Image item_image;
    [SerializeField] ScrollRect Scroll;

    public string SetUI
    {
        set
        {
            if (item_image == null)
                item_image = transform.GetChild(0).GetComponent<Image>();
            item_name = value;

            if (value.Equals(""))
            {
                //None Item Set
            }
            else
            {
                item_image.sprite = GameManager.Instance.Resourcesmanager.Item[value];
            }
        }
    }

    void Start()
    {
        init();

        Scroll = transform.parent.parent.parent.GetComponent<ScrollRect>();
        item_image = transform.GetChild(0).GetComponent<Image>();
    }

    protected override void OnPointerClick(PointerEventData data)
    {
        //아바타 옷 바꾸기
        Debug.Log(item_name);
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
