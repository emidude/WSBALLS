using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class testCentring : MonoBehaviour
{
    public SteamVR_Behaviour_Pose controllerPoseLeft, controllerPoseRight;
   
    public Transform head;
    // Start is called before the first frame update
    void Start()
    {
        controllerPoseLeft.transform.parent = head.transform;
        controllerPoseRight.transform.parent = head.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
