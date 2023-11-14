using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    public GameObject thePlayer;
    public AudioSource crashThud;
    public GameObject levelControl;

    private void OnTriggerEnter(Collider other)
    {
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        thePlayer.GetComponent<PlayerMove>().enabled = false;
        thePlayer.GetComponent<Animator>().Play("DieB");
        levelControl.GetComponent<LevelDistance>().enabled = false;
        crashThud.Play();
        levelControl.GetComponent<EndRunSequence>().enabled = true;
    }
}
