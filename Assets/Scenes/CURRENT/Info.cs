using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

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

    public Vector3 current3Dcoords;


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
        //Debug.Log("numberOfds = " + numberOfDs);
        for (int i = 0; i < numberOfDs; i++)
        {
            GameObject thisSlice = Instantiate(sliceOfD);
            //slice.transform.localScale = new Vector3(1, 1, 1); set transform when you calc slice centres
            setShader shaderInfo = thisSlice.GetComponent<setShader>();
            shaderInfo.coords4D = coords4D;
            shaderInfo.sliceID = 1f - 2* (i / numberOfDs);
            //testing:
            //thisSlice.transform.localScale = new Vector3(1, 1, 1);
            //thisSlice.transform.localPosition = new Vector3(0, 0, 0);

            if (i != 0)
            {
                

                thisSlice.GetComponent<IgnoreHovering>();
                Destroy(thisSlice.GetComponent<InteractableHoverEvents>());
                //Destroy(thisSlice.GetComponent<UpdateSphereCoordinates>());
                //Destroy(thisSlice.GetComponent<Interactable>());
                Destroy(thisSlice.GetComponent<SphereCollider>());
                
                // shaderInfo.metal = 0f;
            }
            //else
            //{
            //  //  shaderInfo.metal = 1f;
            //}
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

        current3Dcoords = slicesOfD[0].transform.position;
    }

    private void Update()
    {
        if(current3Dcoords != slicesOfD[0].transform.position)
        {
             current3Dcoords = slicesOfD[0].transform.position ;
           // Debug.Log("slicesOfD[0].transform.position=" + slicesOfD[0].transform.position + ", current3Dcoords=" + current3Dcoords);
        }
    }
}
