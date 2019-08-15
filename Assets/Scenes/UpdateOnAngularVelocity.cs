using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class UpdateOnAngularVelocity : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean clicked;

    public Transform xArrowPF, yArrowPF, zArrowPF;
    public Transform xArrow, yArrow, zArrow;
    //public GameObject xArrow, yArrow, zArrow;

    // Start is called before the first frame update
    void Start()
    {
        //instantiate prefab
         xArrow = Instantiate(xArrowPF, new Vector3(-1, 2, 1), Quaternion.identity);
         yArrow = Instantiate(yArrowPF, new Vector3(-1, 2, 1), Quaternion.identity);
         zArrow = Instantiate(zArrowPF, new Vector3(-1, 2, 1), Quaternion.identity);


    //  xArrow.localPosition = new Vector3(1f,1f,1f);
    //   yArrow.localPosition = new Vector3(1f,1f,1f);
    //    zArrow.localPosition = new Vector3(1f,1f, 1f);

    //    xArrow.gameObject.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
    //   yArrow.gameObject.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
    //    zArrow.gameObject.transform.localScale = new Vector3(0.3f,0.3f,0.3f);

     xArrow.localScale = new Vector3(0.2f,0.2f,0.2f);
      yArrow.localScale = new Vector3(0.2f,0.2f,0.2f);
       zArrow.localScale = new Vector3(0.2f,0.2f,0.2f);
    }

    // Update is called once per frame
    public void Update()
    {
        
       Vector3 velo = controllerPose.GetAngularVelocity();
        //map to scalable number between 0 and 1...
        // velo.x = 1/(1+ Mathf.Exp(velo.x));
        // velo.y = 1/(1+ Mathf.Exp(velo.y));
        // velo.z = 1/(1+ Mathf.Exp(velo.z));

        if (clicked.GetLastStateDown(handType)){
            Debug.Log("xArrow " + controllerPose.transform.position.x);
            Debug.Log("yArrow " + controllerPose.transform.position.y);
            Debug.Log("zArrow " + controllerPose.transform.position.z);
            Debug.Log("xArrowtransformed " + velo.x);
            Debug.Log("yArrowtransformed " + velo.y);
            Debug.Log("zArrowtransformed " + velo.z);
            
        }

    //     xArrow.gameObject.transform.localScale = new Vector3(velo.x,xArrow.gameObject.transform.localScale.y,xArrow.gameObject.transform.localScale.z);
    //    yArrow.gameObject.transform.localScale = new Vector3(yArrow.gameObject.transform.localScale.x,velo.y,xArrow.gameObject.transform.localScale.z);
    //    zArrow.gameObject.transform.localScale = new Vector3(zArrow.gameObject.transform.localScale.x,zArrow.gameObject.transform.localScale.y,velo.z);
    
    xArrow.localScale = new Vector3(velo.x,xArrow.localScale.y,xArrow.localScale.z);
    yArrow.localScale = new Vector3(yArrow.localScale.x,velo.y,xArrow.localScale.z);
    zArrow.localScale = new Vector3(zArrow.localScale.x,zArrow.localScale.y,velo.z);

    //    xArrow.position = new Vector3(velo.x,xArrow.localScale.y,xArrow.localScale.z);
    //    yArrow.localPosition = pos;
    }
}
