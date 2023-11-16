using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    void Start()
    {
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(CloseShop);
    }

    void Update()
    {
        
    }

    void CloseShop()
    {
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Home);
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("Lev1").transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
    }
}
