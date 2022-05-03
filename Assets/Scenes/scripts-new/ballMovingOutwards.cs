using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballMovingOutwards : MonoBehaviour
{
    public Transform t;
    public float born;
    private float speed = 0.1f;
    void Start()
    {
        born = Time.fixedTime;
    }

    // Update is called once per frame
    void Update()
    {
       // if(Vector3.Distance(t.position, )...more calucaltions, try lighterweihgt first
        if(Time.fixedTime>=born + 2){ Object.Destroy(this.gameObject); };
        //t.position += t.rotation * Vector3.one * 0.2f; 
        
        t.position += ForwardsVector(t.rotation, speed);
    }

    Vector3 ForwardsVector(Quaternion rot, float sped)
    {
        return (rot * Vector3.forward ) * sped;
    }
  
}
