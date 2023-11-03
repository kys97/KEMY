using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Vector3 HomePos = new Vector3(0, 4, -10);
    [SerializeField] Vector3 HomeRot = new Vector3(10, 0, 0);

    [SerializeField] Vector3 InvenPos = new Vector3(0, 7, -20);
    [SerializeField] Vector3 InvenRot = new Vector3(30, 0, 0);

    [SerializeField] float MoveSpeed;
    [SerializeField] float RotateSpeed;
    Vector3 velocity = Vector3.zero;

    void Start()
    {
        transform.position = HomePos;
        transform.rotation = Quaternion.Euler(HomeRot);
    }

    void Update()
    {
        if (GameManager.Instance.TopUI == Define.ui.Home && transform.position != HomePos && transform.rotation != Quaternion.Euler(HomeRot))
        {
            transform.position = Vector3.Lerp(transform.position, HomePos, Time.deltaTime * MoveSpeed);
            transform.eulerAngles = Vector3.SmoothDamp(transform.eulerAngles, HomeRot, ref velocity, RotateSpeed);
        }
        else if (GameManager.Instance.TopUI == Define.ui.Inventory && transform.position != InvenPos && transform.rotation != Quaternion.Euler(InvenRot))
        {
            transform.position = Vector3.Lerp(transform.position, InvenPos, Time.deltaTime * MoveSpeed);
            transform.eulerAngles = Vector3.SmoothDamp(transform.eulerAngles, InvenRot, ref velocity, RotateSpeed);
        }
        /* else if (GameManager.Instance.TopUI == Define.ui.Shop && transform.position == InvenPos && transform.rotation == Quaternion.Euler(InvenRot))//상점
         * {
         * }     
         */
        //메세지 화면에서 화면 왼쪽으로 치워놓기
    
    }
}
