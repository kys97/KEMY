using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectBox : MonoBehaviour
{
    public AudioSource boxFX;

    private void OnTriggerEnter(Collider other)
    {
        boxFX.Play();
        this.gameObject.SetActive(false);
    }
}
