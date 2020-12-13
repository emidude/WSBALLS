using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlanarRotations : MonoBehaviour
{
    public SteamVR_Behaviour_Pose LeftControllerPose;
    public SteamVR_Behaviour_Pose RightControllerPose;

    //public Quaternion normVec4Origin2Hyperplane; 

    GameObject referenceObject;
    SLIDERS_Slicing slicingScript;
    //Quaternion startingUnitNormal;
   
        
    void Start()
    {
        //this is the same as Vec4 unitNormal in SLIDERS_Slicing script
        //startingUnitNormal = new Quaternion( 1f, 0f, 0f, 0f);
        
        referenceObject = GameObject.FindWithTag("SLIDEY-SLICE");
        slicingScript = referenceObject.GetComponent<SLIDERS_Slicing>();
    }
    
    void Update()
    {
        //checking controller updates, it does but does not pass by reference if set Quat newVar = LeftControllerPose.transform.rotation
        //Debug.Log("actal controrller var = " + LeftControllerPose.transform.rotation);

        // checking is unit quaternion: it is!:)
        /*Vector4 testV4 = quat2Vec4(LeftControllerPose.transform.rotation);
        checkUnit(testV4);*/
        
        RotationIn4D(LeftControllerPose.transform.rotation, RightControllerPose.transform.rotation, startingUnitNormal);
        
    }

    void RotationIn4D(Quaternion qL, Quaternion qR, Quaternion p)
    {
        //checking * operator works for quaternions as expected with matrix mutiplication as specified from wiki, 
        //only checked left multiplication but it seems to work ok :)
        /*Quaternion newRot = qL * p
        Debug.Log("quat star = "+Time.time + " " + newRot);
        //wikipedia code:
        Quaternion newRot2 = QuatLeftMultiply(qL, p);
        Debug.Log("quat left multiply = "+Time.time + " "  + newRot2);*/

        Quaternion hyperPlaneDirection = qL * p * qR;
        slicingScript.unitNormal = quat2Vec4(hyperPlaneDirection);
    }

    Quaternion QuatLeftMultiply(Quaternion qL, Quaternion p)
    {
        float w = qL.w * p.w - qL.x * p.x - qL.y * p.y - qL.z * p.z;
        float x = qL.x * p.w + qL.w * p.x - qL.z * p.y + qL.y * p.z;
        float y = qL.y * p.w + qL.z * p.x + qL.w * p.y - qL.x * p.z;
        float z = qL.z * p.w - qL.y * p.x + qL.x * p.y + qL.w * p.z;
        Quaternion newQuat = new Quaternion(x,y,z,w);
        return newQuat;
    }
    
    Vector4 quat2Vec4(Quaternion quat)
    {
     return new Vector4(quat.x,quat.y,quat.z,quat.w);
    }
    void checkUnit(Vector4 vec)
    {
        Debug.Log("mag= "+vec.magnitude);
    }
}
