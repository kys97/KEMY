using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISizeSet : MonoBehaviour
{
    [SerializeField] int HomeSize;
    [SerializeField] int InvenSize;

    [SerializeField] float SizeSpeed;

    void Start()
    {
        transform.localScale = Vector3.one * HomeSize;
    }

    void Update()
    {
        if (GameManager.Instance.TopUI == Define.ui.Home && transform.localScale.x < HomeSize)
            transform.localScale += Vector3.one * HomeSize * Time.deltaTime * SizeSpeed;
        else if (GameManager.Instance.TopUI == Define.ui.Inventory && transform.localScale.x > InvenSize)
            transform.localScale -= Vector3.one * InvenSize * Time.deltaTime * SizeSpeed;
    }
}
