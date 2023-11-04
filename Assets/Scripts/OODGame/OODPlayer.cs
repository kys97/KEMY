using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OODPlayer : MonoBehaviour
{
    [SerializeField]
    private int Score = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetScore(Define.topic obj_topic)
    {
        if(obj_topic == OODGameManager.Instance.GameTopic)
        {
            Score += 100;
        }
        else
        {
            Score -= 30;
        }
    }
}
