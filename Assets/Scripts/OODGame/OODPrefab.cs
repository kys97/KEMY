using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OODPrefab : MonoBehaviour
{
    private Define.topic topic;

    void Start()
    {
        topic = Define.topic.Animal;
    }

    void Update()
    {
        
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Character")
        {
            collision.gameObject.GetComponent<OODPlayer>().GetScore(topic);
            Destroy(gameObject);
        }
    }
}
