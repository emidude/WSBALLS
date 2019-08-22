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

        public int numberOfDSlices = 10;
        float[] slicesOfD;
        float closestD;
        public float furthestD = 10;
        

        void Awake()
        {
            spheres = new List<GameObject>();
            //wSlice = 0f;
            all4Dcoords = new List<Vector4>();
            CalculateCoordinates();

            //locatons = new List<Vector3> { Vector3.one, Vector3.one * 2f, Vector3.one * 3f };

            if (linearMapping == null)
            {
                linearMapping = GetComponent<LinearMapping>();
            }

            //id mat used in rotation formula between vectors
            identityMatrix = createIdentityMatrix(numberOfDimensions);

            slicesOfD = new float[numberOfDSlices];
            closestD = d;

        }

        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < numberOfSpheres; i++)
            {
                CreateSpheres(i);
            }


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
            
            //////////////////////////////////////////////////////////////PUT ME BACK AFFTER TESTING!
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
            

            //update linear mapping from slider, current range: -2,2
            if (currentLinearMapping != linearMapping.value)
            {
                currentLinearMapping = linearMapping.value;
                d = (currentLinearMapping - 0.5f) * 4f;
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            updateSlicesOfD(d);////////////////MAYBE JUST NEED INCREMENT AMOUNT NOT ACTUAL DS



            //Debug.Log("unitNormal=" + unitNormal);
            ///////////////////////////////////////////////////
            for (int i = 0; i < numberOfSpheres; i++)
            {
                //Info sphereInfo = spheres[i].GetComponent<Info>();
                //float r = spheres[i].GetComponent<Info>().radius; //Debug.Log("r = " + r);
                Vector4 c = spheres[i].GetComponent<Info>().Get4DCoords();
                float minDis = calcMinDistance(unitNormal, d, c); //this projects c onto n (n.c) and
                                                                  //caluclates min distance between this and the hyperplane

                //if min distance (c.n - d ) is within radius r of sphere, some of sphere c intersects hyperplane  
                //if is in inntersection, get centreOfSlice and sliceRadius:
                if (-r <= minDis && minDis <= r)
                {
                    //calculate sliceCentre (4D)
                    Vector4 sliceCentre4D = c - minDis * unitNormal;
                    //testing - check if this lies on the hyperplane:
                    if (!isOnHyperPlane(sliceCentre4D, unitNormal, d))
                    {
                        Debug.Log("NOT ON HYPERPLANE!" + " c = " + c + "slice centre:" + sliceCentre4D + " d= " + d + " minDis = " + minDis + "unitNormal = " + unitNormal);
                    }
                    else
                    {
                        Debug.Log("ON HYPERPLANE!");
                    }

                    Vector4 n = reflectionParallelToZ - unitNormal;
                    Vector4 rotated4D;

                    if (n != Vector4.zero)
                    {
                        rotated4D = rotateParallelToW(sliceCentre4D, n);
                    }
                    else { rotated4D = sliceCentre4D; }
                    // Debug.Log(i + ": " + rotated4D);


                    //calcualte radius (perpendicular to n)
                    float sliceRadius = Mathf.Sqrt(r * r - minDis * minDis);
                    Debug.Log(i + " slice radius = " + sliceRadius);


                    //the z coord is now constant for all slice coords so can place in 3d vr space
                    spheres[i].transform.localPosition = new Vector3(rotated4D.x, rotated4D.y, rotated4D.z);
                    spheres[i].transform.localScale = Vector3.one * sliceRadius * 2;
                }
                //else sliceradius = 0;
                else { spheres[i].transform.localScale = Vector3.zero; }
            }
        }

        
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

            Debug.Log("XY " + "unitNormal " + unitNormal + "is unit?: " + unitNormal.magnitude);
            if (!isUnitNormalUNITARY(unitNormal))
            {
                Debug.Log("HERE IS BAD: " + oldUnitNormal + " TO " + unitNormal);
            }
            unitNormal.Normalize();

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
            Debug.Log("YZ " + "unitNormal " + unitNormal + "is unit?: " + unitNormal.magnitude);
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
            Debug.Log("XZ " + "unitNormal " + unitNormal + "is unit?: " + unitNormal.magnitude);
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
            Debug.Log("XW " + "unitNormal " + unitNormal + "is unit?: " + unitNormal.magnitude);
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
            Debug.Log("YW " + "unitNormal " + unitNormal + "is unit?: " + unitNormal.magnitude);
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
            Debug.Log("ZW" + " unitNormal " + unitNormal + "is unit?: " + unitNormal.magnitude);
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
            sphereInfo.setSliceCentresAndRadiusArrays(numberOfDSlices);

            ////////////TEST RUN HERE FOR CUBE SLICE
            sphereInfo.createSlices();
            
            //if sphere is in subDims, set postion and radius
            if (isSphereInSubDim(s))
            {
                s.transform.localScale = CalculateDiameter(s);
            }
            else { s.transform.localScale = Vector3.zero; }

            spheres.Add(s);
        }

        //only used in create spheres////////////////////////////////////////////////
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
            Debug.Log("c=" + c + ", n=" + n + ", Vector4.Dot(c, n) = " + cDotn);
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

        bool isOnHyperPlane(Vector4 sliceCentre4D, Vector4 unitNormal, float d)
        {
            //equation of hyperplane for unitNormal = (a,b,c,e) is ax + by + cz + ew = d
            float hypeEquation = (unitNormal.x * sliceCentre4D.x) + (unitNormal.y * sliceCentre4D.y) + (unitNormal.z * sliceCentre4D.z) + (unitNormal.w * sliceCentre4D.w);
            if (hypeEquation <= d + 0.0000001 &&
                hypeEquation >= d - 0.0000001)
            {
                return true;
            }
            else
            {
                Debug.Log("hypeEquation=" + hypeEquation + ", d=" + d);
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

        void updateSlicesOfD(float d)
        {
         for (int i = 0; i < numberOfSpheres; i++)
            {
                spheres[i].GetComponent<Info>().sliceOfD.transform.localPosition = spheres[i].transform.localPosition;
            }   
        }
    }

}