using System;
using UnityEngine;
using UnityEngine.UI;

public class BottomUI : MonoBehaviour
{
    void Start()
    {
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(()=>GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Home));
        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(()=>GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Lobby));
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(()=>GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Game));

        switch (GameManager.Instance.TopUI)
        {
            case Define.ui.Home:
                transform.GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.home_select.ToString()];
                transform.GetChild(1).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.chat.ToString()];
                transform.GetChild(2).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.book.ToString()];
                break;
            case Define.ui.Lobby:
                transform.GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.home.ToString()];
                transform.GetChild(1).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.chat_select.ToString()];
                transform.GetChild(2).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.book.ToString()];
                break;
            case Define.ui.Game:
                transform.GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.home.ToString()];
                transform.GetChild(1).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.chat.ToString()];
                transform.GetChild(2).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.book_select.ToString()];
                break;

        }
    }
}
