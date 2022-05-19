using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballMovingOutwards : MonoBehaviour
{
    public Transform t;
    public float born;
    private float speed = 0.05f;
    public Vector3 outwardsVec;
   
    void Start()
    {
        born = Time.fixedTime;
        outwardsVec = t.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.fixedTime>=born + 2){ Object.Destroy(this.gameObject); };

         t.position += ForwardsVector(t.rotation, speed);
        /*//public static Quaternion LookRotation(Vector3 forward, Vector3 upwards = Vector3.up);
        Quaternion rot = Quaternion.LookRotation(t.position);
        t.position += ForwardsVector(rot, speed);*/
    }

    Vector3 ForwardsVector(Quaternion rot, float sped)
    {
        return (rot * Vector3.forward ) * sped;
    }
  
}
