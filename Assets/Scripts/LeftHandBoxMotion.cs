using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LeftHandBoxMotion : MonoBehaviour
{
    public SteamVR_Behaviour_Pose controllerPoseLeft, controllerPoseRight;
    public GameObject boxPrefab;
    GameObject leftHandBox;
    //public SteamVR_Action_Boolean clicked;
    //public Transform head;

    private void Awake()
    {
        //do you need below? 
        /*controllerPoseLeft.transform.parent = head.transform;
        controllerPoseRight.transform.parent = head.transform;*/

        leftHandBox = Instantiate(boxPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        ReformBox();
    }

    void ReformBox()
    {
        Vector3 handVelocity = controllerPoseLeft.GetVelocity();
        leftHandBox.transform.localScale = handVelocity;

    }
}
