using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Define;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public NetworkManager Netmanager;
    [HideInInspector] public UIManager UImanager = null;
    [HideInInspector] public ResourcesManager Resourcesmanager = null;
    [HideInInspector] public DataClass Data = null;
    public FirebaseAccountService AccountService { get; private set; }
    public string AuthenticationMessage { get; private set; }


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
            Debug.Log("TopUI : " + top_ui);
            switch (value)
            {
                case ui.Home: HomeCoroutine(); break;
                case ui.Inventory: InvenCoroutine(); break;
                case ui.Lobby: GetComponent<NetworkManager>().ChatOn(); break;
                case ui.QuizReady: QuizReadyCharacterSet(); break;
                case ui.QuizStart: QuizStartCharacterSet(); break;
                case ui.QuizFinish: QuizFinishCharacterSet(); break;
                default: break;
            }
        }
    }

    #region 캐릭터 및 아바타 변수
    [Space(10)]
    [SerializeField] int HomeAvatarSize = 3;//3
    [SerializeField] int InvenAvatarSize = 2;//2
    [SerializeField] int QuizReadyAvatarSize = 2;
    [SerializeField] float QuizGameAvatarSize = 1.5f;
    [SerializeField] int QuizFinishAvatarSize = 1;
    [SerializeField] float AvatarSizeSpeed = 1;//1
    [Space(10)]
    [SerializeField] Vector3 OriginAvatarPos = new Vector3 (0, 0, -1);
    [SerializeField] Vector3 QuizFinishAvatarPos = new Vector3 (-0.85f, 0, -1);
    [Space(10)]
    [SerializeField] Vector3 HomeCameraPos = new Vector3(0, 4.5f, -12);//0, 4.5, -12
    [SerializeField] Vector3 InvenCameraPos = new Vector3(0, 1, -14);//0, 1, -14
    [SerializeField] Vector3 QuizReadyCameraPos = new Vector3(0, 1.7f, -10);
    [SerializeField] Vector3 QuizGameCameraPos = new Vector3(0, 3.15f, -10);
    [SerializeField] Vector3 QuizFinishCameraPos = new Vector3(0, 1.2f, -10);
    [SerializeField] float CameraMoveSpeed = 10;//10
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

    #region Quiz Setting
    void QuizReadyCharacterSet()
    {
        GameObject.FindGameObjectWithTag("Character").transform.localScale = Vector3.one * QuizReadyAvatarSize;
        GameObject.FindGameObjectWithTag("Character").transform.position = OriginAvatarPos;
        Camera.main.transform.position = QuizReadyCameraPos;
    }
    void QuizStartCharacterSet()
    {
        GameObject.FindGameObjectWithTag("Character").transform.localScale = Vector3.one * QuizGameAvatarSize;
        GameObject.FindGameObjectWithTag("Character").transform.position = OriginAvatarPos;
        Camera.main.transform.position = QuizGameCameraPos;
    }
    void QuizFinishCharacterSet()
    {
        GameObject.FindGameObjectWithTag("Character").transform.localScale = Vector3.one * QuizFinishAvatarSize;
        GameObject.FindGameObjectWithTag("Character").transform.position = QuizFinishAvatarPos;
        Camera.main.transform.position = QuizFinishCameraPos;
    }
    #endregion

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
            AccountService = new FirebaseAccountService();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Data
    public async void Save()
    {
        if (!HasMatchingAuthenticatedUser()) return;
        jsonmanager.SaveJson(Data, Data.info.uid);
        try { await AccountService.SaveProfileAsync(Data.info.uid, Data); }
        catch (Exception exception) { Debug.LogError("사용자 데이터 저장 실패: " + exception.GetType().Name); }
    }

    public void Load(string uid)
    {
        Data = jsonmanager.LoadSaveData(uid);
    }

    public async Task<UserProfileLoadResult> RefreshAuthenticatedUserAsync()
    {
        if (AccountService == null || !AccountService.IsReady || AccountService.CurrentUser == null)
            return UserProfileLoadResult.Failed(new InvalidOperationException("인증된 사용자가 없습니다."));

        string uid = AccountService.CurrentUser.UserId;
        UserProfileLoadResult result = await AccountService.LoadProfileAsync(uid);
        if (result.Status == UserProfileLoadStatus.Found)
        {
            Data = result.Data;
            jsonmanager.SaveJson(Data, uid);
        }
        else if (result.Status == UserProfileLoadStatus.Missing)
        {
            Data = new DataClass();
            Data.info.uid = uid;
        }
        return result;
    }

    public async Task<bool> CompleteRegistrationAsync(string nickname)
    {
        if (!HasAuthenticatedUser()) return false;
        string uid = AccountService.CurrentUser.UserId;
        DataClass initialData = new DataClass();
        initialData.info.uid = uid;
        initialData.info.name = nickname;
        initialData.info.login = true;

        bool created = await AccountService.CreateProfileIfMissingAsync(uid, initialData);
        if (created)
        {
            Data = initialData;
            jsonmanager.SaveJson(Data, uid);
            return true;
        }

        UserProfileLoadResult existing = await RefreshAuthenticatedUserAsync();
        return existing.Status == UserProfileLoadStatus.Found;
    }

    public void Logout()
    {
        if (AccountService != null && AccountService.IsReady) AccountService.SignOut();
        Data = new DataClass();
        UImanager.UIsetting(ui_level.Lev1, ui.Login);
    }

    private bool HasAuthenticatedUser() => AccountService != null && AccountService.IsReady && AccountService.CurrentUser != null;
    private bool HasMatchingAuthenticatedUser() => HasAuthenticatedUser() && Data != null && Data.info.uid == AccountService.CurrentUser.UserId;
    #endregion

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + " Load");
        switch ((scene)System.Enum.Parse(typeof(scene), scene.name))
        {
            case Define.scene.Login: LoginSceneInit(); break;
            case Define.scene.Home: HomeSceneInit(); break;
            case Define.scene.Quiz: QuizSceneInit(); break;
            case Define.scene.RunMenu:
            case Define.scene.RunGameExplain:
            case Define.scene.RunGame: break;
            default:
                UImanager = GetComponent<UIManager>();
                UImanager.Init();
                break;
        }
    }
    private void LoginSceneInit()
    {
        // Home is the composition root and the only enabled startup scene.
        SceneManager.LoadScene("Home");
    }

    private async void HomeSceneInit()
    {
        if (Netmanager == null)
            Netmanager = GetComponent<NetworkManager>();

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


        AuthenticationMessage = null;
        try
        {
            await AccountService.InitializeAsync();
            if (AccountService.CurrentUser == null)
            {
                Data = new DataClass();
                UImanager.UIsetting(ui_level.Lev1, ui.Login);
            }
            else
            {
                UserProfileLoadResult result = await RefreshAuthenticatedUserAsync();
                if (result.Status == UserProfileLoadStatus.Found)
                    UImanager.UIsetting(ui_level.Lev1, past_ui == ui.Home ? ui.Home : past_ui);
                else if (result.Status == UserProfileLoadStatus.Missing)
                {
                    AccountService.SignOut();
                    Data = new DataClass();
                    AuthenticationMessage = "가입이 완료되지 않은 계정입니다. 이메일 인증 가입을 완료해 주세요.";
                    UImanager.UIsetting(ui_level.Lev1, ui.Login);
                }
                else
                {
                    AuthenticationMessage = "사용자 데이터를 불러오지 못했습니다. 네트워크를 확인하고 다시 로그인해 주세요.";
                    UImanager.UIsetting(ui_level.Lev1, ui.Login);
                }
            }
        }
        catch (Exception exception)
        {
            AuthenticationMessage = "Firebase 초기화에 실패했습니다. 네트워크와 앱 설정을 확인해 주세요.";
            Debug.LogError("Firebase initialization failed: " + exception.GetType().Name);
            UImanager.UIsetting(ui_level.Lev1, ui.Login);
        }

        MyAvatar();
    }

    private void QuizSceneInit()
    {
        UImanager = GetComponent<UIManager>();
        UImanager.Init();

        UImanager.UIsetting(ui_level.Lev1, ui.QuizReady);
        MyAvatar();
    }

    private void MyAvatar()
    {
        Transform parent = GameObject.FindGameObjectWithTag("Character").transform;

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
    public void SetPastUI(ui ui)
    {
        past_ui = ui;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
