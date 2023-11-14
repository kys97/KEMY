using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 10;
    public float leftRightSpeed = 15;
    static public bool canMove = false;

    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);

        if (canMove == true)
        {
            if (Input.GetKey(KeyCode.A))
            {
                if (this.gameObject.transform.position.x > LevelBoundary.leftSide)
                {
                    transform.Translate(Vector3.left * Time.deltaTime * leftRightSpeed);
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (this.gameObject.transform.position.x < LevelBoundary.rightSide)
                {
                    transform.Translate(Vector3.left * Time.deltaTime * leftRightSpeed * -1);
                }
            }
        }
    }
}

