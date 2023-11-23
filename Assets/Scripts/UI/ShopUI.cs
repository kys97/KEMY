using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Dictionary<string,int> RandomItem;

    private int tot;
    private Image EggImage, ItemImage;

    void Start()
    {
        //Shop Panel
        transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(CloseShop);
        EggImage = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
        transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(ItemEggOpen);

        //Item Result Panel
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
        //Item 확률 계산
        int rand = Random.Range(0, GameManager.Instance.Resourcesmanager.ItemImage.Count);

        //Item 결과 이미지 

        //Inven에 저장
        
    }
}
