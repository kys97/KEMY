using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;

public class KampAvatar : MonoBehaviourPunCallbacks
{
    private PhotonView pv;
    private Vector2 move_dir;

    public float Speed;
    

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(pv.IsMine)
        {
            if(JoyStick.IsMove)
                pv.RPC("Move", RpcTarget.All, JoyStick.MoveDir);
            
        }
    }

    [PunRPC]
    public void Move(Vector2 dir)
    {
        transform.position += new Vector3(dir.x, 0, dir.y) * Speed * Time.deltaTime;
        //ChCamera.position += new Vector3(MoveDir.x, transform.position.y, MoveDir.y) * Speed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(0f, Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg, 0f);
    }
}
