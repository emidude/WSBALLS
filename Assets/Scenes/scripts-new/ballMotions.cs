using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ballMotions : MonoBehaviour
    {
    //very bad makes controllers appear far from body
    public Transform body;
    public Transform rightHand;
    public Transform leftHand;
    public Transform ballPF;
    private Transform projectile;
    // Start is called before the first frame update
    void Start()
    {
        //every 0.3 second instantiate a sphere
        InvokeRepeating("LaunchProjectile", 2.0f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
       

        //let that sphere move away according to rotation of controller (tangent to this)
    }

    void LaunchProjectile()
    {

        Transform instance = Instantiate(projectile, rightHand.position, rightHand.rotation);
    }


}
