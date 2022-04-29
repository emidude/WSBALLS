using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballMovingOutwards : MonoBehaviour
{
    public Transform t;
    // Start is called before the first frame update
    public float born;
    void Start()
    {
        born = Time.fixedTime;
    }

    // Update is called once per frame
    void Update()
    {
       // if(Vector3.Distance(t.position, )...more calucaltions, try lighterweihgt first
        if(Time.fixedTime>=born + 1){ Object.Destroy(this.gameObject); };
        t.position += t.rotation * Vector3.one * 0.2f; 
        
    }
}
