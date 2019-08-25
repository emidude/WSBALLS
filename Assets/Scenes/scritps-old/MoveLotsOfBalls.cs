using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MoveLotsOfBalls : MonoBehaviour
{
    //public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPoseLeft, controllerPoseRight;
    public SteamVR_Action_Boolean clicked;

    public Transform xArrowPF, yArrowPF, zArrowPF;
    private Transform xArrow, yArrow, zArrow, testRotation;
    //public GameObject xArrow, yArrow, zArrow;

    public Transform head;

///////////////////////////////////
    public Transform pointPrefab1, pointPrefab2, pointPrefab3;

    [Range(10,100)]
    public int resolution = 10;

    const float pi = Mathf.PI;

    Transform[] points1, points2, points3;

// /////////////////////////////////////////
    private Vector3[] vertices;
	private Vector3[] normals;
	public int gridSize = 20;
	//public float radius = 1f;
// //////////////////////////////////////////
	public float radius = 1f;
	public Vector3 centre = Vector3.zero;

	public int granularity = 20;

	public int numberOfLayers = 3;

    void Awake(){
		
		float offset = 2 * pi / numberOfLayers;
		//float rad = maxRadius/ numberOfLayers;

		
		CreateSphereOfBalls(offset, radius, pointPrefab1, ref points1);
		CreateSphereOfBalls(offset*2, radius, pointPrefab2, ref points2);
		CreateSphereOfBalls(offset*3, radius, pointPrefab3, ref points3);


		//FOR SQUARE BALL
        // CreateVertices();
        // Vector3 scale = Vector3.one * 0.5f;
        // points = new Transform[vertices.Length];
        // for(int i = 0 ; i < points.Length; i++){
        // Transform point = Instantiate(pointPrefab,vertices[i], Quaternion.identity);
        // point.localScale = scale;
        // point.SetParent(transform,false);
        // points[i] = point;
        // }

		controllerPoseLeft.transform.parent = head.transform;
		controllerPoseRight.transform.parent = head.transform;

    }



    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    public void Update()
    {
		 Vector3 posL =  controllerPoseLeft.transform.localPosition;
		 Vector3 posR =  controllerPoseRight.transform.localPosition;
		 Vector3 posH = head.transform.localPosition;

		 Vector3 veloL = controllerPoseLeft.GetVelocity();
		 Vector3 veloR = controllerPoseRight.GetVelocity();

		 Vector3 avL = controllerPoseLeft.GetAngularVelocity();
		 Vector3 avR = controllerPoseRight.GetAngularVelocity();

		 for(int i = 0 ; i < points1.Length; i++){
			// points1[i].localScale = new Vector3(posR.x, points1[i].localScale.y, points1[i].localScale.z);
			points1[i].localScale = new Vector3(posR.x, posR.x, posR.x);
			
		 }
        

        }

	private void CreateSphereOfBalls(float offset, float radius, Transform prefab, ref Transform[] points){
		float greatCircumference = 2 * pi * radius;
		float ratioOfBallsToCircumferenceSize = greatCircumference / granularity;
		float angleOfRotationPerBall = 360 / granularity;
		float radians = angleOfRotationPerBall *Mathf.Deg2Rad;


		Vector3 location = Vector3.zero;

		//first calc numBalls
		int totalBalls = 0;

			for(int i = 0 ; i < granularity ; i ++){
				
				//sin goes from 0 to 1 - want (centre-radius, to centre+radius)
				float y = (Mathf.Sin(radians*i) * radius) + centre.y ;
				float yRadius = Mathf.Sqrt(radius*radius - (y - centre.y)*(y - centre.y) );

				int numBalls = Mathf.CeilToInt(2*pi*yRadius / ratioOfBallsToCircumferenceSize) ;

				if (numBalls ==0){numBalls =1;}

				totalBalls += numBalls;
			}

		points = new Transform[totalBalls];
		
		int index = 0;
		
			for(int i = 0 ; i < granularity ; i ++){
				

				//sin goes from 0 to 1 - want (centre-radius, to centre+radius)
				float y = (Mathf.Sin(radians*i) * radius) + centre.y ;
				float yRadius = Mathf.Sqrt(radius*radius - (y - centre.y)*(y - centre.y) );

				int numBalls = Mathf.CeilToInt(2*pi*yRadius / ratioOfBallsToCircumferenceSize) ;
				Debug.Log("numberOfBalls rounded to int = " + numBalls);

				//location.x =centre.x - yRadius;
				location.y = y;

				float rotation = 0f;
				if(numBalls!=0){
					rotation = 360/numBalls;
				}
				else{
					numBalls = 1;
					Debug.Log("numballs = 0, y = "+ y + "yRad = " + yRadius );
				}
				
				//Vector3 rotateAroundMe = new Vector3(centre.x, y, centre.z );

				

				for(int j = 0 ; j < numBalls; j++){
					
					float xRadians = rotation * Mathf.Deg2Rad;

					location.z = yRadius * Mathf.Sin((xRadians * j) + offset) + centre.z;

					location.x =  yRadius * Mathf.Cos((xRadians * j) +offset ) + centre.x;
					
					Transform point = Instantiate(prefab, location , Quaternion.identity);
					points[index] = point;
					index++;
					

				}
			

			}

	}

	public float map(float from, float fromMin, float fromMax, float toMin, float toMax){
		float fromAbs = from - fromMin;
		float fromMaxAbs = fromMax - fromMin;
		float normal = fromAbs / fromMaxAbs;
		float toMaxAbs = toMax - toMin;
		float toAbs = toMaxAbs * normal;
		float to = toAbs + toMin;
		return to;
	}

/////////////////////////////SQuare BALLS
    private void CreateVertices () {
		
		int cornerVertices = 8;
		int edgeVertices = (gridSize + gridSize + gridSize - 3) * 4;
		int faceVertices = (
			(gridSize - 1) * (gridSize - 1) +
			(gridSize - 1) * (gridSize - 1) +
			(gridSize - 1) * (gridSize - 1)) * 2;
		vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
		normals = new Vector3[vertices.Length];
		//cubeUV = new Color32[vertices.Length];

		int v = 0;
		for (int y = 0; y <= gridSize; y++) {
			for (int x = 0; x <= gridSize; x++) {
				SetVertex(v++, x, y, 0);
			}
			for (int z = 1; z <= gridSize; z++) {
				SetVertex(v++, gridSize, y, z);
			}
			for (int x = gridSize - 1; x >= 0; x--) {
				SetVertex(v++, x, y, gridSize);
			}
			for (int z = gridSize - 1; z > 0; z--) {
				SetVertex(v++, 0, y, z);
			}
		}
		for (int z = 1; z < gridSize; z++) {
			for (int x = 1; x < gridSize; x++) {
				SetVertex(v++, x, gridSize, z);
			}
		}
		for (int z = 1; z < gridSize; z++) {
			for (int x = 1; x < gridSize; x++) {
				SetVertex(v++, x, 0, z);
			}
		}	
	}

    private void SetVertex (int i, int x, int y, int z) {
		Vector3 v = new Vector3(x, y, z) * 2f / gridSize - Vector3.one;
		float x2 = v.x * v.x;
		float y2 = v.y * v.y;
		float z2 = v.z * v.z;
		Vector3 s;
		s.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
		s.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
		s.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);
		normals[i] = s;
		vertices[i] = normals[i] * radius;
		//cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
	}

	/////////////////////////END SQUARE BALLS
}
