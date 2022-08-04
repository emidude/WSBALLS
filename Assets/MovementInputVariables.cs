using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MovementInputVariables : MonoBehaviour
{
    public SteamVR_Behaviour_Pose controllerPoseL, controllerPoseR;
    public Transform head;

    float localPosX_L, localPosX_R, localPosY_L, localPosY_R, localPosZ_L, localPosZ_R;
    float velX_L, velX_R, velY_L, velY_R, velZ_L, velZ_R;
    float angVelX_L, angVelX_R, angVelY_L, angVelY_R, angVelZ_L, angVelZ_R;
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        //floats do not update with controllers!
        Debug.Log("local pos x L =" + localPosX_L + "controller=" + controllerPoseL.transform.localPosition.x );
        float t = Time.time;
        transform.localPosition = new Vector3(Mathf.Sin(  controllerPoseR.GetVelocity().x + t), Mathf.Sin(controllerPoseR.GetVelocity().y + t), Mathf.Sin(controllerPoseR.GetVelocity().z + t)) ;

    }
}
