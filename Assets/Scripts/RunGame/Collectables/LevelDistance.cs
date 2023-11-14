using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelDistance : MonoBehaviour
{
    public GameObject disDisplay;
    public GameObject disEndDisplay;
    public int disRun;
    public bool addingDis = false;

    void Update()
    {
        if (addingDis == false)
        {
            addingDis = true;
            StartCoroutine(AddingDis());
        }
    }

    IEnumerator AddingDis()
    {
        disRun += 1;
        disDisplay.GetComponent<TextMeshProUGUI>().text = "" + disRun;
        disEndDisplay.GetComponent<TextMeshProUGUI>().text = "" + disRun;
        yield return new WaitForSeconds(0.25f);
        addingDis = false;
    }
}
