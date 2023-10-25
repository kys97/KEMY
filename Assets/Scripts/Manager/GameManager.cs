using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager UImanager;
    public ResourcesManager Resourcesmanager;


    private void Awake()
    {
        UImanager = GetComponent<UIManager>();
        Resourcesmanager = GetComponent<ResourcesManager>();

        UImanager.Init();
        Resourcesmanager.Init();
    }

    void Start()
    {
        //�α��� Ȯ��
        //�α��� ȭ�� Load
        //UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Login);
        UImanager.UIsetting(Define.ui_level.Lev1, Define.ui.Home);
    }

    void Update()
    {
        
    }
}
