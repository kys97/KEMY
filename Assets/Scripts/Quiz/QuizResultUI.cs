using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizResultUI : MonoBehaviour
{
    

    void Start()
    {
        transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = QuizManager.Instance.Score.ToString();
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(GameManager.Instance.UImanager.Goto_Home);
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.QuizReady));

        //Result Set
        Transform result_parent = transform.GetChild(1).GetChild(0).GetChild(0).transform;
        GameObject result_prefab = Resources.Load<GameObject>("Prefabs/QuizResultPrefab");
        foreach(QuizManager.QuizResult quiz in QuizManager.Instance.ResultPull)
        {
            //New Prefab
            GameObject new_result = Instantiate(result_prefab, result_parent);

            //Set Background Image
            if (quiz.Result) 
                new_result.GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.quiz_answer.ToString()];
            else
                new_result.GetComponent<Image>().sprite = GameManager.Instance.Resourcesmanager.Sprites[Define.sprites.quiz_wrong.ToString()];

            //Set Text
            new_result.transform.GetChild(0).GetComponent<TMP_Text>().text = quiz.GetQuiz.Korean;
            new_result.transform.GetChild(1).GetComponent<TMP_Text>().text = quiz.GetQuiz.English;
        }
    }

}
