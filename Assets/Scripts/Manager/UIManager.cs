using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Define;

public class UIManager : MonoBehaviour
{
    List<GameObject> UILevels = new List<GameObject>();

    Dictionary<string, GameObject> UILayout;

    public void Init()
    {
        UILoad();
        for(int i = 0; i < (int)ui_level.Count; i++)
        {
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
        GameManager.Instance.TopUI = ui;
        Instantiate(UILayout[ui.ToString()], UILayout[ui.ToString()].transform.position, Quaternion.identity).transform.SetParent(UILevels[(int)level].transform, false);
    }

    public void UIdelete(ui_level level)
    {
        if (GameObject.FindWithTag(level.ToString()).transform.childCount > 0)
            Destroy(GameObject.FindWithTag(level.ToString()).transform.GetChild(0).gameObject);
    }

    public void Goto_Main()
    {
        SceneManager.LoadScene("Main");
    }

    public void Goto_KampScene()
    {
        SceneManager.LoadScene("Kamp");
    }

    public void Goto_WHGame()
    {
        SceneManager.LoadScene("WHGame");
    }


    public void BottomInUI(GameObject UI)
    {
        StopCoroutine("BottomIn");
        StartCoroutine(BottomIn(UI));
    }
    public void BottomOutUI(GameObject UI)
    {
        StopCoroutine("BottomOut");
        StartCoroutine(BottomOut(UI));
    }
    public void RightInUI(GameObject UI)
    {
        StopCoroutine("RightIn");
        StartCoroutine(RightIn(UI));
    }


    IEnumerator BottomIn(GameObject ui)
    {
        for(int i=0; i < 10; i++)
        {
            
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
    IEnumerator BottomOut(GameObject ui)
    {
        RectTransform transform = ui.GetComponent<RectTransform>();
        float goal = transform.anchoredPosition.y - 190; //Bottom Size

        while (transform.anchoredPosition.y > goal) 
        {
            transform.anchoredPosition += new Vector2(0, goal) * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    IEnumerator RightIn(GameObject ui)
    {
        for (int i = 0; i < 10; i++)
        {

            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
    }
}
