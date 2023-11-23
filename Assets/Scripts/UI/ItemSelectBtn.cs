using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class ItemSelectBtn : UIEventTrigger
{
    public Define.item_type type;
    protected string item_name;

    [SerializeField] Image item_image;
    [SerializeField] ScrollRect Scroll;
    Material[] skin;

    public string SetUI
    {
        set
        {
            if (item_image == null)
                item_image = transform.GetChild(0).GetComponent<Image>();
            item_name = value;
            item_image.sprite = GameManager.Instance.Resourcesmanager.ItemImage[value];
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
        switch (type)
        {
            case item_type.Skin:
                GameObject.FindGameObjectWithTag("Skin").GetComponent<SkinnedMeshRenderer>().material = GameManager.Instance.Resourcesmanager.ItemMaterials[item_name];
                GameManager.Instance.Data.avatar_info.skin = item_name;
                break;
            case item_type.Face:
                skin = GameObject.FindGameObjectWithTag("Skin").GetComponent<SkinnedMeshRenderer>().materials;
                skin[1] = GameManager.Instance.Resourcesmanager.ItemMaterials[item_name];
                GameObject.FindGameObjectWithTag("Skin").GetComponent<SkinnedMeshRenderer>().materials = skin;
                GameManager.Instance.Data.avatar_info.face = item_name;
                break;
        }
        
        GameManager.Instance.Save();
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
