using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    List<GameObject> UILevels = new List<GameObject>();

    Dictionary<string, GameObject> UILayout;

    public void Init()
    {
        UILoad();
        for(int i = 0; i < UILevels.Count; i++)
        {
            UILevels.Add(GameObject.Find("UI Level"+i.ToString()));
        }
    }

    private void UILoad()
    {
        UILayout = new Dictionary<string, GameObject>();

        for (int i = 0; i < (int)Define.ui.Count; i++)
        {
            UILayout.Add(((Define.ui)i).ToString(), Resources.Load<GameObject>("UI/" + ((Define.ui)i).ToString()));
        }
    }

    public void UIsetting(Define.ui_level level, Define.ui ui)
    {
        //Prev UI Destroy
        Transform[] childList = UILevels[(int)level].GetComponentsInChildren<Transform>();
        if (childList != null)
        {
            for (int i = 1; i < childList.Length; i++)
            {
                if (childList[i] != UILevels[(int)level].transform) //UI Level 자체 삭제 제외
                    Destroy(childList[i].gameObject);
            }
        }

        //New UI Load
        Instantiate(UILayout[ui.ToString()], UILayout[ui.ToString()].transform.position, Quaternion.identity).transform.SetParent(UILevels[(int)level].transform, false);
    }

    public void Goto_Main()
    {
        SceneManager.LoadScene("Main");
    }

    public void Goto_KampScene()
    {
        SceneManager.LoadScene("Kamp");
    }
}
