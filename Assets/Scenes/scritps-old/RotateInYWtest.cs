using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class RotateInYWtest : MonoBehaviour
    {

        public GameObject sphere;

        public List<Vector3> locatons;

        public int numberOfSpheres = 16;

        public List<GameObject> spheres;

        public List<Vector4> all4Dcoords;

        public float wSlice;



        public LinearMapping linearMapping;
        private float currentLinearMapping = float.NaN;
        private int framesUnchanged = 0;

        //HYPERPLANE INFO
        ///////////////////////////////////
        //private float[] unitNormal = new float[4];
        //private Vector4 unitNormal = new Vector4(0.5f,0.5f,0.5f,0.5f);
        private Vector4 unitNormal = new Vector4(0f, 1f, 0f, 0f);
        private Vector4 unitNormalParallelToZ = new Vector4(0f, 0f, 0f, 1f);
        private Vector4 reflectionParallelToZ = new Vector4(0f, 0f, 0f, -1f);
        public float d = 0.2f;

        public float r = 1f;


        /////////////////
        public SteamVR_Input_Sources leftHand;
        public SteamVR_Input_Sources rightHand;
        public SteamVR_Behaviour_Pose controllerPoseL;
        public SteamVR_Behaviour_Pose controllerPoseR;
        public SteamVR_Action_Boolean trigger;
        public SteamVR_Action_Boolean clicked;
        private bool triggeredL;
        private bool triggeredR;

        public Transform planePF;
        //private Transform xyRotT, yzRotT, zxRotT, ywRotT, xwRotT, zwRotT;
        private float xyRot, yzRot, zxRot, ywRot, xwRot, zwRot;

        Matrix4x4 xyRotMat = new Matrix4x4();
        Matrix4x4 yzRotMat = new Matrix4x4();
        Matrix4x4 zxRotMat = new Matrix4x4();
        Matrix4x4 xwRotMat = new Matrix4x4();
        Matrix4x4 ywRotMat = new Matrix4x4();
        Matrix4x4 zwRotMat = new Matrix4x4();




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


        }

        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < numberOfSpheres; i++)
            {
                CreateSpheres(i);
            }

            for (int i = 0; i < numberOfSpheres; i++)
            {
                Debug.Log("sphere " + i + spheres[i].GetComponent<Info>().Get4DCoords());
            }

            /////////////////test rotations from contoller(s)
            // //instantiate prefab
            // xyRotT = Instantiate(planePF, new Vector3(0, 1.5f, 1), Quaternion.identity);
            // // xArrow.SetParent(transform, false);
            // yzRotT = Instantiate(planePF, new Vector3(0.5f, 1.5f, 1), Quaternion.identity);
            // //  yArrow.SetParent(transform);
            // zxRotT = Instantiate(planePF, new Vector3(1, 1.5f, 1), Quaternion.identity);
            // //  zArrow.SetParent(transform);
            // ywRotT = Instantiate(planePF, new Vector3(0, 0.5f, 1), Quaternion.identity);
            // xwRotT = Instantiate(planePF, new Vector3(0.5f, 0.5f, 1), Quaternion.identity);
            // zwRotT = Instantiate(planePF, new Vector3(1, 0.5f, 1), Quaternion.identity);



            trigger.AddOnStateDownListener(TriggerDownL, leftHand);
            trigger.AddOnStateUpListener(TriggerUpL, leftHand);
            trigger.AddOnStateDownListener(TriggerDownR, rightHand);
            trigger.AddOnStateUpListener(TriggerUpR, rightHand);
            //SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(TriggerPressed, SteamVR_Input_Sources.Any);


        }

        // Update is called once per frame
        void Update()
        {

            if (currentLinearMapping != linearMapping.value)
            {
                currentLinearMapping = linearMapping.value;
                //d = (currentLinearMapping - 0.5f) * 4f;
                //INSTEAD OF UPDATING D, ROTATE UNIT NORMAL OF HYPERPLANE

                


                ///////////////////////////////////////////////////
                for (int i = 0; i < numberOfSpheres; i++)
                {
                    Vector4 c = spheres[i].GetComponent<Info>().Get4DCoords();
                    float minDis = calcMinDistance(unitNormal, d, c);

                    //if is in inntersection, get centreOfSlice and sliceRadius:
                    if (-r <= minDis && minDis <= r)
                    {
                        //calculate sliceCentre (4D)
                        Vector4 sliceCentre4D = c - minDis * unitNormal;
                        //need to convert 4D vectors to 3D local coordinated defined by hyperplane!!
                        //!!!!!!!!!!!!!!!!!!!!!!!

                        Vector4 n = reflectionParallelToZ - unitNormal;
                        Vector4 rotated4D;

                        if (n != Vector4.zero)
                        {
                            rotated4D = rotateParallelToW(sliceCentre4D, n);
                            Debug.Log(i + ": " + rotated4D);
                        }
                        else rotated4D = sliceCentre4D;


                        //calcualte radius (perpendicular to n)
                        float sliceRadius = Mathf.Sqrt(r * r - minDis * minDis);



                        //need to change below to local coords!
                        spheres[i].transform.localPosition = new Vector3(rotated4D.x, rotated4D.y, rotated4D.z);
                        spheres[i].transform.localScale = Vector3.one * sliceRadius * 2;
                    }
                    //else sliceradius = 0;
                    else { spheres[i].transform.localScale = Vector3.zero; }
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
                Debug.Log("euler " + controllerPoseL.transform.eulerAngles.x + " " + controllerPoseL.transform.eulerAngles.y + " " + controllerPoseL.transform.eulerAngles.z);
                // xyRot = controllerPoseL.transform.eulerAngles.z * Mathf.Deg2Rad;
                // zxRot = controllerPoseL.transform.eulerAngles.y * Mathf.Deg2Rad;
                // yzRot = controllerPoseL.transform.eulerAngles.x * Mathf.Deg2Rad;

                
            }
            if (triggeredR)
            {
                // ywRotT.rotation = Quaternion.Euler(0,0,controllerPoseR.transform.eulerAngles.z);
                // xwRotT.rotation = Quaternion.Euler(controllerPoseR.transform.eulerAngles.x,0,0);
                // zwRotT.rotation = Quaternion.Euler(0,controllerPoseR.transform.eulerAngles.y,0);
                xwRot = controllerPoseR.transform.eulerAngles.z * Mathf.Deg2Rad;
                ywRot = controllerPoseR.transform.eulerAngles.y * Mathf.Deg2Rad;
                zwRot = controllerPoseR.transform.eulerAngles.x * Mathf.Deg2Rad;                                                                                                                                               
            }

            rotateUnitNormal();

            // //need to fix clicked-grabgrip
            // if (clicked.GetLastStateDown(leftHand)){
            // Debug.Log("HERE!! " );  
            // }


        }

        private void rotateUnitNormal()
        {
            //////////////////////////////////////////////////////TO DO!
            //xy:
            float cos = Mathf.Cos(xyRot);
            float sin = Mathf.Sin(xyRot);
            xyRotMat.SetRow(0, new Vector4(cos, sin, 0, 0));
            xyRotMat.SetRow(1, new Vector4(-sin, cos, 0, 0));
            xyRotMat.SetRow(2, new Vector4(0, 0, 1, 0));
            xyRotMat.SetRow(3, new Vector4(0, 0, 0, 1));

            //yz:
            cos = Mathf.Cos(yzRot);
            sin = Mathf.Sin(yzRot);
            yzRotMat.SetRow(0, new Vector4(1, 0, 0, 0));
            yzRotMat.SetRow(1, new Vector4( 0, cos, sin, 0));
            yzRotMat.SetRow(2, new Vector4( 0, -sin, cos, 0));
            yzRotMat.SetRow(3, new Vector4(0, 0, 0, 1));


            //zx:
            cos = Mathf.Cos(zxRot);
            sin = Mathf.Sin(zxRot);
            zxRotMat.SetRow(0, new Vector4(cos, 0, -sin, 0));
            zxRotMat.SetRow(1, new Vector4(0, 1, 0, 0));
            zxRotMat.SetRow(2, new Vector4(sin, 0, cos, 0));
            zxRotMat.SetRow(3, new Vector4(0, 0, 0, 1));

            //xw:
            cos = Mathf.Cos(xwRot);
            sin = Mathf.Sin(xwRot);
            xwRotMat.SetRow(0, new Vector4(cos,0, 0, sin));
            xwRotMat.SetRow(1, new Vector4(0, 1,0, 0));
            xwRotMat.SetRow(2, new Vector4(0, 0, 1, 0));
            xwRotMat.SetRow(3, new Vector4(-sin, 0, 0, cos));

            //yw:
            cos = Mathf.Cos(ywRot);
            sin = Mathf.Sin(ywRot);
            ywRotMat.SetRow(0, new Vector4(1,0,0,0));
            ywRotMat.SetRow(1, new Vector4(0, cos, 0, -sin));
            ywRotMat.SetRow(2, new Vector4(0, 0, 1, 0));
            ywRotMat.SetRow(3, new Vector4(0, sin, 0, cos));

            //zw:
            cos = Mathf.Cos(zwRot);
            sin = Mathf.Sin(zwRot);
            zwRotMat.SetRow(0, new Vector4(1,0,0,0));
            zwRotMat.SetRow(1, new Vector4(0, 1,0, 0));
            zwRotMat.SetRow(2, new Vector4(0, 0, cos, -sin));
            zwRotMat.SetRow(3, new Vector4(0, 0, sin, cos));

            unitNormal = (xyRotMat * yzRotMat * zxRotMat * xwRotMat * ywRotMat * zwRotMat).MultiplyVector(unitNormal);
            unitNormal.Normalize(); //quick fix for now
            Debug.Log("unit normal is now: " + unitNormal + " , it is unitary? " + unitNormal.magnitude );

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

            //s.transform.localPosition = new Vector3(coords.x, coords.y, coords.z);
            //s.transform.localScale = Vector3.one *2;

            //this will need to be changed for different basis vectors
            s.transform.localPosition = new Vector3(coords.x, coords.y, coords.z);

            //if sphere is in subDims, set postion and radius
            if (isSphereInSubDim(s))
            {
                s.transform.localScale = CalculateDiameter(s);
            }
            else { s.transform.localScale = Vector3.zero; }


            spheres.Add(s);
        }

        bool isSphereInSubDim(GameObject s)
        {
            float wCoord = s.GetComponent<Info>().Get4DCoords().w;
            if ((wSlice >= 0 && wSlice <= (wCoord + 1)) ||
                   (wSlice <= 0 && wSlice >= (wCoord - 1))
                   )
            { return true; }
            else return false;
        }



        void SetPosition(GameObject s)
        {

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

        float calcMinDistance(Vector4 n, float d, Vector4 c)
        {
            float minDist = Vector4.Dot(c, n) - d;
            return minDist;
        }

        Vector4 rotateParallelToW(Vector4 coords4D, Vector4 n)
        {
            float dots = Vector4.Dot(coords4D, n) / Vector4.Dot(n, n);
            Vector4 rotatedCoords4D = coords4D - (2 * dots * n);
            return rotatedCoords4D;
        }

        public void TriggerUpL(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Debug.Log("Trigger is up L");
            triggeredL = false;
        }
        public void TriggerDownL(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Debug.Log("Trigger is down L");
            triggeredL = true;
        }
        public void TriggerUpR(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Debug.Log("Trigger is up R");
            triggeredR = false;
        }
        public void TriggerDownR(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Debug.Log("Trigger is down R");
            triggeredR = true;

        }
        // private void TriggerPressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource){}

    }

}