using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenUI : MonoBehaviour
{
    void Start()
    {
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(()=> GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Home));
    }
}
