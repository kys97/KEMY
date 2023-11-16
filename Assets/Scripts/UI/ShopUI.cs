using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.UImanager.BottomOutUI(GameObject.FindGameObjectWithTag("Bottom"));
        GameManager.Instance.UImanager.RightInUI(gameObject);
    }

    void Update()
    {
        
    }
}
