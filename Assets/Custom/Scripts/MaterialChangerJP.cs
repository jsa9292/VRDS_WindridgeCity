using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MaterialChangerJP : MonoBehaviour
{
    public Material mat;
    private Material mat_origin;
    private MeshRenderer mr;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mat_origin = mr.material;
    }

    // Update is called once per frame
    public void Switch(bool on)
    {
        if (on)
        {
            mr.material = mat_origin;
            //Debug.Log("waiting");
        }
        else
        {
            mr.material = mat;
            //Debug.Log("Showing");
        }
    }
}
