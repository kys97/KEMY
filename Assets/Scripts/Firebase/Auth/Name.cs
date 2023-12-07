using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Name : MonoBehaviour
{
    public void NameCheck()
    {
        GameManager.Instance.Data.info.name = GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetComponent<TMP_InputField>().text.ToString();
        GameManager.Instance.Data.info.login = true;
        GameManager.Instance.Save();
        SceneManager.LoadScene("Home");
    }
}
