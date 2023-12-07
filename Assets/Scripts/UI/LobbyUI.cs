using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    void Start()
    {
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(false);

        if (GameManager.Instance.Netmanager.lobby_list.Count == 0)
            transform.GetChild(2).gameObject.SetActive(true);
        else
            transform.GetChild(2).gameObject.SetActive(false);

        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(()=>GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.CreateKamp));
    }

    private void Update()
    {
        
    }
}
