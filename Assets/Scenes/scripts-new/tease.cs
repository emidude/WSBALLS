using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class tease : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public Transform pf;
    public Transform head;
    public Transform hand;
    
    
    // Start is called before the first frame update
    void Start()
    {
        controllerPose.transform.parent = head.transform;
        InvokeRepeating("LaunchProjectile", 2.0f, 0.3f);

        hand = controllerPose.transform;

        Debug.Log("hand transform position = " + hand.position);
        Debug.Log("hand transform lcoal position = " + hand.localPosition);
        //Debug.Log("transform veloctity = " + hand.transform.veloc) 

        //Vector3 posL = controllerPose.transform.localPosition;
       // Vector3 posH = head.transform.localPosition;

        Vector3 velo = controllerPose.GetVelocity();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LaunchProjectile()
    {
        Transform instance = Instantiate(pf, hand.position, hand.rotation);
    }
}
