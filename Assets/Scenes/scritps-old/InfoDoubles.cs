using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoDoubles : MonoBehaviour {

    public double[] coords4D = new double[4];

    Vector3 centre3D; //keep this in float land

    double radius;

	
    public void setCoords4D(double[] c4d){
        coords4D = c4d;
    }

    public double[] Get4DCoords(){
        return coords4D;
    }
}
