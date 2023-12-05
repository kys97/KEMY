using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizReadyUI : MonoBehaviour
{
    TMP_Text Level_txt;
    [SerializeField] string[] Level_color = new string[] { "#717171", "#798B9F", "#4A90E2" };

    void Start()
    {
        //Avatar Set
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(true);

        //Go to Main Btn
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(GameManager.Instance.UImanager.Goto_Home);

        //Level Text
        Level_txt = transform.GetChild(1).GetComponent<TMP_Text>();

        //Level Btn
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(LevelDownBtn);
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(LevelUpBtn);
        
        //Quiz Start Btn
        transform.GetChild(4).GetComponent<Button>().onClick.AddListener(QuizStartBtn);

    }

    void LevelDownBtn()
    {
        if(QuizManager.Instance.Level > 1)
        {
            QuizManager.Instance.Level --;
            SetTextColor();
        }
        //TODO : 난이도에 따른 애니메이션 넣기
    }

    void LevelUpBtn()
    {
        if (QuizManager.Instance.Level < QuizManager.Instance.MaxLevel)
        {
            QuizManager.Instance.Level++;
            SetTextColor();
        }
        //TODO : 난이도에 따른 애니메이션 넣기
    }

    void SetTextColor()
    {
        Level_txt.text = "<color=" + Level_color[QuizManager.Instance.Level - 1] + ">" + "Level " + QuizManager.Instance.Level.ToString() + "</color>";
    }

    void QuizStartBtn()
    {
        GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.QuizStart);
    }
}
