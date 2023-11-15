using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class CategoryTapBtn : UIEventTrigger
{
    [SerializeField] item_type type;

    GameObject skin_prefab;
    Transform parent;

    void Start()
    {
        init();

        parent = GameObject.Find("Content").gameObject.transform;
        skin_prefab = Resources.Load<GameObject>("Prefabs/Item").gameObject;

        //Init - Hair Set
        if (type != item_type.Skin) return;
        for (int i = 0; i < (int)skin.Count; i++)
        {
            //if (GameManager.Instance.Data.avatar_cloth.hair.Contains(((hair_item)(i)).ToString())) //인벤에 데이터 있는지 확인
            //{
                GameObject skin_temp = Instantiate(skin_prefab);
                skin_temp.transform.parent = parent;

                ItemSelectBtn temp;
                temp = skin_temp.GetComponent<ItemSelectBtn>();
                temp.type = item_type.Skin;
                temp.SetUI = ((skin)(i)).ToString();

                //TODO
            //}
        }
    }

    protected override void OnPointerDown(PointerEventData data)
    {
        int cnt = 0;
        GameObject[] skin_transforms = GameObject.FindGameObjectsWithTag("Item");

        //Max Count Set
        switch (type)
        {
            case item_type.Skin: cnt = (int)skin.Count; break;
            case item_type.Face: cnt = (int)face.Count; break;
            //case item_type.Head: cnt = (int)head.Count; break;
            //case item_type.Body: cnt = (int)body.Count; break;
        }


        //None Item Set
        //skin_transforms[0].GetComponent<ItemSelectBtn>().type = type;


        //Item Image Set
        for (int i = 0; i < cnt; i++)
        {
            ItemSelectBtn temp;
            
            if (i < skin_transforms.Length)
            {
                temp = skin_transforms[i].GetComponent<ItemSelectBtn>();
            }
            else
            {
                GameObject skin_temp = Instantiate(skin_prefab);
                skin_temp.transform.parent = parent;
                temp = skin_temp.GetComponent<ItemSelectBtn>();
            }

            temp.type = type;

            switch (type)
            {
                case item_type.Skin: temp.SetUI = ((skin)(i)).ToString(); break;
                case item_type.Face: temp.SetUI = ((face)(i)).ToString(); break;
                //case item_type.Head: temp.SetUI = ((head)(i)).ToString(); break;
                //case item_type.Body: temp.SetUI = ((body)(i)).ToString(); break;
            }
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
