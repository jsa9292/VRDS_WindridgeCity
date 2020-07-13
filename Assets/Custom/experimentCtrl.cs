using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class experimentCtrl : MonoBehaviour
{
    public int randSeed;
    // Start is called before the first frame update
    void Awake()
    {
        Random.InitState(randSeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
