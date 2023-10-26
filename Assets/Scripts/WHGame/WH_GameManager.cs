using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WH_GameManager : MonoBehaviour
{
    private static WH_GameManager instance;
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

    [SerializeField] private Define.game_state GameState;



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
