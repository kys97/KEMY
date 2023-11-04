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

        StartCoroutine(SpawnPrefab());
    }
    public void GameEnd()
    {
        GameState = Define.game_state.End;

        StopAllCoroutines();
        //���� ���
    }


    [SerializeField] private int StartCountDownNum;
    public Define.topic GameTopic;
    [SerializeField] private float SpawnInterval;
    private TMP_Text TopicText, CountDownText;

    private GameObject OODPrefab;

    // Start is called before the first frame update
    void Start()
    {
        TopicText = GameObject.Find("Topic").GetComponent<TMP_Text>();
        CountDownText = GameObject.Find("CountNum").GetComponent<TMP_Text>();
        OODPrefab = Resources.Load<GameObject>("Prefabs/OODPrefab");
        Init();
    }

    

    private void Init()
    {
        //���� ȭ�鿡 ����
        GameTopic = Define.topic.Animal;

        //ī��Ʈ �ٿ� �ڷ�ƾ
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

        //TODO : UI Manager ���
        GameObject.Find("CountPanel").gameObject.active = false;
        GameStart();
        yield return null;
    }


    
    private IEnumerator SpawnPrefab()
    {
        while (true)
        {
            // �������� ������ ����
            //int randomIndex = Random.Range(0, prefabs.Length);
            //GameObject selectedPrefab = prefabs[randomIndex];

            //�������� ��ġ ����
            Vector3 randomPosition = new Vector3(Random.Range(-350f, 350f), 50f, Random.Range(-350f, 350f));

            // ���õ� �������� ����
            GameObject newPrefabInstance = Instantiate(OODPrefab, randomPosition, Quaternion.identity);

            yield return new WaitForSeconds(SpawnInterval);
        }
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
