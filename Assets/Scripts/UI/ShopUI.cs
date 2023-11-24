using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Dictionary<string,int> RandomItem;

    [SerializeField] float shakeDistance; // ��鸮�� �Ÿ�
    [SerializeField] float shakeDuration; // ��鸮�� �ð�

    private int tot;
    private Image EggImage, ItemImage;
    private Vector3 originalPosition;

    void Start()
    {
        //Shop Panel
        transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(CloseShop);
        EggImage = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
        transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(EggOpen);

        originalPosition = EggImage.rectTransform.anchoredPosition;

        //Item Result Panel
        ItemImage = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
        transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(EggClose);
    }

    void CloseShop()
    {
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Home);
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("Lev1").transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
    }

    void EggClose()
    {
        //Panel Set
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    void EggOpen()
    {
        StartCoroutine("EggOpenCoroutine");
    }

    IEnumerator EggOpenCoroutine()
    {
        float elapsedTime;
        float currentShakeDistance = shakeDistance;
        Vector3 newPosition;

        for (int i=0; i < 3; i++)
        {
            elapsedTime = 0f;
            EggImage.sprite = GameManager.Instance.Resourcesmanager.Sprites["cracked" + (i + 1).ToString()];

            while (elapsedTime < shakeDuration)
            {
                // �¿�� �Դٰ��� ����
                float newX = originalPosition.x + Mathf.Sin(elapsedTime * 2 * Mathf.PI) * currentShakeDistance;
                newPosition = new Vector3(newX, originalPosition.y, originalPosition.z);
                EggImage.rectTransform.anchoredPosition = newPosition;

                // ��� �ð� ����
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // ��鸰 �Ŀ� �ʱ� ��ġ�� ���ư���
            EggImage.rectTransform.anchoredPosition = originalPosition;
        }

        SetRandomItem();
        yield return null;
    }

    void SetRandomItem()
    {
        StopCoroutine("EggOpenCoroutine");
        
        ItemWeight random_item = GetRandomItem();

        //Image Set
        ItemImage.sprite = GameManager.Instance.Resourcesmanager.ItemImage[random_item.name];
        
        
        //Inven�� ����
        switch (random_item.type)
        {
            case Define.item_type.none:
                GameManager.Instance.Data.info.heart += 10;
                GameObject.FindGameObjectWithTag("Lev1").transform.GetChild(0).GetComponent<MainUI>().UpdateCoin();
                break;
            case Define.item_type.Skin:
                GameManager.Instance.Data.inven.skin.Add(random_item.name);
                break;
            case Define.item_type.Face:
                GameManager.Instance.Data.inven.face.Add(random_item.name);
                break;
        }

        //Panel Set
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        EggImage.sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.item_egg.ToString()];

        //������ ���� ����
        GameManager.Instance.Save();
    }

    ItemWeight GetRandomItem()
    {
        //Item Ȯ�� ���
        int rand = Random.Range(0, GameManager.Instance.TotWeight);
        int temp = 0;

        //Item ��� �̹��� 
        foreach (var value in GameManager.Instance.ItemWeightDic)
        {
            temp += value.weight;

            if (rand < temp)
                return value;
        }

        return GameManager.Instance.ItemWeightDic[0];
    }
}
