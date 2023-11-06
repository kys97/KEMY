using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTap : MonoBehaviour
{
    [SerializeField] Define.item_type type;


    void Start()
    {
        type = Define.item_type.Hair;

        
    }

    void Update()
    {
        
    }

    public void SetInventoyry(Define.item_type type)
    {
        this.type = type;


    }
}
