using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class experimentCtrl : MonoBehaviour
{
    public int randSeed;
    public GameObject participant;
    public int TrialNumber;
    // Start is called before the first frame update
    void Awake()
    {
        Random.InitState(randSeed);
        //load Windridge
        
    }
    void SetupT1() {
        randSeed = 1;
        //Free Drive
    }
    void SetupT2() {
        randSeed = 1;
        //Script Drive
    }
    void SetupT3() {
        randSeed = 1;
        //Free Drive
    }
    void SetupT4() {
        randSeed = 1;
        //Script Drive
    }
    void SetupT5() {
        randSeed = 1;
        //Free Drive
    }
    void SetupT6() {
        randSeed = 1;
        //load Trolley
    }
}
