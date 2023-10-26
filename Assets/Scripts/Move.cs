using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Move : MonoBehaviour
{
    public bool IsMove = false;

    [SerializeField] private float Speed;
    private Vector3 MovePosition;
    private Vector2 RoateDir;
    public void SetMove(Vector2 dir) { RoateDir = dir; }

    private Animation Anim;


    void Start()
    {

    }

    
    private void FixedUpdate()
    {
        if (IsMove)
        {
            //CharacterMove
            //x축에 방향에 속도 시간을 곱한 값
            //y축에 0, 점프 안할거라서
            //z축에 y방향에 속도 시간을 곱한 값
            transform.position += new Vector3(RoateDir.x * Speed * Time.deltaTime, 0f, RoateDir.y * Speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f,Mathf.Atan2(RoateDir.x, RoateDir.y) * Mathf.Rad2Deg,0f);
        }
    }
}
