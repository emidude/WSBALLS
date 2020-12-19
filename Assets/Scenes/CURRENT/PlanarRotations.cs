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
   
    private const float TwoPi = Mathf.PI * 2;
        
    void Start()
    {
        //this is the same as Vec4 unitNormal in SLIDERS_Slicing script
        //startingUnitNormal = new Quaternion( 1f, 0f, 0f, 0f);
        
        referenceObject = GameObject.FindWithTag("SLIDEY-SLICE");
        slicingScript = referenceObject.GetComponent<SLIDERS_Slicing>();
    }
    
    void Update()
    {

        slicingScript.XYrot = getRotationValue(LeftControllerPose, 0);
        slicingScript.YZrot = getRotationValue(LeftControllerPose, 1);
        slicingScript.ZWrot = getRotationValue(LeftControllerPose, 2);

        slicingScript.YWrot = getRotationValue(RightControllerPose, 0);
        slicingScript.XWrot = getRotationValue(RightControllerPose, 1);
        slicingScript.XZrot = getRotationValue(RightControllerPose, 2);

    }
    
    //TODO: maybe put rotation of unit normal in here, not in sliders script

    float getRotationValue(SteamVR_Behaviour_Pose controller, int dof)
    {
        return Mathf.Deg2Rad * controller.transform.eulerAngles[dof];
    }
    
   
   
}
