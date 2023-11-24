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

    public Define.ui TopUI;

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

    public void Save()
    {
        jsonmanager.SaveJson(Data);
    }

    public void Load()
    {
        Data = jsonmanager.LoadSaveData();
    }

    void Init()
    {
        UImanager = GetComponent<UIManager>();
        Resourcesmanager = GetComponent<ResourcesManager>();

        UImanager.Init();
        Resourcesmanager.Init();

        //�α��� Ȯ��
        //�α��� ȭ�� Load
        //UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Login);

        //Homeȭ��
        UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Main);

        //�ƹ�Ÿ Setting
        Transform parent = GameObject.FindGameObjectWithTag("Character").transform;
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
        ItemWeightDic = new CsvManager().ReadCsv();

        Load();

        Init();
        //������ �ҷ�����
    }

    void Update()
    {
        
    }
}
