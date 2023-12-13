using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TopUI : MonoBehaviour
{
    TMP_Text coin_txt, heart_txt;

    void Start()
    {

        coin_txt = transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>();
        heart_txt = transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>();
    }

    private void Update()
    {
        coin_txt.text = GameManager.Instance.Data.info.coin.ToString();
        heart_txt.text = GameManager.Instance.Data.info.heart.ToString();
    }
}
