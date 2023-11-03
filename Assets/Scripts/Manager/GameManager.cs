using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public UIManager UImanager;
    [HideInInspector] public ResourcesManager Resourcesmanager;
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


    
    void Start()
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
    }

    void Update()
    {
        
    }
}
