using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setShader : MonoBehaviour
{
    public float sliceID;
    public Vector4 coords4D;
    // Start is called before the first frame update
    void Start()
    {
        //Fetch the Renderer from the GameObject
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetVector("_FourDCoordinatesAtStart", coords4D);
        rend.material.SetFloat("_SliceID", sliceID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
