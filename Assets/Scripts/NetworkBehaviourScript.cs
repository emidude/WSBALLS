using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class NetworkBehaviourScript : NetworkBehaviour

{
    public SteamVR_Behaviour_Pose controllerPoseLeft;
    public GameObject networkCube;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;        }

        CmdNetworkCubeSpawn();
    }

    [Command]
    void CmdNetworkCubeSpawn()
    {
        Vector3 handVelocity = controllerPoseLeft.GetVelocity();
        GameObject cubeClone = (GameObject)Instantiate(networkCube, handVelocity, Quaternion.identity);
        NetworkServer.Spawn(cubeClone);
        Destroy(cubeClone, 2.0f);
    }
}
