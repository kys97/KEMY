using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class Move : MonoBehaviour
{
    public bool IsMove = false;
    public void SetMove(Vector2 dir) { MoveDir = dir; }

    [SerializeField] private float Speed;
    private Vector2 MoveDir;

    private Transform ChPosition;
    private Transform ChCamera;

    private Animation Anim;


    void Start()
    {
        ChPosition = transform.GetChild(0).transform;
        ChCamera = transform.GetChild(1).transform;
    }

    
    private void FixedUpdate()
    {
        if (IsMove)
        {
            ChPosition.position += new Vector3(MoveDir.x, 0, MoveDir.y) * Speed * Time.fixedDeltaTime;
            ChCamera.position += new Vector3(MoveDir.x, 2, MoveDir.y) * Speed * Time.fixedDeltaTime;
            ChPosition.rotation = Quaternion.Euler(0f,Mathf.Atan2(MoveDir.x, MoveDir.y) * Mathf.Rad2Deg,0f);
        }
    }
}
