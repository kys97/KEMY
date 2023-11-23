using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    void Start()
    {
        GameObject.FindGameObjectWithTag("Bottom").transform.GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.home.ToString()];
        GameObject.FindGameObjectWithTag("Bottom").transform.GetChild(1).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.chat.ToString()];
        GameObject.FindGameObjectWithTag("Bottom").transform.GetChild(2).GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.book_select.ToString()];

        //Avatar Setting
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(false);
    }

    void Update()
    {
        
    }
}
