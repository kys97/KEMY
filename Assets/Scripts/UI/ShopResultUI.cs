using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopResultUI : MonoBehaviour
{
    void Start()
    {
        //UI Size Set
        GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;


        transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.ItemImage[ShopUI.ItemResultName];
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(CloseShopResult);
    }

    void CloseShopResult()
    {
        GameManager.Instance.UImanager.UIdelete(Define.ui_level.Lev2);
    }
}
