using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class UpdatePlaneRotations : MonoBehaviour
{
    //public LinearMapping XY, XZ, XW, YZ, YW, ZW;
    public GameObject XY, XZ, XW, YZ, YW, ZW;
    // Start is called before the first frame update
    public List<GameObject> planeObjects;
    private LinearMapping[] planeLinearMappings;
    //private float currentXY, currentXZ, currentXW, currentYZ, currentYW, currentZW;
    private float[] currentPlaneValues;

    void Start()
    {
        //if (XY == null)
        //{
        //    linearMapping = GetComponent<LinearMapping>();
        //}

        planeObjects = new List<GameObject>() { XY, XZ, XW, YZ, YW, ZW };
        //currentPlaneValues = new float[6]{ currentXY, currentXZ, currentXW, currentYZ, currentYW, currentZW };
        currentPlaneValues = new float[6] { 0f,0f,0f,0f,0f,0f };
        //planeLinearMappings = new LinearMapping[6] { XY, XZ, XW, YZ, YW, ZW };
        planeLinearMappings = new LinearMapping[6];

        for (int i = 0; i < 6; i++)
        {
            planeLinearMappings[i] = planeObjects[i].GetComponent<LinearMapping>();

        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 6; i++)
        {
            if (currentPlaneValues[i] != planeLinearMappings[i].value)
            {
                currentPlaneValues[i] = planeLinearMappings[i].value;
                Debug.Log(currentPlaneValues[i] + " UPDATED PLANE " + i + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }

            //if (currentXY != XY.value)
            //{
            //    Debug.Log("NewXY");
            //}
        }

    }
}
