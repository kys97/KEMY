using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OODGameManager : MonoBehaviour
{
    public static OODGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<OODGameManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(OODGameManager).Name);
                    instance = singletonObject.AddComponent<OODGameManager>();
                }
            }
            return instance;
        }
    }
    private static OODGameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        GameState = Define.game_state.Ready;
    }


    public Define.game_state GetGameState() { return GameState; }
    private Define.game_state GameState;

    public void GameStart()
    {
        GameState = Define.game_state.Start;
    }
    public void GameEnd()
    {
        GameState = Define.game_state.End;
        //게임 결과
    }


    [SerializeField] private int StartCountDownNum;
    private TMP_Text TopicText, CountDownText;


    // Start is called before the first frame update
    void Start()
    {
        TopicText = GameObject.Find("Topic").GetComponent<TMP_Text>();
        CountDownText = GameObject.Find("CountNum").GetComponent<TMP_Text>();

        Init();
    }

    

    private void Init()
    {
        //주제 화면에 띄우기

        //카운트 다운 코루틴
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        int cnt = StartCountDownNum;

        while (cnt > 0)
        {
            CountDownText.text = cnt.ToString();
            yield return new WaitForSeconds(1f); 
            cnt--;
        }

        //TODO : UI Manager 사용
        GameObject.Find("CountPanel").gameObject.active = false;
        GameStart();
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
