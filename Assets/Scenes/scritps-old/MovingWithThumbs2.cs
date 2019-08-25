using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MovingWithThumbs2 : MonoBehaviour
{
    public  SteamVR_Action_Vector2 touchPad;
    public SteamVR_Input_Sources hand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // Vector2 x =  touchPad.GetAxis(hand);
      //  Debug.Log("xy="+x);
    }
}
