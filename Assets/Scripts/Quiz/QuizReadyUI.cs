using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizReadyUI : MonoBehaviour
{
    void Start()
    {
        GameObject.FindGameObjectWithTag("Character").transform.GetChild(0).gameObject.SetActive(true);
    }

    void Update()
    {
        
    }
}
