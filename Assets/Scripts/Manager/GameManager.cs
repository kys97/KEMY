using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public UIManager UImanager = null;
    [HideInInspector] public ResourcesManager Resourcesmanager = null;
    [HideInInspector] public DataClass Data = null;

    [HideInInspector] public List<ItemWeight> ItemWeightDic = null;
    [HideInInspector] public int TotWeight = 0;

    JsonManager jsonmanager;

    private ui past_ui = ui.Home;
    [SerializeField] private ui top_ui;
    public ui TopUI
    {
        get
        {
            return top_ui;
        }
        set
        {
            top_ui = value;
            Debug.Log(top_ui.ToString());
            switch (value)
            {
                case ui.Home: HomeCoroutine(); break;
                case ui.Inventory: InvenCoroutine(); break;
                case ui.QuizReady: QuizReadyCharacterSet(); break;
                case ui.QuizStart: QuizStartCharacterSet(); break;
                case ui.QuizResult: QuizEndCharacterSet(); break;
            }
        }
    }

    #region ĳ���� �� �ƹ�Ÿ ����
    [SerializeField] int HomeAvatarSize = 3;//3
    [SerializeField] int InvenAvatarSize = 2;//2
    [SerializeField] int QuizReadyAvatarSize = 2;
    [SerializeField] float QuizGameAvatarSize = 1.5f;
    [SerializeField] int QuizEndAvatarSize = 1;
    [SerializeField] float AvatarSizeSpeed = 1;//1

    [SerializeField] Vector3 HomeCameraPos = new Vector3(0, 4.5f, -12);//0, 4.5, -12
    [SerializeField] Vector3 InvenCameraPos = new Vector3(0, 1, -14);//0, 1, -14
    [SerializeField] Vector3 QuizReadyCameraPos = new Vector3(0, 1.7f, -10);
    [SerializeField] Vector3 QuizGameCameraPos = new Vector3(0, 3.15f, -10);
    [SerializeField] Vector3 QuizEndCameraPos = new Vector3(0, 1.2f, -10);
    [SerializeField] float CameraMoveSpeed = 10;//10
    #endregion

    #region ȭ�� �̵� �ڷ�ƾ

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

        if (avatar == null)
        {
            avatar = GameObject.FindGameObjectWithTag("Character").transform;
        }

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch ((scene)System.Enum.Parse(typeof(scene), scene.name))
        {
            case Define.scene.Home: HomeSceneInit(); break;
            case Define.scene.Quiz: QuizSceneInit(); break;
        }
    }

    private void HomeSceneInit()
    {
        if(UImanager == null)
        {
            UImanager = GetComponent<UIManager>();
            UImanager.Init();
        }

        if (Resourcesmanager == null)
        {
            Resourcesmanager = GetComponent<ResourcesManager>();
            Resourcesmanager.Init();
        }

        if (Data == null)
           Data = new DataClass();
        
        if(jsonmanager == null)
            jsonmanager = new JsonManager();
        
        if(ItemWeightDic == null)
            ItemWeightDic = new CsvManager().Read_ItemWeight_Csv();


        Load();

        UImanager.UIsetting(ui_level.Lev1, past_ui);

        MyAvatar();
    }

    private void QuizSceneInit()
    {
        UImanager.UIsetting(ui_level.Lev1, ui.QuizReady);

        MyAvatar();
    }

    private void MyAvatar()
    {
        Transform parent = GameObject.FindGameObjectWithTag("Character").transform;

        switch (TopUI)
        {
            case ui.Home:
                parent.localScale = Vector3.one * HomeAvatarSize;
                Camera.main.transform.position = HomeCameraPos;
                break;
            case ui.QuizReady:
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

    public void SetPastUI()
    {
        past_ui = top_ui;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
