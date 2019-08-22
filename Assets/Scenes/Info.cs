﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info : MonoBehaviour {

    public Vector4 coords4D;

    Vector3 centre3D; //keep this in float land

    public float radius;// = 1f; cannot set value here! => radius = 0;

    public int totalNumberOfSpheres;
    int uniqueBallIdentifier;
    float uniqueColorIdentifier;

    int numberOfDs;
    Vector4[] sliceCentres;
    Vector4[] sliceRadii;
    float minDistanceBetween;

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

    public void setSliceCentresAndRadiusArrays(int numberOfDs)
    {
        this.numberOfDs = numberOfDs;
        sliceCentres = new Vector4[numberOfDs];
        sliceRadii = new Vector4[numberOfDs];
    }

    public void createSlices()
    {
        GameObject slice = Instantiate(sliceOfD);
        slice.transform.localScale = new Vector3(1, 1, 1);
        slicesOfD.Add(slice);
    }

    private void Start()
    {
        //setting shader colors:
        //Fetch the Renderer from the GameObject
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetVector("_FourDCoordinatesAtStart", coords4D);
        rend.material.SetFloat("_BallColorID", uniqueColorIdentifier);

        radius = 1f;

        
    }
}
