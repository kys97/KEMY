using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Vector3 HomePos;
    [SerializeField] Vector3 InvenPos;

    [SerializeField] float MoveSpeed;

    void Start()
    {
        transform.position = HomePos;
    }

    void Update()
    {
        //카메라 위치 이동
        if (GameManager.Instance.TopUI == Define.ui.Home && transform.position != HomePos)
            transform.position = Vector3.Lerp(transform.position, HomePos, Time.deltaTime * MoveSpeed);
        else if (GameManager.Instance.TopUI == Define.ui.Inventory && transform.position != InvenPos)
            transform.position = Vector3.Lerp(transform.position, InvenPos, Time.deltaTime * MoveSpeed);
    }
}
