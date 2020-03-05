using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class setShader : MonoBehaviour
{
    public Vector4 coords4D;
    public float sliceID;

    public int sliceIDINT;
    //public float metal;

    public Vector3 randomCols;

    Vector3 intersectingCols = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        //setting shader colors:
        //Fetch the Renderer from the GameObject
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetVector("_FourDCoordinatesAtStart", coords4D);
        //rend.material.SetFloat("_BallColorID", uniqueColorIdentifier);
        rend.material.SetFloat("_SliceID", sliceID);
        
        //set random colours with seed
        
       // Debug.Log(randomCols);
        rend.material.SetVector("_RandColors", randomCols);

        // rend.material.SetFloat("_Metallic", metal);


        intersectingCols = Vector3.zero;
    }

    public void setShaderColorIntersecting()
    {
        Renderer rend = GetComponent<Renderer>();
        
        rend.material.SetVector("_RandColors", intersectingCols);
    }

    public void setOriginalColors()
    {
        Renderer rend = GetComponent<Renderer>();
        
        rend.material.SetVector("_RandColors", randomCols);
    }


}
