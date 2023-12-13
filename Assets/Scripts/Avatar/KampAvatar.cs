using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;
using static System.Net.WebRequestMethods;
using Cinemachine;

public class KampAvatar : MonoBehaviourPunCallbacks
{
    private PhotonView pv;

    
    public float Speed;
    private float DirY;
    private float prev_rotation;

    Transform cam;

    Animator animator;
    bool move = false;

    void Start()
    {
        animator = transform.GetChild(2).GetComponent<Animator>();

        Transform parent = GameObject.FindGameObjectWithTag("Character").transform;
        transform.position = parent.position;
        transform.localScale = parent.localScale;
        transform.parent = parent;

        pv = GetComponent<PhotonView>();
        cam = GameObject.FindGameObjectWithTag("Cam").transform;

        if (pv.IsMine)
        {
            cam.GetComponent<CinemachineVirtualCamera>().Follow = transform.GetChild(0).transform;
            transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        }

        prev_rotation = transform.rotation.eulerAngles.y;
    }

    void Update()
    {
        if(pv.IsMine)
        {
            if (JoyStick.IsMove)
            {
                pv.RPC("Move", RpcTarget.All, JoyStick.MoveDir);
                if(!move && JoyStick.IsMove)
                {
                    move = true;
                    pv.RPC("WalkingAnim", RpcTarget.All, move);
                }
            }
            else
            {
                prev_rotation = transform.rotation.eulerAngles.y;
                if (move && !JoyStick.IsMove)
                {
                    move = false;
                    pv.RPC("WalkingAnim", RpcTarget.All, move);
                }
            }
        }
    }

    [PunRPC]
    public void Move(Vector2 dir)
    {
        transform.position += transform.forward * Speed * Time.deltaTime;

        DirY = (Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI - 90) * -1f;
        transform.rotation = Quaternion.Euler(0f, DirY + prev_rotation, 0f);
    }

    [PunRPC]
    public void WalkingAnim(bool move)
    {
        animator.SetBool("move", move);
    }
}
