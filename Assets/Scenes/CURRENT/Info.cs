using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Info : MonoBehaviour {

    public Vector4 coords4D;

    Vector3 centre3D; //keep this in float land

    public float radius;// = 1f; cannot set value here! => radius = 0;

    public int totalNumberOfSpheres;
    public int uniqueBallIdentifier;
    float uniqueColorIdentifier;

    public int numberOfDs;
    public GameObject sliceOfD;
    public List<GameObject> slicesOfD;

    public Vector3 current3Dcoords;
    public Vector4 rotated4d;

    GameObject referenceObject;
    SLIDERS_Slicing slicingScript;

    private Vector4 unitNormalParallelToZ = new Vector4(0f, 0f, 0f, 1f);
    public float minDis;

    public bool changedToTempCols = false;

    public void setCoords4D(Vector4 c4d){
        coords4D = c4d;
    }

    public Vector4 Get4DCoords(){
        return coords4D;
    }
    public void setUnity4Dcoords(Vector4 u4dc) {
        rotated4d = u4dc; 
    }
    public void setUniqueColorIdentifier(int i)
    {
        uniqueBallIdentifier = i;
        uniqueColorIdentifier = i/ (float)totalNumberOfSpheres; 
    }

    public void createSlices(Vector3 randColors)//////////////////////aggggggggg so bad
    {
        slicesOfD = new List<GameObject>();
        //Debug.Log("numberOfds = " + numberOfDs);
        for (int i = 0; i < numberOfDs; i++)
        {
            GameObject thisSlice = Instantiate(sliceOfD);
            //thisSlice.transform.SetParent(parent, true);
            thisSlice.transform.localScale = Vector3.one;
            //slice.transform.localScale = new Vector3(1, 1, 1); set transform when you calc slice centres
            setShader shaderInfo = thisSlice.GetComponent<setShader>();
            shaderInfo.coords4D = coords4D;
            shaderInfo.sliceID = 1f - 2* (i / numberOfDs);
            //shaderInfo.sliceIDINT = uniqueBallIdentifier * 10 + i; 
            shaderInfo.randomCols = randColors;
            
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

        referenceObject = GameObject.FindWithTag("SLIDEY-SLICE");
        slicingScript = referenceObject.GetComponent<SLIDERS_Slicing>();
    }

    void calculateUnreflectedParalleltoWCoords(Vector4 unitNormal, Vector4 unityCoords) {
        //perform the opposite reflection to get back world coords 
        //is this actually what you need though? - yes for calculating next reflection back for screen
        //but for intersection calculation? probably more straightforward for this too...?
    }

    private void Update()
    {
        if(current3Dcoords != slicesOfD[0].transform.position)
        {
            current3Dcoords = slicesOfD[0].transform.position ;
            //Debug.Log("slicesOfD[0].transform.position=" + slicesOfD[0].transform.position + ", current3Dcoords=" + current3Dcoords);
            rotated4d.x = current3Dcoords.x;
            rotated4d.y = current3Dcoords.y;
            rotated4d.z = current3Dcoords.z;

            //needs n!
            //Vector4 n = GetComponentInParent<SLIDERS_Slicing>().getUnitNormal(); //this does not work because parent not set (becuase child scale went to zero when set)
            Vector4 unitNormal = slicingScript.getUnitNormal();
            Vector4 unrotatedSliceCentre = rotateXfromUtoV(rotated4d, unitNormalParallelToZ, unitNormal);
            //Debug.Log("unitNormal = " + n + ", unitycoords = " + rotated4d);
            //Vector4 oldCoords4D = coords4D;
            coords4D = unrotatedSliceCentre - minDis * unitNormal;
            //Debug.Log("old coords4D=" + oldCoords4D + ", updated coords4D =" + coords4D + ", minDis="+ minDis + ", unitNormal=" + unitNormal);

            updateSlicesOfD();
            
            int state = slicingScript.checkIntersection(coords4D, uniqueBallIdentifier);
            //if intersecting - give haptic feedback and change colour of balls intersecting (like with highlighting)
            if (state == 0)
            {
                //kissing
                if (changedToTempCols)
                {
                    for (int i = 0; i < numberOfDs; i++)
                    {
                        slicesOfD[i].GetComponent<setShader>().setOriginalColors();
                        changedToTempCols = false;
                    }
                    
                }
            }
            else if (state == 1)
            {
                //intersecting
                for (int i = 0; i < numberOfDs; i++)
                {
                    slicesOfD[i].GetComponent<setShader>().setShaderColorIntersecting();
                    changedToTempCols = true;
                }
            }
            else if (state == 2)
            {
                for (int i = 0; i < numberOfDs; i++)
                {
                    slicesOfD[i].GetComponent<setShader>().setOriginalColors();
                    changedToTempCols = false;
                }
            }
            

        }
    }

    void updateSlicesOfD()
    {
        for (int i = 1; i < slicesOfD.Count; i++)
        {
            slicesOfD[i].transform.localPosition = slicesOfD[0].transform.position;

        }
    }

    Vector4 rotateXfromUtoV(Vector4 x, Vector4 u, Vector4 v)
    {
        Vector4 vdash = v;
        vdash.w = -v.w;

        Vector4 n = u - vdash;

        if (n != Vector4.zero)
        {
            float dots = Vector4.Dot(x, n) / Vector4.Dot(n, n);
            Vector4 rotatedCoords4D = x - (2 * dots * n);
            //FINAL REFLECTION REQUIRES MULTIPLICATION
            //WITH DIAG D, WITH -1 WHERE FOR W COMPONENT (AS PREVIOUSLY FLIPPED)
            rotatedCoords4D.w = -rotatedCoords4D.w;
            ///////////////////////////////////////////////////////////// 
            return rotatedCoords4D;
        }
        else return x;

    }
}
