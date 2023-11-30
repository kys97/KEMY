using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public UIManager UImanager;
    [HideInInspector] public ResourcesManager Resourcesmanager;
    [HideInInspector] public DataClass Data;
    
    [HideInInspector] public List<ItemWeight> ItemWeightDic;
    [HideInInspector] public int TotWeight = 0;

    JsonManager jsonmanager;

    #region 캐릭터 및 아바타 변수
    [SerializeField] int HomeAvatarSize = 3;//3
    [SerializeField] int InvenAvatarSize = 2;//2
    [SerializeField] int QuizReadyAvatarSize = 2;
    [SerializeField] float QuizGameAvatarSize = 1.5f;
    [SerializeField] int QuizEndAvatarSize = 1;
    [SerializeField] float AvatarSizeSpeed = 1;//1

    [SerializeField] Vector3 HomeCameraPos = new Vector3(0, 4.5f, -12);//0, 4.5, -12
    [SerializeField] Vector3 InvenCameraPos = new Vector3(0, 1, -14);//0, 1, -14
    [SerializeField] Vector3 QuizReadyCameraPos = new Vector3(0, 1.7f, - 10);
    [SerializeField] Vector3 QuizGameCameraPos = new Vector3(0, 3.15f, - 10);
    [SerializeField] Vector3 QuizEndCameraPos = new Vector3(0, 1.2f, - 10);
    [SerializeField] float CameraMoveSpeed = 10;//10


    [SerializeField]private Define.ui top_ui; 
    public Define.ui TopUI
    {
        set
        {
            top_ui = value;
            Debug.Log(top_ui.ToString());
            switch (value)
            {
                case Define.ui.Home : HomeCoroutine(); break;
                case Define.ui.Inventory: InvenCoroutine(); break;
                case Define.ui.QuizReady: QuizReadyCharacterSet(); break;
                case Define.ui.QuizStart: QuizStartCharacterSet(); break;
                case Define.ui.QuizResult: QuizEndCharacterSet(); break;
            }
        }
    }
    #endregion

    #region 화면 이동 코루틴
    #region Home Coroutine
    void HomeCoroutine()
    {
        StopAllCoroutines();
        StartCoroutine("HomeAvatarCoroutine");
        StartCoroutine("HomeCameraCoroutine");
    }
    IEnumerator HomeAvatarCoroutine()
    {
        Transform avatar = GameObject.FindGameObjectWithTag("Character").transform;

        while (avatar.localScale.x < HomeAvatarSize)
        {
            avatar.localScale += Vector3.one * HomeAvatarSize * Time.fixedDeltaTime * AvatarSizeSpeed;
            yield return new WaitForFixedUpdate();
        }
        
        yield return null;
    }
    IEnumerator HomeCameraCoroutine()
    {
        Transform camera = Camera.main.transform;
        while (camera.position != HomeCameraPos)
        {
            camera.position = Vector3.Lerp(camera.position, HomeCameraPos, Time.fixedDeltaTime * CameraMoveSpeed);
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }
    #endregion
    #region Inven Coroutine
    void InvenCoroutine()
    {
        StopAllCoroutines();
        StartCoroutine("InvenAvatarCoroutine");
        StartCoroutine("InvenCameraCoroutine");
    }
    IEnumerator InvenAvatarCoroutine()
    {
        Transform avatar = GameObject.FindGameObjectWithTag("Character").transform;
        while (avatar.localScale.x > InvenAvatarSize)
        {
            avatar.localScale -= Vector3.one * InvenAvatarSize * Time.fixedDeltaTime * AvatarSizeSpeed;
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }
    IEnumerator InvenCameraCoroutine()
    {
        Transform camera = Camera.main.transform;
        while (camera.position != InvenCameraPos)
        {
            camera.position = Vector3.Lerp(camera.position, InvenCameraPos, Time.fixedDeltaTime * CameraMoveSpeed);
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }
    #endregion
    void QuizReadyCharacterSet()
    {
        GameObject.FindGameObjectWithTag("Character").transform.localScale = Vector3.one * QuizReadyAvatarSize;
        Camera.main.transform.position = QuizReadyCameraPos;
    }
    void QuizStartCharacterSet()
    {
        GameObject.FindGameObjectWithTag("Character").transform.localScale = Vector3.one * QuizGameAvatarSize;
        Camera.main.transform.position = QuizGameCameraPos;
    }
    void QuizEndCharacterSet()
    {
        GameObject.FindGameObjectWithTag("Character").transform.localScale = Vector3.one * QuizEndAvatarSize;
        Camera.main.transform.position = QuizEndCameraPos;
    }
    #endregion

    #region Singleton
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Data
    public void Save()
    {
        jsonmanager.SaveJson(Data);
    }

    public void Load()
    {
        Data = jsonmanager.LoadSaveData();
    }
    #endregion

    public void Init()
    {
        UImanager = GetComponent<UIManager>();
        Resourcesmanager = GetComponent<ResourcesManager>();

        UImanager.Init();
        Resourcesmanager.Init();

        //로그인 확인
        //로그인 화면 Load
        //UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Login);

        //Home화면
        UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Main);

        //아바타 Setting
        MyAvatar();
    }

    public void MyAvatar()
    {
        Transform parent = GameObject.FindGameObjectWithTag("Character").transform;
        Debug.Log(top_ui);
        switch (top_ui)
        {
            case Define.ui.Home: 
                parent.localScale = Vector3.one * HomeAvatarSize;
                Camera.main.transform.position = HomeCameraPos;
                break;
            case Define.ui.QuizReady:
                parent.localScale = Vector3.one * QuizReadyAvatarSize;
                Camera.main.transform.position = QuizReadyCameraPos;
                break;
            default: break;
        }
        
        GameObject player = Instantiate(Resources.Load<GameObject>("Prefabs/Cat"), parent);
        Material[] skin = player.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().materials;
        skin[0] = Resourcesmanager.ItemMaterials[Data.avatar_info.skin];
        skin[1] = Resourcesmanager.ItemMaterials[Data.avatar_info.face];
        player.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().materials = skin;
    }

    void Start()
    {
        Data = new DataClass();
        jsonmanager = new JsonManager();
        ItemWeightDic = new CsvManager().Read_ItemWeight_Csv();

        Load();

        Init();

        UImanager.UIsetting(Define.ui_level.Lev2, Define.ui.Home);
        //데이터 불러오기
    }

    void Update()
    {
        
    }
}
