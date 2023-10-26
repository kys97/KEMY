using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WH_GameManager : MonoBehaviour
{
    public static WH_GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WH_GameManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(WH_GameManager).Name);
                    instance = singletonObject.AddComponent<WH_GameManager>();
                }
            }
            return instance;
        }
    }
    private static WH_GameManager instance;
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
    


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        
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
