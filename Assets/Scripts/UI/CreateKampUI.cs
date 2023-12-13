using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateKampUI : MonoBehaviour
{
    TMP_InputField name_txt;
    TMP_Text max_txt;

    int cnt;

    void Start()
    {
        GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
        cnt = 1;

        name_txt = transform.GetChild(1).GetComponent<TMP_InputField>();
        max_txt = transform.GetChild(2).GetComponent<TMP_Text>();
        max_txt.text = cnt.ToString();

        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(DownMaxCount);
        transform.GetChild(4).GetComponent<Button>().onClick.AddListener(UpMaxCount);

        transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.UImanager.UIdelete(Define.ui_level.Lev2));
        transform.GetChild(6).GetComponent<Button>().onClick.AddListener(CreateKamp);
    }

    void DownMaxCount()
    {
        if(cnt > 1)
        {
            cnt--;
            max_txt.text = cnt.ToString();
        }
    }

    void UpMaxCount()
    {
        if(cnt < 20)
        {
            cnt++;
            max_txt.text = cnt.ToString();
        }
    }

    void CreateKamp()
    {
        if(GameManager.Instance.Data.info.heart < 100)
        {
            GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev3, Define.ui.NoHeartPop);
        }
        else
        {
            if (GameManager.Instance.Netmanager.lobby_name_list.Contains(name_txt.text))
                FailedCreateKamp();
            else
            {
                GameManager.Instance.Data.info.heart -= 100;
                GameManager.Instance.Save();
                GameManager.Instance.Netmanager.CreateKamp(name_txt.text, cnt);
            }
        }
    }

    private void FailedCreateKamp()
    {
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev3, Define.ui.FailedPop);
    }
}
