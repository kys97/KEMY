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
        ItemWeight random_item = GetRandomItem();

        //Image Set
        ItemImage.sprite = GameManager.Instance.Resourcesmanager.ItemImage[random_item.name];

        //Inven에 저장
        switch (random_item.type)
        {
            case Define.item_type.Skin:
                GameManager.Instance.Data.inven.skin.Add(random_item.name);
                break;
            case Define.item_type.Face:
                GameManager.Instance.Data.inven.face.Add(random_item.name);
                break;
        }

        //데이터 파일 저장
        GameManager.Instance.Save();
    }

    ItemWeight GetRandomItem()
    {
        //Item 확률 계산
        int rand = Random.Range(0, GameManager.Instance.TotWeight);
        int temp = 0;

        //Item 결과 이미지 
        foreach (var value in GameManager.Instance.ItemWeightDic)
        {
            temp += value.weight;

            if (rand < temp)
                return value;
        }

        return GameManager.Instance.ItemWeightDic[0];
    }
}
