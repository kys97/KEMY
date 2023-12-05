using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Define;

public class GameBtn : UIEventTrigger
{
    [SerializeField] private scene scece_type;
    private ScrollRect Scroll;

    void Start()
    {
        init();

        Scroll = transform.parent.parent.parent.GetComponent<ScrollRect>();
    }

    protected override void OnPointerClick(PointerEventData data)
    {
        switch (scece_type)
        {
            case scene.Quiz:
                GameManager.Instance.UImanager.Goto_Quiz();
                break;
            case scene.RunningGame:
                //TODO
                SceneManager.LoadScene(scene.RunningGame.ToString());
                break;
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
