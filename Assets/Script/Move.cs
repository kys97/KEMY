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
            //x�࿡ ���⿡ �ӵ� �ð��� ���� ��
            //y�࿡ 0, ���� ���ҰŶ�
            //z�࿡ y���⿡ �ӵ� �ð��� ���� ��
            transform.position += new Vector3(RoateDir.x * Speed * Time.deltaTime, 0f, RoateDir.y * Speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f,Mathf.Atan2(RoateDir.x, RoateDir.y) * Mathf.Rad2Deg,0f);
        }
    }
}
