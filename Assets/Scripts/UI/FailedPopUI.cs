using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FailedPopUI : MonoBehaviour
{
    void Start()
    {
        transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(()=>GameManager.Instance.UImanager.UIdelete(Define.ui_level.Lev3));
    }
}
