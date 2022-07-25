using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class XZsendToSlicing : MonoBehaviour
{
    public LinearMapping linearMapping;
    private float sliderVal;
    private float rotationVal;

    private const float PI = Mathf.PI;
    private const float TwoPi = PI * 2;

    GameObject referenceObject;
    SLIDERS_Slicing slicingScript;
    // Start is called before the first frame update
    void Start()
    {
        if (linearMapping == null)
        {
            linearMapping = GetComponent<LinearMapping>();
        }

        referenceObject = GameObject.FindWithTag("SLIDEY-SLICE");
        slicingScript = referenceObject.GetComponent<SLIDERS_Slicing>();

    }

    // Update is called once per frame
    void Update()
    {
        if (sliderVal != linearMapping.value)
        {
            sliderVal = linearMapping.value;
            rotationVal = Mathf.Lerp(0, TwoPi, sliderVal);
            slicingScript.XZrot = rotationVal;
        }
    }
}
