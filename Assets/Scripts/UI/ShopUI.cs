using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public static string ItemResultName;


    [SerializeField] float shakeDistance; // ��鸮�� �Ÿ�
    [SerializeField] float shakeDuration; // ��鸮�� �ð�
    private Image EggImage;
    private Vector3 originalPosition;


    void Start()
    {
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(CloseShop);
        EggImage = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(EggOpen);

        //Egg Shaking
        originalPosition = EggImage.rectTransform.anchoredPosition;
    }

    void CloseShop()
    {
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Home);
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(true);
    }

    void EggOpen()
    {
        if (GameManager.Instance.Data.info.coin < 100)
            Debug.Log("������");
        else
        {
            GameManager.Instance.Data.info.coin -= 100;
            StartCoroutine("EggOpenCoroutine");
        }
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

        //TODO : �̹� ������ �������� ��� ���� ��Ʈ �߰� �̹��� ����
        ItemWeight random_item = GetRandomItem();
        ItemResultName = random_item.name;

        //Inven�� �ִ��� Ȯ�� �� ����
        switch (random_item.type)
        {
            case Define.item_type.none:
                GameManager.Instance.Data.info.heart += 20;
                break;
            case Define.item_type.Skin:
                if (!GameManager.Instance.Data.inven.skin.Contains(random_item.name))
                    GameManager.Instance.Data.inven.skin.Add(random_item.name);
                else
                {
                    GameManager.Instance.Data.info.coin += 10;
                    GameManager.Instance.Data.info.heart += 5;
                }
                break;
            case Define.item_type.Face:
                if (!GameManager.Instance.Data.inven.face.Contains(random_item.name))
                    GameManager.Instance.Data.inven.face.Add(random_item.name);
                else
                {
                    GameManager.Instance.Data.info.coin += 10;
                    GameManager.Instance.Data.info.heart += 5;
                }
                break;
        }

        //Panel Set
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.ShopResult);
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
