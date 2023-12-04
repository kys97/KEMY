using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizFinishUI : MonoBehaviour
{
    

    void Start()
    {
        //Score
        transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = QuizManager.Instance.Score.ToString();

        //Coin
        transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "+ " + (QuizManager.Instance.Score / 10).ToString();
        GameManager.Instance.Data.info.coin += QuizManager.Instance.Score / 10;
        //Heart
        transform.GetChild(1).GetChild(1).GetChild(0).GetComponent <TMP_Text>().text = "+ " + QuizManager.Instance.Heart.ToString();
        GameManager.Instance.Data.info.heart += QuizManager.Instance.Heart;

        GameManager.Instance.Save();

        //Main Btn
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(GameManager.Instance.UImanager.Goto_Home);
        
        //Replay Btn
        transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.QuizReady));

        //Result Set
        Transform result_parent = transform.GetChild(2).GetChild(0).GetChild(0).transform;
        GameObject result_prefab = Resources.Load<GameObject>("Prefabs/QuizResultPrefab");
        foreach(KeyValuePair<Quiz, bool> quiz in QuizManager.Instance.ResultPull)
        {
            //New Prefab
            GameObject new_result = Instantiate(result_prefab, result_parent);

            //Set Background Image
            if (quiz.Value) 
                new_result.GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.quiz_answer.ToString()];
            else
                new_result.GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.quiz_wrong.ToString()];

            //Set Text
            new_result.transform.GetChild(0).GetComponent<TMP_Text>().text = quiz.Key.Korean;
            new_result.transform.GetChild(1).GetComponent<TMP_Text>().text = quiz.Key.English;
        }
    }

}
