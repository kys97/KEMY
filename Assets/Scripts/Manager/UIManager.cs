using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Define;

public class UIManager : MonoBehaviour
{
    List<GameObject> UILevels = new List<GameObject>();

    Dictionary<string, GameObject> UILayout = null;

    private float height;

    public void Init()
    {
        height = GameObject.Find("Canvas").GetComponent<CanvasScaler>().referenceResolution.y;

        if (UILayout == null)
            UILoad();

        UILevelLoad();
    }

    public void UILevelLoad()
    {
        UILevels.Clear();

        float new_width = (float)Screen.width * height / Screen.height;
        Vector2 full_screen = new Vector2(new_width, height);

        for (int i = 0; i < (int)ui_level.Count; i++)
        {
            GameObject.FindWithTag(((ui_level)i).ToString()).GetComponent<RectTransform>().sizeDelta = full_screen;
            UILevels.Add(GameObject.FindWithTag(((ui_level)i).ToString()));
        }
    }

    private void UILoad()
    {
        UILayout = new Dictionary<string, GameObject>();

        for (int i = 0; i < (int)ui.Count; i++)
        {
            UILayout.Add(((ui)i).ToString(), Resources.Load<GameObject>("UI/" + ((ui)i).ToString()));
        }
    }

    public void UIsetting(ui_level level, ui ui)
    {
        if (UILevels[(int)level] == null)
        {
            UILevelLoad();
        }

        //Prev UI Destroy
        Transform[] childList = UILevels[(int)level].GetComponentsInChildren<Transform>();
        if (childList != null)
        {
            for (int i = 1; i < childList.Length; i++)
            {
                if (childList[i] != UILevels[(int)level].transform) //UI Level 자체 삭제 제외
                {
                    Destroy(childList[i].gameObject);
                }
            }
        }

        //New UI Load
        GameManager.Instance.TopUI = ui;
        Instantiate(UILayout[ui.ToString()], UILayout[ui.ToString()].transform.position, Quaternion.identity).transform.SetParent(UILevels[(int)level].transform, false);
    }

    public void UIdelete(ui_level level)
    {
        if (GameObject.FindWithTag(level.ToString()).transform.childCount > 0)
            Destroy(GameObject.FindWithTag(level.ToString()).transform.GetChild(0).gameObject);

        switch(level)
        {
            case ui_level.Lev2:
                GameManager.Instance.TopUI = (ui)Enum.Parse(typeof(ui), GameObject.FindGameObjectWithTag("Lev1").transform.GetChild(0).name.Replace("(Clone)",""));
                break;
            case ui_level.Lev3:
                GameManager.Instance.TopUI = (ui)Enum.Parse(typeof(ui), GameObject.FindGameObjectWithTag("Lev2").transform.GetChild(0).name.Replace("(Clone)", ""));
                break;
        }
    }

    public void Goto_KampScene()
    {
        //GameManager.Instance.UImanager.UIdelete(ui_level.Lev2);
        GameManager.Instance.SetPastUI(ui.Lobby);
        SceneManager.LoadScene("Kamp");
    }

    public void Goto_Home()
    {
        SceneManager.LoadScene("Home");
    }

    public void Goto_Quiz()
    {
        GameManager.Instance.SetPastUI();
        SceneManager.LoadScene("Quiz");
    }
}
