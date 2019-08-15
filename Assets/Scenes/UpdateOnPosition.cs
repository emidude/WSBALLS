using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class UpdateOnPosition : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean clicked;

    public Transform xArrowPF, yArrowPF, zArrowPF;
    public Transform xArrow, yArrow, zArrow, testRotation;
    //public GameObject xArrow, yArrow, zArrow;

    public GameObject startingPosObject;
    public Vector3 startingPositon; 
    // Start is called before the first frame update
    void Start()
    {
        //instantiate prefab
         xArrow = Instantiate(xArrowPF, new Vector3(1, 1, 1), Quaternion.identity);
         // xArrow.SetParent(transform, false);
         yArrow = Instantiate(yArrowPF, new Vector3(1, 1, 1), Quaternion.identity);
        //  yArrow.SetParent(transform);
         zArrow = Instantiate(zArrowPF, new Vector3(1, 1, 1), Quaternion.identity);
        //  zArrow.SetParent(transform);

        testRotation = Instantiate(zArrowPF, new Vector3(-1, 1, 1), Quaternion.identity);


    //  xArrow.localPosition = new Vector3(1f,1f,1f);
    //   yArrow.localPosition = new Vector3(1f,1f,1f);
    //    zArrow.localPosition = new Vector3(1f,1f, 1f);

    //    xArrow.gameObject.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
    //   yArrow.gameObject.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
    //    zArrow.gameObject.transform.localScale = new Vector3(0.3f,0.3f,0.3f);

     xArrow.localScale = new Vector3(0.2f,0.2f,0.2f);
      yArrow.localScale = new Vector3(0.2f,0.2f,0.2f);
       zArrow.localScale = new Vector3(0.2f,0.2f,0.2f);

       testRotation.localScale =new Vector3(0.2f,0.2f,0.2f);

        startingPositon = startingPosObject.transform.position;

       
    }

    // Update is called once per frame
    public void Update()
    {
        
       Vector3 pos =  controllerPose.transform.localPosition - startingPositon;
        //map to scalable number between 0 and 1...
        // pos.x = 5/(1+ Mathf.Exp(pos.x));
        // pos.y = 1/(1+ Mathf.Exp(pos.y));
        // pos.z = 1/(1+ Mathf.Exp(pos.z));

        if (clicked.GetLastStateDown(handType)){
            Debug.Log("xArrow " + controllerPose.transform.position.x);
            Debug.Log("yArrow " + controllerPose.transform.position.y);
            Debug.Log("zArrow " + controllerPose.transform.position.z);
            Debug.Log("xArrowtransformed " + pos.x);
            Debug.Log("yArrowtransformed " + pos.y);
            Debug.Log("zArrowtransformed " + pos.z);
            
        }

    //     xArrow.gameObject.transform.localScale = new Vector3(pos.x,xArrow.gameObject.transform.localScale.y,xArrow.gameObject.transform.localScale.z);
    //    yArrow.gameObject.transform.localScale = new Vector3(yArrow.gameObject.transform.localScale.x,pos.y,xArrow.gameObject.transform.localScale.z);
    //    zArrow.gameObject.transform.localScale = new Vector3(zArrow.gameObject.transform.localScale.x,zArrow.gameObject.transform.localScale.y,pos.z);
    
    //xArrow.localScale.x = pos.x;
       
       xArrow.localScale = new Vector3(pos.x,xArrow.localScale.y,xArrow.localScale.z);
       yArrow.localScale = new Vector3(yArrow.localScale.x,pos.y,yArrow.localScale.z);
       zArrow.localScale = new Vector3(zArrow.localScale.x,zArrow.localScale.y,pos.z);

    //    xArrow.position = new Vector3(pos.x,xArrow.localScale.y,xArrow.localScale.z);
    //    yArrow.localPosition = pos;

    // Hand playerHand;
    // Vector3 eulerRotation = new Vector3(0,0,playerHand.transform.eulerAngles.z);
    // transform.rotation = Quaternion.Euler(eulerRotation);

 //Debug.Log("contoller pose rotation z "+controllerPose.transform.eulerAngles.z);
 testRotation.rotation = Quaternion.Euler(controllerPose.transform.eulerAngles.x,controllerPose.transform.eulerAngles.y,controllerPose.transform.eulerAngles.z);
    //Debug.Log("test z rptatopm "+testRotation.eulerAngles.z);
        }
}
