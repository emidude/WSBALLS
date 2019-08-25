using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Valve.VR.InteractionSystem;

namespace Valve.VR.InteractionSystem
{
    public class SLIDERS_Slicing : MonoBehaviour
    {
        private int numberOfDimensions = 4;
        public GameObject sphere;
        public List<Vector3> locatons;
        public int numberOfSpheres = 16;
        public List<GameObject> spheres;
        public List<Vector4> all4Dcoords;
        public float wSlice;
        public Vector4 testVec4;


        public LinearMapping linearMapping;
        private float currentLinearMapping = float.NaN;
        private int framesUnchanged = 0;

        //HYPERPLANE INFO
        ///////////////////////////////////
        //private float[] unitNormal = new float[4];
        //private Vector4 unitNormal = new Vector4(0.5f,0.5f,0.5f,0.5f);
        private Vector4 unitNormal = new Vector4(1f, 0f, 0f, 0f);
        private Vector4 unitNormalParallelToZ = new Vector4(0f, 0f, 0f, 1f);
        private Vector4 reflectionParallelToZ = new Vector4(0f, 0f, 0f, -1f);
        // private double unitNormal = new double{0f, 1f, 0f, 0f};
        // private double unitNormalParallelToZ = new double{0f, 0f, 0f, 1f};
        // private double reflectionParallelToZ = new double{0f, 0f, 0f, -1f};

        public float d = -1f; //d = distance of hyperplane from origin

        public float r = 1f; //r = radius of balls (some commented out code for accessing in script for potential differing radii
        //or to view for how to access vars from other scripts/objects)


        /////////////////
        public SteamVR_Input_Sources leftHand;
        public SteamVR_Input_Sources rightHand;
        public SteamVR_Behaviour_Pose controllerPoseL;
        public SteamVR_Behaviour_Pose controllerPoseR;
        public SteamVR_Action_Boolean trigger;
        public SteamVR_Action_Boolean clicked;
        private bool triggeredL = false;
        private bool triggeredR = false;

        public Transform planePF;
        //private Transform xyRotT, yzRotT, zxRotT, ywRotT, xwRotT, zwRotT;
        //private float xyRot, yzRot, zxRot, ywRot, xwRot, zwRot;

        Matrix4x4 xyRotMat = new Matrix4x4();
        Matrix4x4 yzRotMat = new Matrix4x4();
        Matrix4x4 zxRotMat = new Matrix4x4();
        Matrix4x4 xwRotMat = new Matrix4x4();
        Matrix4x4 ywRotMat = new Matrix4x4();
        Matrix4x4 zwRotMat = new Matrix4x4();


        //public Transform testBall;
        public Transform pm;
        private const float PI = Mathf.PI;
        private const float halfPi = Mathf.PI / 2;

        Vector3 dragVector = new Vector3(0f, 0f, 0f); //dragging direction for rolling ball matrix

        private Vector4 v0 = new Vector4(0f, 0f, 0f, 1f); //to multiply with rolling ball matrix

        float[,] identityMatrix;

        private Player player = null;

        public Vector3 startingPosition;
        public Transform startingPositionPublicObject;

        public float XYrot;
        float currentXYrot;
        public float XZrot;
        float currentXZrot;
        public float XWrot;
        float currentXWrot;
        public float YZrot;
        float currentYZrot;
        public float YWrot;
        float currentYWrot;
        public float ZWrot;
        float currentZWrot;

        //bool needToResetPositionL = true; //reset position when let go of trigger
        //bool needToResetPositionR = true;

        public int numberOfDs = 2;
        float dIncrement = 0.2f; //can change this later
        //public GameObject sliceOfD;
        //public List<GameObject> slicesOfD;
        float fakeEyeDistance = 3f;

        public Vector3 coordTransform;
        public Vector3 currentCoordTransform;

        void Awake()
        {
            spheres = new List<GameObject>();
            wSlice = 0f;
            all4Dcoords = new List<Vector4>();
            CalculateCoordinates();

            //locatons = new List<Vector3> { Vector3.one, Vector3.one * 2f, Vector3.one * 3f };

            if (linearMapping == null)
            {
                linearMapping = GetComponent<LinearMapping>();
            }

            triggeredL = false;
            triggeredR = false;

            //id mat used in rotation formula between vectors
            identityMatrix = createIdentityMatrix(numberOfDimensions);
            numberOfDs = 1;
            
        }

        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < numberOfSpheres; i++)
            {
                CreateSpheres(i);
            }

      
            ////////////////////////////////////////////////////////////////////////////////////////////////
            //trigger.AddOnStateDownListener(TriggerDownR, rightHand);
            //trigger.AddOnStateUpListener(TriggerUpR, rightHand);
            ////SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(TriggerPressed, SteamVR_Input_Sources.Any);
            //trigger.AddOnStateDownListener(TriggerDownL, leftHand);
            //trigger.AddOnStateUpListener(TriggerUpL, leftHand);
            ///////////////////////////////////////////////////////////////////////////////////////////////

            //GameObject pm = Instantiate(positionMarker);
            testVec4 = new Vector4(1f, 0f, 0f, 0f);
            //testRotor(testVec4);

            player = InteractionSystem.Player.instance;
            if (player == null)
            {
                Debug.LogError("<b>[SteamVR Interaction]</b> Slicing: No Player instance found in map.");
                Destroy(this.gameObject);
                return;
            }

          
        }


        // Update is called once per frame
        void Update()
        {
            //////////////////////////////////////////////////////////////
            updateRotations();
            
            updateD();


            //////////////////////////////////////////////////////////////////////////////////////////////////////////


            //Debug.Log("unitNormal=" + unitNormal);
            ///////////////////////////////////////////////////

            Vector4 n = reflectionParallelToZ - unitNormal; //this is used in the following (nested) loop to map 4d points on plane
                                                            //to plane rotated parallel to z (actually w) axis
            for (int i = 0; i < numberOfSpheres; i++)
            {
                //Info sphereInfo = spheres[i].GetComponent<Info>();
                //float r = spheres[i].GetComponent<Info>().radius; //Debug.Log("r = " + r);
                Vector4 c = spheres[i].GetComponent<Info>().Get4DCoords();

                for  (int j = 0; j < numberOfDs; j++)
                {
                    float dSlice = d + (j * dIncrement);
                    //Debug.Log("dSlice = " + dSlice);
                    float minDis = calcMinDistance(unitNormal, dSlice, c); //this projects c onto n (n.c) and
                                                                      //caluclates min distance between this and the hyperplane

                    //if min distance (c.n - d ) is within radius r of sphere, some of sphere c intersects hyperplane  
                    //if is in inntersection, get centreOfSlice and sliceRadius:
                    if (-r <= minDis && minDis <= r)
                    {
                        //calculate sliceCentre (4D)
                        Vector4 sliceCentre4D = c - minDis * unitNormal;
                        //testing - check if this lies on the hyperplane:
                        if (!isOnHyperPlane(sliceCentre4D, unitNormal, dSlice))
                        {
                            Debug.Log("NOT ON HYPERPLANE!" + " c = " + c + "slice centre:" + sliceCentre4D + " dSlice= " + dSlice + " minDis = " + minDis + "unitNormal = " + unitNormal);
                        }
                        else
                        {
                          //  Debug.Log("ON HYPERPLANE!");
                        }

                        
                        Vector4 rotated4D;

                        if (n != Vector4.zero)
                        {
                            rotated4D = rotateParallelToW(sliceCentre4D, n);
                        }
                        else { rotated4D = sliceCentre4D; }
                        // Debug.Log(i + ": " + rotated4D);


                        //calcualte radius (perpendicular to n)
                        float sliceRadius = Mathf.Sqrt(r * r - minDis * minDis);
                        //Debug.Log(i + " slice radius = " + sliceRadius);


                        //the z coord is now constant for all slice coords so can place in 3d vr space
                        //spheres[i].transform.localPosition = new Vector3(rotated4D.x, rotated4D.y, rotated4D.z);
                        //spheres[i].transform.localScale = Vector3.one * sliceRadius * 2;

                        //adjust further out slices as projected smaller
                       // Debug.Log("sphre " + i + ", slice " + j + " : " + rotated4D);
                        
                        
                        //float dividingAmount = 1 / (fakeEyeDistance + dSlice); //this makes many intersecting balls for d /= -2
                        //float dividingAmount = d / dSlice; //terrible  -end up dividing by 0;
                        //float dividingAmount = 1 / ((j +1) * dIncrement);
                        //rotated4D.x *= dividingAmount;
                        //rotated4D.y *= dividingAmount;
                        //rotated4D.z *= dividingAmount; //this does not work well

                        spheres[i].GetComponent<Info>().slicesOfD[j].transform.localPosition = new Vector3(rotated4D.x, rotated4D.y, rotated4D.z);
                        spheres[i].GetComponent<Info>().slicesOfD[j].transform.localScale = Vector3.one * sliceRadius * 2; // * dividingAmount;

                    }
                    //else sliceradius = 0;
                    else { //spheres[i].transform.localScale = Vector3.zero; 
                        spheres[i].GetComponent<Info>().slicesOfD[j].transform.localScale = Vector3.zero;
                    }
                }
            }





            if (triggeredL)
            {
                // xyRotT.rotation = Quaternion.Euler(0,0,controllerPoseL.transform.eulerAngles.z);
                // yzRotT.rotation = Quaternion.Euler(controllerPoseL.transform.eulerAngles.x,0,0);
                // zxRotT.rotation = Quaternion.Euler(0,controllerPoseL.transform.eulerAngles.y,0);
                // xyRotT.localScale = new Vector3(0.5f,controllerPoseL.transform.eulerAngles.x/100,0.5f);
                // zxRotT.localScale = new Vector3(0.5f,controllerPoseL.transform.eulerAngles.y/100,0.5f);
                // yzRotT.localScale = new Vector3(0.5f,controllerPoseL.transform.eulerAngles.z/100,0.5f);
                // 
                //  xyRot = controllerPoseL.transform.eulerAngles.z * Mathf.Deg2Rad;
                // zxRot = controllerPoseL.transform.eulerAngles.y * Mathf.Deg2Rad;
                // yzRot = controllerPoseL.transform.eulerAngles.x * Mathf.Deg2Rad;

                // testBall.transform.localScale =controllerPoseL.GetVelocity();

                //this works to get velcoty:
                //xyRot =  controllerPoseL.transform.position.z; 
                //yzRot =  controllerPoseL.GetVelocity().x; 
                //zxRot =  controllerPoseL.GetVelocity().y;          

                //    dragVector = controllerPoseL.GetVelocity();

                //update Unit Normal:
                //Debug.Log("velocity is: " + controllerPoseL.transform.position);
                // updateUnitNormal(controllerPoseL.GetVelocity());

                XYrot = controllerPoseL.transform.position.z;
                YZrot = controllerPoseL.transform.position.x;
                XZrot = controllerPoseL.transform.position.y;

            }
            if (triggeredR)
            {
                // ywRotT.rotation = Quaternion.Euler(0,0,controllerPoseR.transform.eulerAngles.z);
                // xwRotT.rotation = Quaternion.Euler(controllerPoseR.transform.eulerAngles.x,0,0);
                // zwRotT.rotation = Quaternion.Euler(0,controllerPoseR.transform.eulerAngles.y,0);
                // xwRot = controllerPoseR.transform.eulerAngles.z * Mathf.Deg2Rad;
                // ywRot = controllerPoseR.transform.eulerAngles.y * Mathf.Deg2Rad;
                // zwRot = controllerPoseR.transform.eulerAngles.x * Mathf.Deg2Rad;  

                //this works to get velocity
                //xyRot =  controllerPoseR.GetVelocity().z; 
                //ywRot =  controllerPoseR.GetVelocity().x; 
                //zwRot =  controllerPoseR.GetVelocity().y;      

                //    dragVector = controllerPoseR.GetVelocity(); 
                // updateUnitNormal(controllerPoseR.GetVelocity());      

                YWrot = controllerPoseR.transform.position.z;
                XWrot = controllerPoseR.transform.position.y;
                ZWrot = controllerPoseR.transform.position.x;

            }

            //UPDATE UNIT NORMAL:

            //unitNormal = rotateRollingBall();


            //rotateUnitNormal2();

            // //need to fix clicked-grabgrip
            // if (clicked.GetLastStateDown(leftHand)){
            // Debug.Log("HERE!! " );  
            // }

            //Debug.Log("pm transform = "+ pm.position );
            //rotateUnitNormal3(unitNormal);
            // rotateUnitNormal2();

        }

       

        void updateRotations() {
            if (currentXYrot != XYrot)
            {
                Debug.Log(XYrot);
                float increment = currentXYrot - XYrot;
                currentXYrot = XYrot;
                rotateXY(increment);
            }
            if (currentXZrot != XZrot)
            {
                float increment = currentXZrot - XZrot;
                currentXZrot = XZrot;
                rotateXZ(increment);
            }
            if (currentXWrot != XWrot)
            {
                float increment = currentXWrot - XWrot;
                currentXWrot = XWrot;
                rotateXW(increment);
            }
            if (currentYZrot != YZrot)
            {
                float increment = currentYZrot - YZrot;
                currentYZrot = YZrot;
                rotateYZ(increment);
            }
            if (currentYWrot != YWrot)
            {
                float increment = currentYWrot - YWrot;
                currentYWrot = YWrot;
                rotateYW(increment);
            }
            if (currentZWrot != ZWrot)
            {
                float increment = currentZWrot - ZWrot;
                currentZWrot = ZWrot;
                rotateZW(increment);
            }
        }
        void updateD() {
            //update linear mapping from slider, current range: -2,2
            if (currentLinearMapping != linearMapping.value)
            {
                currentLinearMapping = linearMapping.value;
                d = (currentLinearMapping - 0.5f) * 4f;
            }
        }
        
        //Matrix4x4 rotateRollingBallMatrix;
        float[,] rotateRollingBallMatrix = new float[4, 4];
        private Vector4 rotateRollingBall()
        {
            float R = 1; //rolling ball radius = 1
            float rDrag = dragVector.magnitude; //amount of drag

            float D = Mathf.Sqrt((R * R) + (rDrag * rDrag)); //hypotenuse between r and R
            //cos = R/hpotenuseD, sin = r/D

            if (D == 0)
            {
                Debug.Log("D = 0, something went wrong");
                return Vector4.zero;
            }

            float cos = R / D;
            float sin = rDrag / D;
            float oneMinusCos = 1 - cos;
            //Debug.Log("cos = " + cos );

            Vector3 dragNormal = dragVector.normalized;

            //Debug.Log("rDrag = " + rDrag);
            // Debug.Log("test dragNormal =1? :" + (dragNormal.x*dragNormal.x + dragNormal.y*dragNormal.y + dragNormal.z*dragNormal.z));
            // Debug.Log("x = r*n_x " + rDrag*dragNormal.x + "= " + dragVector.x);
            // Debug.Log("y = r*n_y " + rDrag*dragNormal.y + "= " + dragVector.y);
            // Debug.Log("z = r*n_z " + rDrag*dragNormal.z + "= " + dragVector.z);
            float nx = dragNormal.x;
            float ny = dragNormal.y;
            float nz = dragNormal.z;

            // rotateRollingBallMatrix.SetRow(0, new Vector4(
            //     (1 - (nx*nx*oneMinusCos)), (-oneMinusCos*nx*ny),(-oneMinusCos*nx*nz), sin*nx));
            // rotateRollingBallMatrix.SetRow(1, new Vector4(
            //     (-oneMinusCos*nx*ny), (1 - (ny*ny*oneMinusCos)), (-oneMinusCos*ny*nz),sin*ny));
            // rotateRollingBallMatrix.SetRow(2, new Vector4(
            //     (-oneMinusCos*nx*nz), (-oneMinusCos*ny*nz), (1-(nz*nz*oneMinusCos)), sin*nz));
            // rotateRollingBallMatrix.SetRow(3, new Vector4(-sin*nx, -sin*ny, -sin*nz, cos));

            rotateRollingBallMatrix[0, 0] = 1 - (nx * nx * oneMinusCos);
            rotateRollingBallMatrix[0, 1] = -oneMinusCos * nx * ny;
            rotateRollingBallMatrix[0, 2] = -oneMinusCos * nx * nz;
            rotateRollingBallMatrix[0, 3] = sin * nx;

            rotateRollingBallMatrix[1, 0] = -oneMinusCos * nx * ny;
            rotateRollingBallMatrix[1, 1] = 1 - (ny * ny * oneMinusCos);
            rotateRollingBallMatrix[1, 2] = -oneMinusCos * ny * nz;
            rotateRollingBallMatrix[1, 3] = sin * ny;

            rotateRollingBallMatrix[2, 0] = -oneMinusCos * nx * nz;
            rotateRollingBallMatrix[2, 1] = -oneMinusCos * ny * nz;
            rotateRollingBallMatrix[2, 2] = 1 - (nz * nz * oneMinusCos);
            rotateRollingBallMatrix[2, 3] = sin * nz;

            rotateRollingBallMatrix[3, 0] = -sin * nx;
            rotateRollingBallMatrix[3, 1] = -sin * ny;
            rotateRollingBallMatrix[3, 2] = -sin * nz;
            rotateRollingBallMatrix[3, 3] = cos;

            Vector4 newV = multiplyMatrixVector(rotateRollingBallMatrix, v0);
            //Vector4 newV = multiplyMatrixVector (rotateRollingBallMatrix, unitNormal); 
            return newV;
            //Debug.Log("test rolling Mv = newV = " + newV + "norm? " + newV.magnitude);



        }

        private float[,] getRotationMatrixFromV1toV2(Vector4 v1, Vector4 v2)
        {
            //using method from here: https://math.stackexchange.com/questions/180418/calculate-rotation-matrix-to-align-vector-a-to-vector-b-in-3d/2161631#2161631
            //background: https://math.stackexchange.com/questions/432057/how-can-i-calculate-a-4-times-4-rotation-matrix-to-match-a-4d-direction-vector/2161406#2161406
            //v1, v2 must be same length
            v1.Normalize();
            v2.Normalize();

            //N = length(u); gets number of dims of vector. 
            //Here we know it is numberOfDims = 4 and using identityMatrix created in Awake

            float[,] S = reflectMatrix(identityMatrix, v1 + v2);
            return reflectMatrix(S, v2);
        }

        //reflect u on hyperplane h:
        float[,] reflectMatrix(float[,] u, Vector4 h)
        {

            Vector4 hTu = multiplyVectorMatrix(h, u);
            hTu *= 2;
            if (h == Vector4.zero) { Debug.Log("h = v1 + v2 = 0!!"); }
            hTu /= Vector4.Dot(h, h);

            return matrixSubtract(u, outerProduct(h, hTu));

        }

        // double[] multiplyMatrixVector(double[,] mat, double[] vec){
        //     double[] newVec = new double[4]{0,0,0,0};
        //     if (mat.GetLength(1) != vec.Length){
        //         Debug.Log("matrix and vector lengths do not add up to multiply!");
        //         return newVec; 
        //     }
        //     for(int i = 0 ;i< numberOfDimensions; i++){
        //         for(int j = 0; j < numberOfDimensions; j++){
        //             newVec[i] += mat[i,j]*vec[j];
        //         }
        //     }
        //     Debug.Log("is the new vector unit -shouldbe: "+ newVec.magnitude)
        //     return newVec;
        // }

        //vT*M
        Vector4 multiplyVectorMatrix(Vector4 v, float[,] mat)
        {
            Vector4 newVec = Vector4.zero;
            for (int i = 0; i < numberOfDimensions; i++)
            {
                for (int j = 0; j < numberOfDimensions; j++)
                {
                    newVec[i] += v[j] * mat[j, i];
                }
            }
            return newVec;
        }
        Vector4 multiplyMatrixVector(float[,] mat, Vector4 vec)
        {
            Vector4 newVec = new Vector4(0f, 0f, 0f, 0f);
            if (mat.GetLength(1) != 4)
            {
                Debug.Log("matrix and vector lengths do not add up to multiply!");
                return newVec;
            }
            for (int i = 0; i < numberOfDimensions; i++)
            {
                for (int j = 0; j < numberOfDimensions; j++)
                {
                    newVec[i] += (float)mat[i, j] * vec[j];
                }
            }
            //Debug.Log("is the new vector = magOfOriginalVector =1 ?="+ newVec.magnitude);
            return newVec;
        }

        //updatesTheUnitNormal based on Rolling ball technique
        void updateUnitNormal(Vector3 velocity)
        {
            //Debug.Log("rolling ball updating normal with reflection method");
            dragVector = velocity;
            //rotate with rolling ball
            Vector4 vecTo = rotateRollingBall();
            //vecFrom = v0; vecTo = vector output from rolling ball
            //Debug.Log("vecTO = " + vecTo);

            //get rotation matrix from vecFrom to vecTo
            float[,] rotationMatrixToUpdateNormal = getRotationMatrixFromV1toV2(v0, vecTo);
            //Debug.Log("rotation matrix is + " +rotationMatrixToUpdateNormal);

            unitNormal = multiplyMatrixVector(rotationMatrixToUpdateNormal, unitNormal);
            //Debug.Log("unit normal is now "+ unitNormal + " is it normal? 1 =?" + unitNormal.magnitude );
        }

        private float roughlyIncreaseRange(float a)
        {
            return a * 7; //makes numbers cover whole 3sphere more easily
        }
        //private void rotateUnitNormal()
        //{
        //    Debug.Log("rotating Unit Normal 1");
        //    //////////////////////////////////////////////////////TO DO!
        //    //xy:
        //    float cos = Mathf.Cos(xyRot);
        //    float sin = Mathf.Sin(xyRot);
        //    xyRotMat.SetRow(0, new Vector4(cos, sin, 0, 0));
        //    xyRotMat.SetRow(1, new Vector4(-sin, cos, 0, 0));
        //    xyRotMat.SetRow(2, new Vector4(0, 0, 1, 0));
        //    xyRotMat.SetRow(3, new Vector4(0, 0, 0, 1));

        //    //yz:
        //    cos = Mathf.Cos(yzRot);
        //    sin = Mathf.Sin(yzRot);
        //    yzRotMat.SetRow(0, new Vector4(1, 0, 0, 0));
        //    yzRotMat.SetRow(1, new Vector4( 0, cos, sin, 0));
        //    yzRotMat.SetRow(2, new Vector4( 0, -sin, cos, 0));
        //    yzRotMat.SetRow(3, new Vector4(0, 0, 0, 1));


        //    //zx:
        //    cos = Mathf.Cos(zxRot);
        //    sin = Mathf.Sin(zxRot);
        //    zxRotMat.SetRow(0, new Vector4(cos, 0, -sin, 0));
        //    zxRotMat.SetRow(1, new Vector4(0, 1, 0, 0));
        //    zxRotMat.SetRow(2, new Vector4(sin, 0, cos, 0));
        //    zxRotMat.SetRow(3, new Vector4(0, 0, 0, 1));

        //    //xw:
        //    cos = Mathf.Cos(xwRot);
        //    sin = Mathf.Sin(xwRot);
        //    xwRotMat.SetRow(0, new Vector4(cos,0, 0, sin));
        //    xwRotMat.SetRow(1, new Vector4(0, 1,0, 0));
        //    xwRotMat.SetRow(2, new Vector4(0, 0, 1, 0));
        //    xwRotMat.SetRow(3, new Vector4(-sin, 0, 0, cos));

        //    //yw:
        //    cos = Mathf.Cos(ywRot);
        //    sin = Mathf.Sin(ywRot);
        //    ywRotMat.SetRow(0, new Vector4(1,0,0,0));
        //    ywRotMat.SetRow(1, new Vector4(0, cos, 0, -sin));
        //    ywRotMat.SetRow(2, new Vector4(0, 0, 1, 0));
        //    ywRotMat.SetRow(3, new Vector4(0, sin, 0, cos));

        //    //zw:
        //    cos = Mathf.Cos(zwRot);
        //    sin = Mathf.Sin(zwRot);
        //    zwRotMat.SetRow(0, new Vector4(1,0,0,0));
        //    zwRotMat.SetRow(1, new Vector4(0, 1,0, 0));
        //    zwRotMat.SetRow(2, new Vector4(0, 0, cos, -sin));
        //    zwRotMat.SetRow(3, new Vector4(0, 0, sin, cos));

        //    unitNormal = (xyRotMat * yzRotMat * zxRotMat * xwRotMat * ywRotMat * zwRotMat).MultiplyVector(unitNormal);
        //   // unitNormal.Normalize(); //quick fix for now
        // //   Debug.Log("unit normal is now: " + unitNormal + " , it is unitary? " + unitNormal.magnitude );

        //}

        private void rotateUnitNormal2()
        {
            Debug.Log("rotating Unit Normal 2");
            float a = roughlyIncreaseRange(pm.position.x);
            float b = roughlyIncreaseRange(pm.position.y);
            float c = roughlyIncreaseRange(pm.position.z);

            // if(b > halfPi){
            //     //DO SOMETHING! GEOMETRIC INTERPRETATION OF COORDS WOULD HELP HERE
            //     b = halfPi;
            // }
            //  if(b < 0){
            //     //DO SOMETHING! GEOMETRIC INTERPRETATION OF COORDS WOULD HELP HERE
            //     b = 0;
            // }
            // THIS DOES NOT WORK

            unitNormal.x = Mathf.Cos(a) * Mathf.Sin(b);
            unitNormal.y = Mathf.Sin(a) * Mathf.Sin(b);
            unitNormal.z = Mathf.Cos(c) * Mathf.Cos(b);
            unitNormal.w = Mathf.Sin(c) * Mathf.Sin(b);

            //unitNormal.Normalize();
            Debug.Log("unit normal is now: " + unitNormal + " , it is unitary? " + unitNormal.magnitude);
        }

        //      private void rotateUnitNormal3(Vector4 v)
        //      {
        //          Debug.Log("rotating Unit Normal 3");

        //          Debug.Log("original v "+ v + " , angle xy: "+xyRot + ", yz: "+ yzRot + ", zx:"+ zxRot + ", xw: "+ xwRot+ ", yw: "+ywRot+ ", zw: "+zwRot);
        //          //////////////////////////////////////////////////////TO DO!
        //          //xy:
        //          float cos = Mathf.Cos(xyRot);
        //          float sin = Mathf.Sin(xyRot);

        //          v.x = (cos*v.x) + (sin*v.y);
        //          v.y = (-sin*v.x) + (cos*v.y);


        //          //yz:
        //          cos = Mathf.Cos(yzRot);
        //          sin = Mathf.Sin(yzRot);

        //          v.y = (cos*v.y) + (sin*v.z);
        //          v.z = (-sin*v.y) + (cos*v.z);


        //          //zx:
        //          cos = Mathf.Cos(zxRot);
        //          sin = Mathf.Sin(zxRot);

        //          v.z = (cos*v.z) + (sin*v.x);
        //          v.x = (-sin*v.z) + (cos*v.x);

        //          //xw:
        //          cos = Mathf.Cos(xwRot);
        //          sin = Mathf.Sin(xwRot);

        //          v.x = (cos*v.x) + sin*v.w;
        //          v.w = cos*v.w - sin*v.x;

        //          //yw:
        //          cos = Mathf.Cos(ywRot);
        //          sin = Mathf.Sin(ywRot);

        //          v.y = cos*v.y - sin*v.w;
        //          v.w = sin*v.y + cos*v.w;

        //          //zw:
        //          cos = Mathf.Cos(zwRot);
        //          sin = Mathf.Sin(zwRot);

        //          v.z = cos*v.z - sin*v.w;
        //          v.w = cos*v.w + sin*v.z;

        //         //Debug.Log("Final v: " + v);
        //         // unitNormal.Normalize(); //quick fix for now
        //          unitNormal = v;
        //// unitNormal.Normalize();

        //          Debug.Log("unit normal is now: " + unitNormal + " , it is unitary? " + unitNormal.magnitude );

        //      }

        private void rotateXY(float xyRot)
        {
            float cos = Mathf.Cos(xyRot);
            float sin = Mathf.Sin(xyRot);
            //xyRotMat.SetRow(0, new Vector4(cos, sin, 0, 0));
            //xyRotMat.SetRow(1, new Vector4(-sin, cos, 0, 0));
            //xyRotMat.SetRow(2, new Vector4(0, 0, 1, 0));
            //xyRotMat.SetRow(3, new Vector4(0, 0, 0, 1));

            Vector4 oldUnitNormal = unitNormal;
            unitNormal.x = (cos * oldUnitNormal.x) + (sin * oldUnitNormal.y); //does not work because y below is updated with already updated x, should be previous x
            unitNormal.y = (-sin * oldUnitNormal.x) + (cos * oldUnitNormal.y);

            //Debug.Log("unitNormal before" + unitNormal);

            //unitNormal = xyRotMat.MultiplyVector(unitNormal); //terrible returns vector0

            //Debug.Log("XY " + "unitNormal " + unitNormal + "is unit?: " + unitNormal.magnitude);
            if (!isUnitNormalUNITARY(unitNormal))
            {
                Debug.Log("HERE IS BAD: " + oldUnitNormal + " TO " + unitNormal);
            }
            unitNormal.Normalize();

        }

        //% Implementation of the Aguilera-Perez Algorithm.
        //% Aguilera, Antonio, and Ricardo Pérez-Aguila. "General n-dimensional rotations." (2004).
        //function M = rotmnd(v, theta)
        //    n = size(v,1);
        //        M = eye(n);
        //    for c = 1:(n-2)
        //        for r = n:-1:(c+1)
        //            t = atan2(v(r, c), v(r-1, c));
        //            R = eye(n);
        //        R([r r-1], [r r-1]) = [cos(t) -sin(t); sin(t) cos(t)];
        //            v = R* v;
        //        M = R* M;
        //        end
        //    end
        //    R = eye(n);
        //        R([n-1 n], [n-1 n]) = [cos(theta) -sin(theta); sin(theta) cos(theta)];
        //    M = M\R* M;

        float[,] Aguilera_Perez(float[,] v, float theta)
        {
            //v = axis plane of rotation? v is n-2 subbasis of n

            //need to change all index entires to be -1 what they currently are - switch from matlab to c# array
            int n = numberOfDimensions;
            float[,] M = identityMatrix;
            for (int c = 0; c < (n - 2); c++)
            {
                for (int r = n - 1; r >= (c + 1); r--)
                {
                    Debug.Log("r:c:v: " + r + " " + c + " " + v);
                    float t = Mathf.Atan2(v[r, c], v[r - 1, c]);
                    float[,] R = identityMatrix;
                    R[r, r] = Mathf.Cos(t);
                    R[r, r - 1] = -Mathf.Sin(t);
                    R[r - 1, r] = Mathf.Sin(t);
                    R[r - 1, r - 1] = Mathf.Cos(t);
                    v = matrixMultiply(R, v);
                    M = matrixMultiply(R, M);
                }
            }
            float[,] R2 = identityMatrix;
            Debug.Log("R length " + R2.GetLength(0) + " " + R2.GetLength(1));
            Debug.Log("n = " + n);
            R2[(n - 2), (n - 2)] = Mathf.Cos(theta);
            R2[(n - 2), n - 1] = -(Mathf.Sin(theta));
            R2[n - 1, (n - 2)] = Mathf.Sin(theta);
            R2[n - 1, n - 1] = Mathf.Cos(theta);

            //convert to Matrix4x4 to compute inverse
            Matrix4x4 R3 = turnFloatArrayIntoMatrix4x4(R2);
            Matrix4x4 R4 = R3.inverse;
            float[,] Rinv = turnMatrixIntoFloatArrray(R4);
            //M = M\R* M;
            M = matrixMultiply(R2, M);
            M = matrixMultiply(Rinv, M);
            return M;
        }


        private void rotateYZ(float yzRot)
        {
            Vector4 oldUnitNormal = unitNormal;
            float cos = Mathf.Cos(yzRot);
            float sin = Mathf.Sin(yzRot);
            //yzRotMat.SetRow(0, new Vector4(1, 0, 0, 0));
            //yzRotMat.SetRow(1, new Vector4(0, cos, sin, 0));
            //yzRotMat.SetRow(2, new Vector4(0, -sin, cos, 0));
            //yzRotMat.SetRow(3, new Vector4(0, 0, 0, 1));
            //unitNormal = yzRotMat.MultiplyVector(unitNormal);

            unitNormal.y = (cos * oldUnitNormal.y) + (sin * oldUnitNormal.z);
            unitNormal.z = (-sin * oldUnitNormal.y) + (cos * oldUnitNormal.z);
            //Debug.Log("YZ " + "unitNormal " + unitNormal + "is unit?: " + unitNormal.magnitude);
            if (!isUnitNormalUNITARY(unitNormal))
            {
                Debug.Log("HERE IS BAD: " + oldUnitNormal + " TO " + unitNormal);
            }
            unitNormal.Normalize();
        }

        private void rotateXZ(float xzRot)
        {
            Vector4 oldUnitNormal = unitNormal;
            float cos = Mathf.Cos(xzRot);
            float sin = Mathf.Sin(xzRot);
            //zxRotMat.SetRow(0, new Vector4(cos, 0, -sin, 0));
            //zxRotMat.SetRow(1, new Vector4(0, 1, 0, 0));
            //zxRotMat.SetRow(2, new Vector4(sin, 0, cos, 0));
            //zxRotMat.SetRow(3, new Vector4(0, 0, 0, 1));
            //unitNormal = zxRotMat.MultiplyVector(unitNormal);
            unitNormal.z = (cos * oldUnitNormal.z) + (sin * oldUnitNormal.x);
            unitNormal.x = (-sin * oldUnitNormal.z) + (cos * oldUnitNormal.x);
            //Debug.Log("XZ " + "unitNormal " + unitNormal + "is unit?: " + unitNormal.magnitude);
            if (!isUnitNormalUNITARY(unitNormal))
            {
                Debug.Log("HERE IS BAD: " + oldUnitNormal + " TO " + unitNormal);
            }
            unitNormal.Normalize();
        }
        private void rotateXW(float xwRot)
        {
            Vector4 oldUnitNormal = unitNormal;
            float cos = Mathf.Cos(xwRot);
            float sin = Mathf.Sin(xwRot);
            //xwRotMat.SetRow(0, new Vector4(cos, 0, 0, sin));
            //xwRotMat.SetRow(1, new Vector4(0, 1, 0, 0));
            //xwRotMat.SetRow(2, new Vector4(0, 0, 1, 0));
            //xwRotMat.SetRow(3, new Vector4(-sin, 0, 0, cos));
            //unitNormal = xwRotMat.MultiplyVector(unitNormal);
            unitNormal.x = (cos * oldUnitNormal.x) + sin * oldUnitNormal.w;
            unitNormal.w = cos * oldUnitNormal.w - sin * oldUnitNormal.x;
            //Debug.Log("XW " + "unitNormal " + unitNormal + "is unit?: " + unitNormal.magnitude);
            if (!isUnitNormalUNITARY(unitNormal))
            {
                Debug.Log("HERE IS BAD: " + oldUnitNormal + " TO " + unitNormal);
            }
            unitNormal.Normalize();
        }
        private void rotateYW(float ywRot)
        {
            Vector4 oldUnitNormal = unitNormal;
            float cos = Mathf.Cos(ywRot);
            float sin = Mathf.Sin(ywRot);
            //ywRotMat.SetRow(0, new Vector4(1, 0, 0, 0));
            //ywRotMat.SetRow(1, new Vector4(0, cos, 0, -sin));
            //ywRotMat.SetRow(2, new Vector4(0, 0, 1, 0));
            //ywRotMat.SetRow(3, new Vector4(0, sin, 0, cos));
            //unitNormal = ywRotMat.MultiplyVector(unitNormal);
            unitNormal.y = cos * oldUnitNormal.y - sin * oldUnitNormal.w;
            unitNormal.w = sin * oldUnitNormal.y + cos * oldUnitNormal.w;
            //Debug.Log("YW " + "unitNormal " + unitNormal + "is unit?: " + unitNormal.magnitude);
            if (!isUnitNormalUNITARY(unitNormal))
            {
                Debug.Log("HERE IS BAD: " + oldUnitNormal + " TO " + unitNormal);
                unitNormal.Normalize();
            }

        }
        private void rotateZW(float zwRot)
        {
            Vector4 oldUnitNormal = unitNormal;
            float cos = Mathf.Cos(zwRot);
            float sin = Mathf.Sin(zwRot);
            //zwRotMat.SetRow(0, new Vector4(1, 0, 0, 0));
            //zwRotMat.SetRow(1, new Vector4(0, 1, 0, 0));
            //zwRotMat.SetRow(2, new Vector4(0, 0, cos, -sin));
            //zwRotMat.SetRow(3, new Vector4(0, 0, sin, cos));
            //unitNormal = zwRotMat.MultiplyVector(unitNormal);
            unitNormal.z = cos * oldUnitNormal.z - sin * oldUnitNormal.w;
            unitNormal.w = cos * oldUnitNormal.w + sin * oldUnitNormal.z;
            //Debug.Log("ZW" + " unitNormal " + unitNormal + "is unit?: " + unitNormal.magnitude);
            if (!isUnitNormalUNITARY(unitNormal))
            {
                Debug.Log("HERE IS BAD: " + oldUnitNormal + " TO " + unitNormal);
                unitNormal.Normalize();
            }


        }

        Vector4 getCoords(GameObject s)
        {
            return s.GetComponent<Info>().Get4DCoords();
        }

        void CreateSpheres(int which)
        {
            GameObject s = Instantiate(sphere);

            //MAYBE NOT NECESSARY: (NOT RIGHT NOW AT LEAST)
            //s.transform.parent = this.transform;

            Info sphereInfo = s.GetComponent<Info>();
            Vector4 coords = all4Dcoords[which];
            sphereInfo.setCoords4D(coords);
            sphereInfo.totalNumberOfSpheres = numberOfSpheres;
            sphereInfo.setUniqueColorIdentifier(which);
            sphereInfo.numberOfDs = numberOfDs;
            sphereInfo.createSlices();



            //s.transform.localPosition = new Vector3(coords.x, coords.y, coords.z);
            //s.transform.localScale = Vector3.one *2;

            //this will need to be changed for different basis vectors
            //s.transform.localPosition = new Vector3(coords.x, coords.y, coords.z);

            //if sphere is in subDims, set postion and radius
            if (isSphereInSubDim(s))
            {
                s.transform.localScale = CalculateDiameter(s);
            }
            else { s.transform.localScale = Vector3.zero; }


            spheres.Add(s);
        }
        //public void createSlices(Vector4 c4D)//////////////////////aggggggggg so bad
        //{
        //    Debug.Log("numberOfds = " + numberOfDs);
        //    for (int i = 0; i < numberOfDs; i++)
        //    {
        //        GameObject thisSlice = Instantiate(sliceOfD);
        //        //slice.transform.localScale = new Vector3(1, 1, 1); set transform when you calc slice centres
        //        setShader shaderInfo = thisSlice.GetComponent<setShader>();
        //        shaderInfo.coords4D = c4D;
        //        shaderInfo.sliceID = 1 - i / numberOfDs;
        //        //testing:
        //        thisSlice.transform.localScale = new Vector3(1, 1, 1);
        //        thisSlice.transform.localPosition = new Vector3(0, 0, 0);
        //        slicesOfD.Add(thisSlice);
        //    }

        //}
        //only used in create spheres - no longer actually needed////////////////////////////////////////////////
        bool isSphereInSubDim(GameObject s)
        {
            float wCoord = s.GetComponent<Info>().Get4DCoords().w;
            if ((wSlice >= 0 && wSlice <= (wCoord + 1)) ||
                   (wSlice <= 0 && wSlice >= (wCoord - 1))
                   )
            { return true; }
            else return false;
        }

        void CalculateCoordinates()
        {
            List<float> numbers = new List<float>();
            float[] options = { 1f, -1f };
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            numbers.Add(options[i]);
                            numbers.Add(options[j]);
                            numbers.Add(options[k]);
                            numbers.Add(options[l]);
                        }
                    }
                }
            }
            //for (int i = 0; i < numbers.Count; i+=4){
            //    Debug.Log(numbers[i] + " " + numbers[i + 1] + " " + numbers[i + 2] + " " + numbers[i + 3]);
            //}

            for (int i = 0; i < numbers.Count; i += 4)
            {
                Vector4 v = new Vector4(numbers[i], numbers[i + 1], numbers[i + 2], numbers[i + 3]);
                all4Dcoords.Add(v);
            }

            for (int i = 0; i < all4Dcoords.Count; i++)
            {
                Debug.Log(all4Dcoords[i]);
            }
            //Debug.Log("count = " + all4Dcoords.Count);
        }

        //from previous version where no rotations
        Vector3 CalculateDiameter(GameObject s)
        {

            float wCoord = s.GetComponent<Info>().Get4DCoords().w;
            //Debug.Log("w coord =" + wCoord);

            //update scale (diamter) based on distance of wslice from center of w coordinate
            float distanceFromCentre = Mathf.Sqrt(1 - (wSlice - wCoord) * (wSlice - wCoord)); //want it to be smallest at max distancwe
            return Vector3.one * 2 * distanceFromCentre;
            //       Debug.Log("diameter = " + s.transform.localScale);


        }
        ///end////////////////////////////////////////////////////////////////////////

        float calcMinDistance(Vector4 n, float d, Vector4 c)
        {
            float cDotn = Vector4.Dot(c, n);
            float minDist = cDotn - d;
            //Debug.Log("c=" + c + ", n=" + n + ", Vector4.Dot(c, n) = " + cDotn);
            return minDist;
        }

        Vector4 rotateParallelToW(Vector4 coords4D, Vector4 n)
        {
            float dots = Vector4.Dot(coords4D, n) / Vector4.Dot(n, n);
            Vector4 rotatedCoords4D = coords4D - (2 * dots * n);
            //TEST!! REMOVE IF BREAKS - FINAL REFLECTION REQUIRES MULTIPLICATION
            //WITH DIAG D, WITH -1 WHERE FOR W COMPONENT (AS PREVIOUSLY FLIPPED)
            rotatedCoords4D.w = -rotatedCoords4D.w;
            ///////////////////////////////////////////////////////////// 
            return rotatedCoords4D;
        }

        //Vector4 rotateParallelToVector(Vector4 coords4D, Vector4 n)
        //{
        //    float dots = Vector4.Dot(coords4D, n) / Vector4.Dot(n, n);
        //    Vector4 rotatedCoords4D = coords4D - (2 * dots * n);
        //    //TEST!! REMOVE IF BREAKS - FINAL REFLECTION REQUIRES MULTIPLICATION
        //    //WITH DIAG D, WITH -1 WHERE FOR W COMPONENT (AS PREVIOUSLY FLIPPED)
        //    rotatedCoords4D.w = -rotatedCoords4D.w;
        //    ///////////////////////////////////////////////////////////// 
        //    return rotatedCoords4D;
        //}

        //public void TriggerUpL(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        //{
        //  //  Debug.Log("Trigger is up L  "+ fromAction +" "+fromSource);
        //    triggeredL = false;
        //    needToResetPositionL = true;
        //}

        //public void TriggerDownL(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        //{
        //    // Debug.Log("Trigger is down L");
        //    if (needToResetPositionL)
        //    {
        //        currentXYrot = controllerPoseL.transform.position.z;
        //        currentYZrot = controllerPoseL.transform.position.x;
        //        currentXZrot = controllerPoseL.transform.position.y;
        //        needToResetPositionL = false;
        //    }
        //    triggeredL = true;
        //}
        //public void TriggerUpR(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        //{
        //    //  Debug.Log("Trigger is up Y");
        //    triggeredR = false;
        //    needToResetPositionR = true;

        //}
        //public void TriggerDownR(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        //{
        //    // Debug.Log("Trigger is down R");
        //    if (needToResetPositionR)
        //    {
        //        YWrot = controllerPoseR.transform.position.z;
        //        XWrot = controllerPoseR.transform.position.y;
        //        ZWrot = controllerPoseR.transform.position.x;
        //        needToResetPositionR = false;
        //    }
        //    triggeredR = true;

        //}


        ///

        // private void TriggerPressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource){}

        //public void testRotor(Vector4 v){
        //    // float xyRotH = xyRot/2;
        //    // float zxRotH = zxRot/2;
        //    // float yzRotH = yzRot/2;
        //    // float xwRotH = xwRot/2;
        //    // float ywRotH = ywRot/2;
        //    // float zwRotH = zwRot/2;

        //    // float A12 = Mathf.Cos(xyRotH);
        //    // float B12 = Mathf.Sin(xyRotH);

        //    // float A13 = Mathf.Cos(zxRotH);
        //    // float B13 = Mathf.Sin(zxRotH);

        //    // float A23 = Mathf.Cos(yzRotH);
        //    // float B23 = Mathf.Sin(yzRotH);

        //    // float A14 = Mathf.Cos(xwRotH);
        //    // float B14 = Mathf.Sin(xwRotH);

        //    // float A24 = Mathf.Cos(ywRotH);
        //    // float B24 = Mathf.Sin(ywRotH);

        //    // float A34 = Mathf.Cos(zwRotH);
        //    // float B34 = Mathf.Sin(zwRotH);



        //    float A12 = Mathf.Cos(xyRot);
        //    float B12 = Mathf.Sin(xyRot);

        //    float A13 = Mathf.Cos(zxRot);
        //    float B13 = Mathf.Sin(zxRot);

        //    float A23 = Mathf.Cos(yzRot);
        //    float B23 = Mathf.Sin(yzRot);

        //    float A14 = Mathf.Cos(xwRot);
        //    float B14 = Mathf.Sin(xwRot);

        //    float A24 = Mathf.Cos(ywRot);
        //    float B24 = Mathf.Sin(ywRot);

        //    float A34 = Mathf.Cos(zwRot);
        //    float B34 = Mathf.Sin(zwRot);



        //}

        private float[,] createIdentityMatrix(int numberOfDimensions)
        {
            float[,] identityMat = new float[numberOfDimensions, numberOfDimensions];
            for (int i = 0; i < numberOfDimensions; i++)
            {
                for (int j = 0; j < numberOfDimensions; j++)
                {
                    if (i == j)
                    {
                        identityMat[i, j] = 1f;
                    }
                    else
                    {
                        identityMat[i, j] = 0f;
                    }
                }
            }
            return identityMat;
        }

        private float[,] outerProduct(Vector4 u, Vector4 v)
        {
            float[,] outerProductMatrix = new float[numberOfDimensions, numberOfDimensions];
            for (int i = 0; i < numberOfDimensions; i++)
            {
                for (int j = 0; j < numberOfDimensions; j++)
                {
                    outerProductMatrix[i, j] = u[i] * v[j];
                }
            }
            return outerProductMatrix;
        }

        //below, big = subtracted from, little = thing subtracting
        private float[,] matrixSubtract(float[,] mBig, float[,] mLittle)
        {
            float[,] newMat = new float[numberOfDimensions, numberOfDimensions];
            for (int i = 0; i < numberOfDimensions; i++)
            {
                for (int j = 0; j < numberOfDimensions; j++)
                {
                    newMat[i, j] = mBig[i, j] - mLittle[i, j];
                }
            }
            return newMat;
        }
        float[,] matrixMultiply(float[,] m1, float[,] m2) //M1*M2
        {
            int m1Rows = m1.GetLength(0);
            int m1Columns = m1.GetLength(1);
            int m2Rows = m2.GetLength(0);
            int m2Columns = m2.GetLength(1);
            Debug.Log("m1 size: " + m1.GetLength(0) + " " + m1.GetLength(1) + "m2 size: " + m2.GetLength(0) + m2.GetLength(1));
            if (m1.GetLength(1) != m2.GetLength(0))
            {
                Debug.Log("Arrays wrong length! m1(1) = " + m1.GetLength(1) + " , m2(0) = " + m2.GetLength(0));
            }
            float[,] multipliedMatrix = new float[m1Rows, m2Columns];
            for (int i = 1; i < m1Rows; i++)
            {
                for (int j = 0; j < m2Columns; j++)
                {
                    for (int k = 0; k < m1Columns; k++)
                    {
                        //IndexOutOfRangeException: Index was outside the bounds of the array.

                        multipliedMatrix[i, j] += m1[i, k] * m2[k, j];
                    }

                }

            }
            return multipliedMatrix;
        }

        Matrix4x4 turnFloatArrayIntoMatrix4x4(float[,] floatArr)
        {
            Matrix4x4 mat = Matrix4x4.zero;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    mat[i, j] = floatArr[i, j];
                }
            }
            return mat;
        }

        float[,] turnMatrixIntoFloatArrray(Matrix4x4 mat)
        {
            float[,] floatArr = new float[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    floatArr[i, j] = mat[i, j];
                }
            }
            return floatArr;
        }

        bool isOnHyperPlane(Vector4 sliceCentre4D, Vector4 unitNormal, float de)
        {
            //equation of hyperplane for unitNormal = (a,b,c,e) is ax + by + cz + ew = d
            float hypeEquation = (unitNormal.x * sliceCentre4D.x) + (unitNormal.y * sliceCentre4D.y) + (unitNormal.z * sliceCentre4D.z) + (unitNormal.w * sliceCentre4D.w);
            if (hypeEquation <= de + 0.0000001 &&
                hypeEquation >= de- 0.0000001)
            {
                return true;
            }
            else
            {
                Debug.Log("hypeEquation=" + hypeEquation + ", dslice=" + de);
                return false;
            }

        }

        bool isUnitNormalUNITARY(Vector4 unitNormal)
        {
            if (unitNormal.magnitude <= 1 + 0.0000005 &&
                unitNormal.magnitude >= 1 - 0.0000005)
            {
                return true;
            }
            else return false;
        }
    }

}