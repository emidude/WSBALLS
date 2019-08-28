using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_createASphere : MonoBehaviour
{
    public GameObject ball;
    // Start is called before the first frame update
    void Start()
    {
        ball = Instantiate(ball);
        ball.transform.SetParent(this.transform, true);
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
