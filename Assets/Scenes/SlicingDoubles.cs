using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Collections.IEnumerable;
//using Valve.VR.InteractionSystem;
using System;

namespace Valve.VR.InteractionSystem
{
    public class SlicingDoubles : MonoBehaviour
    {
//         private int numberOfDimensions = 4;

//         public GameObject sphere;

//         public List<Vector3> locatons;

//         public int numberOfSpheres = 16;

//         public List<GameObject> spheres;

//         public List<Vector4> all4Dcoords;

//         public double wSlice;

//         public Vector4 testVec4;


//         public LinearMapping linearMapping;
//         private double currentLinearMapping = float.NaN;
//         private int framesUnchanged = 0;

//         //HYPERPLANE INFO
//         ///////////////////////////////////
//         //private float[] unitNormal = new float[4];
//         //private Vector4 unitNormal = new Vector4(0.5f,0.5f,0.5f,0.5f);
//         // private Vector4 unitNormal = new Vector4(0f, 1f, 0f, 0f);
//         // private Vector4 unitNormalParallelToZ = new Vector4(0f, 0f, 0f, 1f);
//         // private Vector4 reflectionParallelToZ = new Vector4(0f, 0f, 0f, -1f);
//         // private double[] unitNormal = new double{0f, 1f, 0f, 0f};
//         // private double[] unitNormalParallelToZ = new double{0f, 0f, 0f, 1f};
//         // private double[] reflectionParallelToZ = new double{0f, 0f, 0f, -1f};
//         private double[] unitNormal = new double[4]{0f, 1f, 0f, 0f};
//         private double[] unitNormalParallelToZ = new double[4]{0f, 0f, 0f, 1f};
//         private double[] reflectionParallelToZ = new double[4]{0f, 0f, 0f, -1f};
        
        
//         public double d = 0.2f;

//         public double r = 1f;


//         /////////////////
//         public SteamVR_Input_Sources leftHand;
//         public SteamVR_Input_Sources rightHand;
//         public SteamVR_Behaviour_Pose controllerPoseL;
//         public SteamVR_Behaviour_Pose controllerPoseR;
//         public SteamVR_Action_Boolean trigger;
//         public SteamVR_Action_Boolean clicked;
//         private bool triggeredL;
//         private bool triggeredR;

//         public Transform planePF;
//         //private Transform xyRotT, yzRotT, zxRotT, ywRotT, xwRotT, zwRotT;
//         private double xyRot, yzRot, zxRot, ywRot, xwRot, zwRot;

//         // Matrix4x4 xyRotMat = new Matrix4x4();
//         // Matrix4x4 yzRotMat = new Matrix4x4();
//         // Matrix4x4 zxRotMat = new Matrix4x4();
//         // Matrix4x4 xwRotMat = new Matrix4x4();
//         // Matrix4x4 ywRotMat = new Matrix4x4();
//         // Matrix4x4 zwRotMat = new Matrix4x4();


//         //public Transform testBall;
//         public Transform pm;
//         private const double halfPi = Mathf.PI/2;
       
//         double[] dragVector = new double[3]{0,0,0}; 

       
//         void Awake()
//         {
//             spheres = new List<GameObject>();
//             wSlice = 0f;
//             all4Dcoords = new List<Vector4>();
//             CalculateCoordinates();

//             //locatons = new List<Vector3> { Vector3.one, Vector3.one * 2f, Vector3.one * 3f };

//             if (linearMapping == null)
//             {
//                 linearMapping = GetComponent<LinearMapping>();
//             }

//             triggeredL = false;
//             triggeredR = false;


//         }

//         // Use this for initialization
//         void Start()
//         {
//             for (int i = 0; i < numberOfSpheres; i++)
//             {
//                 CreateSpheres(i);
//             }

//             // for (int i = 0; i < numberOfSpheres; i++)
//             // {
//             //     Debug.Log("sphere " + i + spheres[i].GetComponent<Info>().Get4DCoords());
//             // }

//             /////////////////test rotations from contoller(s)
//             // //instantiate prefab
//             // xyRotT = Instantiate(planePF, new Vector3(0, 1.5f, 1), Quaternion.identity);
//             // // xArrow.SetParent(transform, false);
//             // yzRotT = Instantiate(planePF, new Vector3(0.5f, 1.5f, 1), Quaternion.identity);
//             // //  yArrow.SetParent(transform);
//             // zxRotT = Instantiate(planePF, new Vector3(1, 1.5f, 1), Quaternion.identity);
//             // //  zArrow.SetParent(transform);
//             // ywRotT = Instantiate(planePF, new Vector3(0, 0.5f, 1), Quaternion.identity);
//             // xwRotT = Instantiate(planePF, new Vector3(0.5f, 0.5f, 1), Quaternion.identity);
//             // zwRotT = Instantiate(planePF, new Vector3(1, 0.5f, 1), Quaternion.identity);



           
//             trigger.AddOnStateDownListener(TriggerDownR, rightHand);
//             trigger.AddOnStateUpListener(TriggerUpR, rightHand);
//             //SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(TriggerPressed, SteamVR_Input_Sources.Any);
//             trigger.AddOnStateDownListener(TriggerDownL, leftHand);
//             trigger.AddOnStateUpListener(TriggerUpL, leftHand);

         
//         // playerScript.Health -= 10.0f;
//        // GameObject pm = Instantiate(positionMarker);
//         testVec4 = new Vector4(1f,0f,0f,0f);
//         //testRotor(testVec4);
//         }

//         // Update is called once per frame
//         void Update()
//         {

//             if (currentLinearMapping != linearMapping.value)
//             {
//                 currentLinearMapping = linearMapping.value;
//                 d = (currentLinearMapping - 0.5f) * 4f;

                

                

//                 ///////////////////////////////////////////////////
//                 for (int i = 0; i < numberOfSpheres; i++)
//                 {
//                     double[] c = spheres[i].GetComponent<InfoDoubles>().Get4DCoords();
//                     double minDis = calcMinDistance(unitNormal, d, c);

//                     //if is in inntersection, get centreOfSlice and sliceRadius:
//                     if (-r <= minDis && minDis <= r)
//                     {
//                         //calculate sliceCentre (4D)
//                         double[] dN = scalarMultiply( minDis , unitNormal);

//                         double[] sliceCentre4D = subtract(c, dN);
//                         //need to convert 4D vectors to 3D local coordinated defined by hyperplane!!
//                         //!!!!!!!!!!!!!!!!!!!!!!!

//                         double[] n = subtract(reflectionParallelToZ , unitNormal);
//                         double[] rotated4D = new double[4];

//                         if (!isVEqualToZero(n))
//                         {
//                             rotated4D = rotateParallelToW(sliceCentre4D, n);
//                         //    Debug.Log(i + ": " + rotated4D);
//                         }
//                         else rotated4D = sliceCentre4D;


//                         //calcualte radius (perpendicular to n)
//                         double sliceRadius = Math.Sqrt(r * r - minDis * minDis);



//                         //need to change below to local coords!
//                         //spheres[i].transform.localPosition = new Vector3(rotated4D.x, rotated4D.y, rotated4D.z);
//                         spheres[i].transform.localScale = Vector3.one * (float)sliceRadius * 2;
//                     }
//                     //else sliceradius = 0;
//                     else { spheres[i].transform.localScale = Vector3.zero; }
//                 }



//             }

//             if (triggeredL)
//             {
//                 // xyRotT.rotation = Quaternion.Euler(0,0,controllerPoseL.transform.eulerAngles.z);
//                 // yzRotT.rotation = Quaternion.Euler(controllerPoseL.transform.eulerAngles.x,0,0);
//                 // zxRotT.rotation = Quaternion.Euler(0,controllerPoseL.transform.eulerAngles.y,0);
//                 // xyRotT.localScale = new Vector3(0.5f,controllerPoseL.transform.eulerAngles.x/100,0.5f);
//                 // zxRotT.localScale = new Vector3(0.5f,controllerPoseL.transform.eulerAngles.y/100,0.5f);
//                 // yzRotT.localScale = new Vector3(0.5f,controllerPoseL.transform.eulerAngles.z/100,0.5f);
//                 // 
//                 //  xyRot = controllerPoseL.transform.eulerAngles.z * Mathf.Deg2Rad;
//                 // zxRot = controllerPoseL.transform.eulerAngles.y * Mathf.Deg2Rad;
//                 // yzRot = controllerPoseL.transform.eulerAngles.x * Mathf.Deg2Rad;
               
//                // testBall.transform.localScale =controllerPoseL.GetVelocity();

//                 xyRot =  controllerPoseL.GetVelocity().z; 
//                 yzRot =  controllerPoseL.GetVelocity().x; 
//                 zxRot =  controllerPoseL.GetVelocity().y;       


                   
            
              
    
//             }
//             if (triggeredR)
//             {
//                 // ywRotT.rotation = Quaternion.Euler(0,0,controllerPoseR.transform.eulerAngles.z);
//                 // xwRotT.rotation = Quaternion.Euler(controllerPoseR.transform.eulerAngles.x,0,0);
//                 // zwRotT.rotation = Quaternion.Euler(0,controllerPoseR.transform.eulerAngles.y,0);
//                 // xwRot = controllerPoseR.transform.eulerAngles.z * Mathf.Deg2Rad;
//                 // ywRot = controllerPoseR.transform.eulerAngles.y * Mathf.Deg2Rad;
//                 // zwRot = controllerPoseR.transform.eulerAngles.x * Mathf.Deg2Rad;  

//                 xwRot =  controllerPoseR.GetVelocity().z; 
//                 ywRot =  controllerPoseR.GetVelocity().x; 
//                 zwRot =  controllerPoseR.GetVelocity().y;                                                                                                                                                    
//             }

//            // rotateUnitNormal();

//             // //need to fix clicked-grabgrip
//             // if (clicked.GetLastStateDown(leftHand)){
//             // Debug.Log("HERE!! " );  
//             // }

//             Debug.Log("pm transform = "+ pm.position );
//             rotateUnitNormal3(unitNormal);

//         }

//         private float roughlyIncreaseRange(float a){
//             return a*7; //makes numbers cover whole 3sphere more easily
//         }

//         //commented out for now since 4x4matrix class requires floats
//         // private void rotateUnitNormal()
//         // {
//         //     //////////////////////////////////////////////////////
//         //     //xy:
//         //     double cos = Math.Cos(xyRot);
//         //     double sin = Math.Sin(xyRot);
//         //     xyRotMat.SetRow(0, new Vector4(cos, sin, 0, 0));
//         //     xyRotMat.SetRow(1, new Vector4(-sin, cos, 0, 0));
//         //     xyRotMat.SetRow(2, new Vector4(0, 0, 1, 0));
//         //     xyRotMat.SetRow(3, new Vector4(0, 0, 0, 1));

//         //     //yz:
//         //     cos = Mathf.Cos(yzRot);
//         //     sin = Mathf.Sin(yzRot);
//         //     yzRotMat.SetRow(0, new Vector4(1, 0, 0, 0));
//         //     yzRotMat.SetRow(1, new Vector4( 0, cos, sin, 0));
//         //     yzRotMat.SetRow(2, new Vector4( 0, -sin, cos, 0));
//         //     yzRotMat.SetRow(3, new Vector4(0, 0, 0, 1));


//         //     //zx:
//         //     cos = Mathf.Cos(zxRot);
//         //     sin = Mathf.Sin(zxRot);
//         //     zxRotMat.SetRow(0, new Vector4(cos, 0, -sin, 0));
//         //     zxRotMat.SetRow(1, new Vector4(0, 1, 0, 0));
//         //     zxRotMat.SetRow(2, new Vector4(sin, 0, cos, 0));
//         //     zxRotMat.SetRow(3, new Vector4(0, 0, 0, 1));

//         //     //xw:
//         //     cos = Mathf.Cos(xwRot);
//         //     sin = Mathf.Sin(xwRot);
//         //     xwRotMat.SetRow(0, new Vector4(cos,0, 0, sin));
//         //     xwRotMat.SetRow(1, new Vector4(0, 1,0, 0));
//         //     xwRotMat.SetRow(2, new Vector4(0, 0, 1, 0));
//         //     xwRotMat.SetRow(3, new Vector4(-sin, 0, 0, cos));

//         //     //yw:
//         //     cos = Mathf.Cos(ywRot);
//         //     sin = Mathf.Sin(ywRot);
//         //     ywRotMat.SetRow(0, new Vector4(1,0,0,0));
//         //     ywRotMat.SetRow(1, new Vector4(0, cos, 0, -sin));
//         //     ywRotMat.SetRow(2, new Vector4(0, 0, 1, 0));
//         //     ywRotMat.SetRow(3, new Vector4(0, sin, 0, cos));

//         //     //zw:
//         //     cos = Mathf.Cos(zwRot);
//         //     sin = Mathf.Sin(zwRot);
//         //     zwRotMat.SetRow(0, new Vector4(1,0,0,0));
//         //     zwRotMat.SetRow(1, new Vector4(0, 1,0, 0));
//         //     zwRotMat.SetRow(2, new Vector4(0, 0, cos, -sin));
//         //     zwRotMat.SetRow(3, new Vector4(0, 0, sin, cos));

//         //     unitNormal = (xyRotMat * yzRotMat * zxRotMat * xwRotMat * ywRotMat * zwRotMat).MultiplyVector(unitNormal);
//         //    // unitNormal.Normalize(); //quick fix for now
//         //  //   Debug.Log("unit normal is now: " + unitNormal + " , it is unitary? " + unitNormal.magnitude );

//         // }

//         private void rotateUnitNormal2(){
//             double a = roughlyIncreaseRange(pm.position.x);
//             double b = roughlyIncreaseRange(pm.position.y);
//             double c = roughlyIncreaseRange(pm.position.z);

//             if(b > halfPi){
//                 //DO SOMETHING! GEOMETRIC INTERPRETATION OF COORDS WOULD HELP HERE
//                 b = halfPi;
//             }
//              if(b < 0){
//                 //DO SOMETHING! GEOMETRIC INTERPRETATION OF COORDS WOULD HELP HERE
//                 b = 0;
//             }
//             // THIS DOES NOT WORK

//             unitNormal[0] = Math.Cos(a)*Math.Sin(b);
//             unitNormal[1] = Math.Sin(a)*Math.Sin(b);
//             unitNormal[2] = Math.Cos(c)*Math.Cos(b);
//             unitNormal[3] = Math.Sin(c)*Math.Sin(b);
            
//             //unitNormal.Normalize();
//             unitNormal = normalise(unitNormal);
//             Debug.Log("unit normal is now: " + unitNormal + " , it is unitary? " + norm(unitNormal) );
//         }

//         //commented out because this method is not commuative/the internet says there are problems
// //         private void rotateUnitNormal3(double[] v)
// //         {

// //             Debug.Log("original v "+ v + " , angle xy: "+xyRot + ", yz: "+ yzRot + ", zx:"+ zxRot + ", xw: "+ xwRot+ ", yw: "+ywRot+ ", zw: "+zwRot);
// //             //////////////////////////////////////////////////////TO DO!
// //             //xy:
// //             double cos = Math.Cos(xyRot);
// //             double sin = Math.Sin(xyRot);
            
// //             v[0] = (cos*v[0]) + (sin*v[1]);
// //             v[1] = (-sin*v[0]) + (cos*v[1]);

// //            v = rotateInPlane(v, 0,1, xyRot);


// //             //yz:
// //             cos = Mathf.Cos(yzRot);
// //             sin = Mathf.Sin(yzRot);
            
// //             v.y = (cos*v.y) + (sin*v.z);
// //             v.z = (-sin*v.y) + (cos*v.z);


// //             //zx:
// //             cos = Mathf.Cos(zxRot);
// //             sin = Mathf.Sin(zxRot);
            
// //             v.z = (cos*v.z) + (sin*v.x);
// //             v.x = (-sin*v.z) + (cos*v.x);

// //             //xw:
// //             cos = Mathf.Cos(xwRot);
// //             sin = Mathf.Sin(xwRot);
            
// //             v.x = (cos*v.x) + sin*v.w;
// //             v.w = cos*v.w - sin*v.x;

// //             //yw:
// //             cos = Mathf.Cos(ywRot);
// //             sin = Mathf.Sin(ywRot);
            
// //             v.y = cos*v.y - sin*v.w;
// //             v.w = sin*v.y + cos*v.w;

// //             //zw:
// //             cos = Mathf.Cos(zwRot);
// //             sin = Mathf.Sin(zwRot);
            
// //             v.z = cos*v.z - sin*v.w;
// //             v.w = cos*v.w + sin*v.z;

// //            //Debug.Log("Final v: " + v);
// //            // unitNormal.Normalize(); //quick fix for now
// //             unitNormal = v;
// //   // unitNormal.Normalize();

// //             Debug.Log("unit normal is now: " + unitNormal + " , it is unitary? " + unitNormal.magnitude );

// //         }
        
//         private void rotateUnitNormalRollingBall(){
            
//         }
        
//         double[] getCoords(GameObject s)
//         {
//             return s.GetComponent<Info>().Get4DCoords();
//         }

//         void CreateSpheres(int which)
//         {
//             GameObject s = Instantiate(sphere);

//             //MAYBE NOT NECESSARY: (NOT RIGHT NOW AT LEAST)
//             //s.transform.parent = this.transform;

//             //getting the coords from big list where put the coords
//             Info sphereInfo = s.GetComponent<Info>();
//             double[] coords = all4Dcoords[which];
//             sphereInfo.setCoords4D(coords);

//             //s.transform.localPosition = new Vector3(coords.x, coords.y, coords.z);
//             //s.transform.localScale = Vector3.one *2;

//             //this will need to be changed for different basis vectors!!!!!!!!!!!!!!!!!!
//             s.transform.localPosition = new Vector3(coords.x, coords.y, coords.z);

//             //if sphere is in subDims, set postion and radius
//             if (isSphereInSubDim(s))
//             {
//                 s.transform.localScale = CalculateDiameter(s);
//             }
//             else { s.transform.localScale = Vector3.zero; }


//             spheres.Add(s);
//         }

//         bool isSphereInSubDim(GameObject s)
//         {
//             float wCoord = s.GetComponent<Info>().Get4DCoords().w;
//             if ((wSlice >= 0 && wSlice <= (wCoord + 1)) ||
//                    (wSlice <= 0 && wSlice >= (wCoord - 1))
//                    )
//             { return true; }
//             else return false;
//         }



//         void SetPosition(GameObject s)
//         {

//         }

//         void CalculateCoordinates()
//         {
//             List<float> numbers = new List<float>();
//             float[] options = { 1f, -1f };
//             for (int i = 0; i < 2; i++)
//             {
//                 for (int j = 0; j < 2; j++)
//                 {
//                     for (int k = 0; k < 2; k++)
//                     {
//                         for (int l = 0; l < 2; l++)
//                         {
//                             numbers.Add(options[i]);
//                             numbers.Add(options[j]);
//                             numbers.Add(options[k]);
//                             numbers.Add(options[l]);
//                         }
//                     }
//                 }
//             }
//             //for (int i = 0; i < numbers.Count; i+=4){
//             //    Debug.Log(numbers[i] + " " + numbers[i + 1] + " " + numbers[i + 2] + " " + numbers[i + 3]);
//             //}

//             for (int i = 0; i < numbers.Count; i += 4)
//             {
//                 Vector4 v = new Vector4(numbers[i], numbers[i + 1], numbers[i + 2], numbers[i + 3]);
//                 all4Dcoords.Add(v);
//             }

//             for (int i = 0; i < all4Dcoords.Count; i++)
//             {
//                 Debug.Log(all4Dcoords[i]);
//             }
//             //Debug.Log("count = " + all4Dcoords.Count);
//         }

//         Vector3 CalculateDiameter(GameObject s)
//         {

//             double wCoord = s.GetComponent<Info>().Get4DCoords()[3];
//             //Debug.Log("w coord =" + wCoord);

//             //update scale (diamter) based on distance of wslice from center of w coordinate
//             double distanceFromCentre = Math.Sqrt(1 - (wSlice - wCoord) * (wSlice - wCoord)); //want it to be smallest at max distancwe
//             return Vector3.one * 2 * distanceFromCentre;
//             //       Debug.Log("diameter = " + s.transform.localScale);


//         }

//         double calcMinDistance(double[] n, double d, double[] c)
//         {
//             double dot = 0;
//             for (int i =0;i<numberOfDimensions;i++){
//                 dot += n[i]*c[i];
//             }
//             double minDist = dot - d;
//             return minDist;
//         }

//         double[] rotateParallelToW(double[] coords4D, double[] n)
//         {
//            // double dots = Vector4.Dot(coords4D, n) / Vector4.Dot(n, n);
//             double dots = dot(coords4D, n) / dot(n,n);

//             //double[] rotatedCoords4D = coords4D - (2 * dots * n);
//             double[] rotatedCoords4D = subtract(coords4D , scalarMultiply(2*dots, n));

//             return rotatedCoords4D;
//         }

//         public void TriggerUpL(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
//         {
//             Debug.Log("Trigger is up L  "+ fromAction +" "+fromSource);
//             triggeredL = false;
            
//         }
//         public void TriggerDownL(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
//         {
//             Debug.Log("Trigger is down L");
//             triggeredL = true;
//         }
//         public void TriggerUpR(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
//         {
//             Debug.Log("Trigger is up Y");
//             triggeredR = false;
//         }
//         public void TriggerDownR(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
//         {
//             Debug.Log("Trigger is down R");
//             triggeredR = true;

//         }
//         // private void TriggerPressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource){}

//         public void testRotor(Vector4 v){
//             // float xyRotH = xyRot/2;
//             // float zxRotH = zxRot/2;
//             // float yzRotH = yzRot/2;
//             // float xwRotH = xwRot/2;
//             // float ywRotH = ywRot/2;
//             // float zwRotH = zwRot/2;

//             // float A12 = Mathf.Cos(xyRotH);
//             // float B12 = Mathf.Sin(xyRotH);

//             // float A13 = Mathf.Cos(zxRotH);
//             // float B13 = Mathf.Sin(zxRotH);

//             // float A23 = Mathf.Cos(yzRotH);
//             // float B23 = Mathf.Sin(yzRotH);

//             // float A14 = Mathf.Cos(xwRotH);
//             // float B14 = Mathf.Sin(xwRotH);

//             // float A24 = Mathf.Cos(ywRotH);
//             // float B24 = Mathf.Sin(ywRotH);

//             // float A34 = Mathf.Cos(zwRotH);
//             // float B34 = Mathf.Sin(zwRotH);

            

//             float A12 = Mathf.Cos(xyRot);
//             float B12 = Mathf.Sin(xyRot);

//             float A13 = Mathf.Cos(zxRot);
//             float B13 = Mathf.Sin(zxRot);

//             float A23 = Mathf.Cos(yzRot);
//             float B23 = Mathf.Sin(yzRot);

//             float A14 = Mathf.Cos(xwRot);
//             float B14 = Mathf.Sin(xwRot);

//             float A24 = Mathf.Cos(ywRot);
//             float B24 = Mathf.Sin(ywRot);

//             float A34 = Mathf.Cos(zwRot);
//             float B34 = Mathf.Sin(zwRot);

            

//         }

//         public double[] scalarMultiply(double s, double[] vec4){
//             for (int i = 0;i<numberOfDimensions;i++){
//                 vec4[i] = s*vec4[i];
//             }
//             return vec4;
//         }

//         //big = what you subtract from, little = what is subtracting, not necessarily big>=little
//         public double[] subtract(double[] big, double[] little){
//             for (int i = 0;i<numberOfDimensions;i++){
//                 big[i] -= little[i];
//             }
//             return big;
//         }

//         public bool isVEqualToZero(double[] v){
//             for(int i=0;i<numberOfDimensions;i++){
//                 if(v[i]!=0){
//                     return false;
//                 }
//             }
//             return true;
//         }

//         double dot(double[] a, double[] b){
//             double d = 0;
//             for (int i =0;i<numberOfDimensions;i++){
//                 d += a[i]*b[i];
//             }
//             return d;
//         }

//         double norm(double[] n){
//             return Math.Sqrt(dot(n,n));
//         }

//         double[] normalise(double[] a){
//             double normA = norm(a);
//             for(int i = 0 ; i < numberOfDimensions;i++){
//                 a[i] /= normA;
//             }
//             return a;
//         }

//         double[] rotateInPlane(double[] vec, int u, int v, double angle){
//             double cos = Math.Cos(angle);
//             double sin = Math.Sin(angle);
            
//             vec[u] = (cos*vec[u]) + (sin*vec[v]);
//             vec[v] = (-sin*vec[u]) + (cos*vec[v]);

//             return vec;


//         }

     }

}