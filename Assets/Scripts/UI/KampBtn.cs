using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KampBtn : UIEventTrigger
{
    ScrollRect Scroll;
    TMP_Text KampNameTxt, PlayerCountTxt;

    public string KampName;
    public int PlayerCount;
    public int MaxCount;

    public void Set(string name, int cnt, int max)
    {
        KampName = name;
        PlayerCount = cnt;
        MaxCount = max;
    }
    void Start()
    {
        init();

        Scroll = transform.parent.parent.parent.GetComponent<ScrollRect>();
        KampNameTxt = transform.GetChild(0).GetComponent<TMP_Text>();
        PlayerCountTxt = transform.GetChild(1).GetComponent<TMP_Text>();
    }

    private void Update()
    {
        KampNameTxt.text = KampName;
        PlayerCountTxt.text = "(" + PlayerCount + "/" + MaxCount + ")";
    }

    protected override void OnPointerDown(PointerEventData data)
    {
        GameManager.Instance.Netmanager.JoinKamp(KampName);
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
