using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : MonoBehaviour
{
    void Start()
    {
        //UI Size Set
        GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
        
        //Avatar Setting
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(true);

        //User Name(=NickName) Setting
        transform.GetChild(0).GetComponent<TMP_Text>().text = GameManager.Instance.Data.info.name;

        //Inven Btn Setting
        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(()=> GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Inventory));

        //Shop Btn Setting
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(OpenShopBtn);

        CreateLogoutButton();
    }


    void OpenShopBtn()
    {
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(false);
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Shop);
    }

    void CreateLogoutButton()
    {
        GameObject buttonObject = new GameObject("LogoutBtn", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(transform, false);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-35f, -35f);
        rect.sizeDelta = new Vector2(180f, 70f);
        buttonObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.35f);
        buttonObject.GetComponent<Button>().onClick.AddListener(GameManager.Instance.Logout);

        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(buttonObject.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = "로그아웃";
        text.fontSize = 28f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
    }
}
