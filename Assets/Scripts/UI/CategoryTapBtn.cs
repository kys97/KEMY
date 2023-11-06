using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CategoryTapBtn : UIEventTrigger
{
    [SerializeField] Define.item_type type;

    GameObject skin_prefab;
    Transform parent;

    void Start()
    {
        parent = GameObject.Find("Content").gameObject.transform;

        //Init - Hair Set
        if (type != Define.item_type.Hair) return;

        for (int i = 0; i < (int)Define.hair_item.Count; i++)
        {
            GameObject skin_temp = Instantiate(skin_prefab);
            skin_temp.transform.parent = parent;
            ItemSelectBtn temp;
            temp = skin_temp.AddComponent<ItemSelectBtn>();
            temp.type = Define.item_type.Hair;

            //TODO
        }
    }

    protected override void OnPointerDown(PointerEventData data)
    {
        int cnt = 0;
        Transform[] skin_transforms = parent.transform.GetComponentsInChildren<Transform>();

        //Max Count Set
        switch (type)
        {
            case Define.item_type.Hair: cnt = (int)Define.hair_item.Count + 1; break;
            case Define.item_type.Top: cnt = (int)Define.top_item.Count + 1; break;
            case Define.item_type.Bottom: cnt = (int)Define.bottom_item.Count + 1; break;
            case Define.item_type.Shoes: cnt = (int)Define.shoes_item.Count + 1; break;
        }


        //None Item Set
        skin_transforms[0].GetComponent<ItemSelectBtn>().type = type;


        //Item Image Set
        for (int i = 1; i < cnt; i++)
        {
            ItemSelectBtn temp;

            switch (type)
            {
                case Define.item_type.Hair: 
                    skin_transforms[i].GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Item[((Define.hair_item)(i - 1)).ToString()]; 
                    break;
                case Define.item_type.Top:
                    skin_transforms[i].GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Item[((Define.top_item)(i - 1)).ToString()]; 
                    break;
                case Define.item_type.Bottom:
                    skin_transforms[i].GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Item[((Define.bottom_item)(i - 1)).ToString()];
                    break;
                case Define.item_type.Shoes: 
                    skin_transforms[i].GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Item[((Define.shoes_item)(i - 1)).ToString()];
                    break;
            }
            
            if (i < skin_transforms.Length)
            {
                temp = skin_transforms[i].GetComponent<ItemSelectBtn>();
            }
            else
            {
                GameObject skin_temp = Instantiate(skin_prefab);
                skin_temp.transform.parent = parent;
                temp = skin_temp.AddComponent<ItemSelectBtn>();
            }

            temp.type = type;
        }

        if (cnt < skin_transforms.Length)
        {
            for (int i = cnt; i < skin_transforms.Length; i++)
            {
                Destroy(skin_transforms[i]);
            }
        }
    }
}
