using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KampUI : MonoBehaviour
{
    void Start()
    {
        Screen.SetResolution(2340 * (Screen.height / Screen.width), 2340, true);

        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(GameManager.Instance.UImanager.Goto_Home);
    }
    
    public void NewPlayer()
    {
        transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "(<color=#4A90E2>" + KampManager.PlayerCount + "</color>/" + NetworkManager.MaxCount + ")";
    }
}
