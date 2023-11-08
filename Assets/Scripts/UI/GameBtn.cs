using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameBtn : UIEventTrigger
{
    ScrollRect Scroll;

    void Start()
    {
        init();

        Scroll = transform.parent.parent.parent.GetComponent<ScrollRect>();
    }

    protected override void OnPointerClick(PointerEventData data)
    {
        if (transform.GetChild(0).GetComponent<TMP_Text>().text.Equals("Word Hunting"))
        {
            SceneManager.LoadScene("WordHunting");
        }
        else if (transform.GetChild(0).GetComponent<TMP_Text>().text.Equals("Hangul Run"))
        {
            SceneManager.LoadScene("HangulRun");
        }
    }


    protected override void OnBeginDrag(PointerEventData data)
    {
        Scroll.OnBeginDrag(data);
    }
    protected override void OnDrag(PointerEventData data)
    {
        Scroll.OnDrag(data);
    }
    protected override void OnEndDrag(PointerEventData data)
    {
        Scroll.OnEndDrag(data);
    }
}
