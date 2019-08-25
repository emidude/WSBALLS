using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info : MonoBehaviour {

    public Vector4 coords4D;

    Vector3 centre3D; //keep this in float land

    public float radius;// = 1f; cannot set value here! => radius = 0;

    public int totalNumberOfSpheres;
    int uniqueBallIdentifier;
    float uniqueColorIdentifier;

    public int numberOfDs;
    public GameObject sliceOfD;
    public List<GameObject> slicesOfD;


    public void setCoords4D(Vector4 c4d){
        coords4D = c4d;
    }

    public Vector4 Get4DCoords(){
        return coords4D;
    }

    public void setUniqueColorIdentifier(int i)
    {
        uniqueBallIdentifier = i;
        uniqueColorIdentifier = i/ (float)totalNumberOfSpheres; 
    }

    public void createSlices()//////////////////////aggggggggg so bad
    {
        slicesOfD = new List<GameObject>();
        Debug.Log("numberOfds = " + numberOfDs);
        for (int i = 0; i < numberOfDs; i++)
        {
            GameObject thisSlice = Instantiate(sliceOfD);
            //slice.transform.localScale = new Vector3(1, 1, 1); set transform when you calc slice centres
            setShader shaderInfo = thisSlice.GetComponent<setShader>();
            shaderInfo.coords4D = coords4D;
            shaderInfo.sliceID = 0.9f - i / numberOfDs;
            //testing:
            //thisSlice.transform.localScale = new Vector3(1, 1, 1);
            //thisSlice.transform.localPosition = new Vector3(0, 0, 0);
            slicesOfD.Add(thisSlice);
        }

    }

    private void Start()
    {
        //setting shader colors:
        //Fetch the Renderer from the GameObject
        //Renderer rend = GetComponent<Renderer>();
        //rend.material.SetVector("_FourDCoordinatesAtStart", coords4D);
        //rend.material.SetFloat("_BallColorID", uniqueColorIdentifier);

        radius = 1f;
    }
}
