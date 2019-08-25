using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ActionsTest : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    // public SteamVR_Action_Pose teleportAction;
    public SteamVR_Action_Boolean teleportAction;
    public SteamVR_Action_Boolean grabAction;

    private GameObject collidingObject;
    private GameObject objectInHand;


    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;
    
    private void SetCollidingObject(Collider col){
        if (collidingObject || !col.GetComponent<Rigidbody>()){
            return;
        }
        collidingObject = col.gameObject;
    }
    public void OnTriggerEnter(Collider other)
{
    SetCollidingObject(other);
}

public void OnTriggerStay(Collider other)
{
    SetCollidingObject(other);
}
public void OnTriggerExit(Collider other)
{
    if (!collidingObject)
    {
        return;
    }

    collidingObject = null;
}

private void GrabObject(){
    
}



    void Update()
    {
        // Vector3 eulerRotation = new Vector3(0, 0, playerHand.transform.eulerAngles.z);
        // transform.rotation = Quaternion.Euler(eulerRotation);

        // if (teleportAction.GetStateDown(handType)){
        //     RaycastHit hit;

        // if (Physics.Raycast(controllerPose.transform.position, laserTransform.forward, out hit, 100)){
        //     hitPoint = hit.point;
        //     ShowLaser(hit);
        // }
        // }
        // else {
        //     laser.SetActive(false);
        // }

        if (GetTeleportDown()){
            print("telepor" + handType);
        }
        if(GetGrab()){
            print("grab" + handType);
        }
    }

    public bool GetTeleportDown(){
        return teleportAction.GetStateDown(handType);
    }

    public bool GetGrab(){
        return grabAction.GetState(handType);
    }



    private void ShowLaser(RaycastHit hit){

        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(controllerPose.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x,laserTransform.localScale.y,hit.distance);
    }
}


