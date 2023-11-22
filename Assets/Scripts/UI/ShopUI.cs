using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Dictionary<string,int> RandomItem;

    private int tot;
    private Image ItemEggImage, ItemImage;

    void Start()
    {
        transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(CloseShop);
        ItemEggImage = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
        transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(ItemEggOpen);

        ItemImage = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();

        
    }

    void CloseShop()
    {
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Home);
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("Lev1").transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
    }

    void ItemEggOpen()
    {
        //Item È®·ü °è»ê
        int rand = Random.Range(0, tot);

    }
}
