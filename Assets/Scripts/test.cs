using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] Material[] skin;
    [SerializeField] Material[] newskin;
    [SerializeField] Material[] materials;
    [SerializeField] int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        skin = GetComponent<SkinnedMeshRenderer>().materials;
        newskin = new Material[2];
        materials = Resources.LoadAll<Material>("Materials/");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (index == 9) index = 0;
            else index++;
            newskin[0] = materials[index];
            newskin[1] = skin[1];
            skin = newskin;
        }
    }
}
