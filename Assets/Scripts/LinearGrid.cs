using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LinearGrid : MonoBehaviour
{
    public GameObject pf;
    public int size;
    public GameObject player;
    public Vector3 centre;
    public float scale = 0.5f;
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;


    // Start is called before the first frame update
    void Start()
    {
        centre = player.transform.position;

        InvokeRepeating("LaunchProjectiles", 2.0f, 0.25f);

        
    }

    void LaunchProjectiles()
    {
        //EXPERIMTATIONS WITH GRID, BOTH 3D AND 2D VERY BLOCKY, TRY SPHERICAL
        for (int i = -size; i < size; i++)
        {
            for (int j = -size; j < size; j++)
            {
                /* for (int k = -size; k < size; k++)
                 {

                     GameObject point = Instantiate(pf);
                     point.transform.position = new Vector3(i + centre.x, j + centre.y, k + centre.z) * scale;
                     point.transform.rotation = controllerPose.transform.rotation;
                 }*/
               /* GameObject point = Instantiate(pf);
                point.transform.position = new Vector3(i + centre.x, j + centre.y, centre.z) * scale;
                point.transform.rotation = controllerPose.transform.rotation;*/
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
