using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KampUI : MonoBehaviour
{
    TMP_InputField message_inputfield;

    void Start()
    {
        //UI Size Set
        GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;

        //Kamp Set
        KampManager.Instance.SetKampText(transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>(),
            transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>());

        //Message Set
        KampManager.Instance.SetScroll(transform.GetChild(1).GetChild(0).GetComponent<ScrollRect>());
        KampManager.Instance.SetMessageParent(transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).transform);
        message_inputfield = transform.GetChild(1).GetChild(1).GetComponent<TMP_InputField>();
        message_inputfield.text = "";
        transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(SendMessage);

        //Message View Btn Set
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(MessageBtnClick);

        //Back to Home Btn Set
        transform.GetChild(4).GetComponent<Button>().onClick.AddListener(GameManager.Instance.UImanager.Goto_Home);
    }

    private void MessageBtnClick()
    {
        GameObject Message_Panel = transform.GetChild(1).gameObject;
        Message_Panel.SetActive(!Message_Panel.activeSelf);
    }


    void SendMessage()
    {
        if (!message_inputfield.text.Equals(""))
        {
            KampManager.Instance.SendBtnClick(message_inputfield.text);
            message_inputfield.text = "";
        }
    }

}
