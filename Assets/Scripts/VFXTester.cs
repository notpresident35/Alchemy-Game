using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXTester : MonoBehaviour
{
    
    public float CompletenessSinSpeed = 1;
    public GameObject obj1;
    public GameObject obj2;
    private Material mat1;
    private Material mat2;

    private void Start() {
        mat1 = obj1.GetComponent<Renderer>().material;
        mat2 = obj2.GetComponent<Renderer>().material;
    }

    void Update()
    {
        float sample = (Mathf.Sin(CompletenessSinSpeed * Time.time) + 1) / 2;
        mat1.SetFloat("_Completeness", sample);
        mat2.SetFloat("_Completeness", sample);
    }
}
