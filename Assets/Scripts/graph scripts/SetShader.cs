using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetShader : MonoBehaviour {

    public Vector3 colours;

	// Use this for initialization
	void Start () {
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetVector("_Colours", colours);
    }
	
	// Update is called once per frame
	void Update () {
		
	}


}
