using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class YWsendToSlicing : MonoBehaviour
{
    public LinearMapping linearMapping;
    private float sliderVal;
    private float rotationVal;

    private const float PI = Mathf.PI;
    private const float TwoPi = PI * 2;

    GameObject referenceObject;
    SlicingSpheres slicingScript;
    // Start is called before the first frame update
    void Start()
    {
        if (linearMapping == null)
        {
            linearMapping = GetComponent<LinearMapping>();
        }

        referenceObject = GameObject.FindWithTag("slice");
        slicingScript = referenceObject.GetComponent<SlicingSpheres>();

    }

    // Update is called once per frame
    void Update()
    {
        if (sliderVal != linearMapping.value)
        {
            sliderVal = linearMapping.value;
            rotationVal = Mathf.Lerp(0, TwoPi, sliderVal);
            slicingScript.YWrot = rotationVal;
        }
    }
}
