using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetNameUI : MonoBehaviour
{
    void Start()
    {
        transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() =>
            SetName(transform.GetChild(3).GetComponent<TMP_InputField>().text));
    }

    void SetName(string name)
    {
        //Local Data Save & auto login
        GameManager.Instance.Data.info.login = true;
        GameManager.Instance.Data.info.name = name;
        GameManager.Instance.Save();

        //Home UI
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Home);
    }
}
