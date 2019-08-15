using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class UpdateN : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    //public SteamVR_Input_Sources rightHand;
    public SteamVR_Behaviour_Pose controllerPose;
    //public SteamVR_Behaviour_Pose controllerPoseR;
    public SteamVR_Action_Boolean trigger;

    public Transform planePF;
    public Transform xyRot, yzRot, xzRot;
    // Start is called before the first frame update
    void Start()
    {
        //instantiate prefab
         xyRot = Instantiate(planePF, new Vector3(1, 1.5f, 1), Quaternion.identity);
         // xArrow.SetParent(transform, false);
         yzRot = Instantiate(planePF, new Vector3(1, 1.5f, 1), Quaternion.identity);
        //  yArrow.SetParent(transform);
         xzRot = Instantiate(planePF, new Vector3(1, 1.5f, 1), Quaternion.identity);
        //  zArrow.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (trigger.GetLastStateDown(handType)){

        }
    }
}
