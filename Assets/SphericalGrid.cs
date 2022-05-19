using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SphericalGrid : MonoBehaviour
{
    public SteamVR_Behaviour_Pose controllerPoseLeft, controllerPoseRight;
    public Transform head;

    public GameObject pointPrefab1, pointPrefab2, pointPrefab3;


    [Range(10, 100)]
    public int resolution = 10;

    const float pi = Mathf.PI;

    Transform[] points1, points2, points3;

    public float radius = 1f;
    public Vector3 centre = Vector3.zero;

    public int granularity = 20;

    public int numberOfLayers = 3;

	public int size = 5;
	float offset;

    // Start is called before the first frame update
    void Start()
    {
        controllerPoseLeft.transform.parent = head.transform;
        controllerPoseRight.transform.parent = head.transform;
        offset = 2 * pi / numberOfLayers;
        //float rad = maxRadius/ numberOfLayers;

       

		InvokeRepeating("LaunchProjectiles", 2.0f, 0.1f);

	}

    // Update is called once per frame
    void Update()
    {
        
    }

	
	void LaunchProjectiles()
	{
		CreateSphereOfBalls(offset, radius, pointPrefab1.transform, ref points1, controllerPoseLeft.transform);
		CreateSphereOfBalls(offset * 2, radius, pointPrefab2.transform, ref points2, controllerPoseRight.transform);
		CreateSphereOfBalls(offset * 3, radius, pointPrefab3.transform, ref points3, controllerPoseRight.transform);
		for (int i = -size; i < size; i++)
		{
			for (int j = -size; j < size; j++)
			{
				/* for (int k = -size; k < size; k++)
                 {
				
                     GameObject point = Instantiate(pf);
                     point.transform.position = new Vector3(i + centre.x, j + centre.y, k + centre.z) * scale;
                     point.transform.rotation = controllerPose.transform.rotation;
                 }*/
				/* GameObject point = Instantiate(pf);
				 point.transform.position = new Vector3(i + centre.x, j + centre.y, centre.z) * scale;
				 point.transform.rotation = controllerPose.transform.rotation;*/
			}
		}
	}

	private void CreateSphereOfBalls(float offset, float radius, Transform prefab, ref Transform[] points, Transform controllerTransform)
	{
		float greatCircumference = 2 * pi * radius;
		float ratioOfBallsToCircumferenceSize = greatCircumference / granularity;
		float angleOfRotationPerBall = 360 / granularity;
		float radians = angleOfRotationPerBall * Mathf.Deg2Rad;


		Vector3 location = Vector3.zero;

		//first calc numBalls
		int totalBalls = 0;

		for (int i = 0; i < granularity; i++)
		{

			//sin goes from 0 to 1 - want (centre-radius, to centre+radius)
			float y = (Mathf.Sin(radians * i) * radius) + centre.y;
			float yRadius = Mathf.Sqrt(radius * radius - (y - centre.y) * (y - centre.y));

			int numBalls = Mathf.CeilToInt(2 * pi * yRadius / ratioOfBallsToCircumferenceSize);

			if (numBalls == 0) { numBalls = 1; }

			totalBalls += numBalls;
		}

		points = new Transform[totalBalls];

		int index = 0;

		for (int i = 0; i < granularity; i++)
		{


			//sin goes from 0 to 1 - want (centre-radius, to centre+radius)
			float y = (Mathf.Sin(radians * i) * radius) + centre.y;
			float yRadius = Mathf.Sqrt(radius * radius - (y - centre.y) * (y - centre.y));

			int numBalls = Mathf.CeilToInt(2 * pi * yRadius / ratioOfBallsToCircumferenceSize);
			Debug.Log("numberOfBalls rounded to int = " + numBalls);

			//location.x =centre.x - yRadius;
			location.y = y;

			float rotation = 0f;
			if (numBalls != 0)
			{
				rotation = 360 / numBalls;
			}
			else
			{
				numBalls = 1;
				Debug.Log("numballs = 0, y = " + y + "yRad = " + yRadius);
			}

			//Vector3 rotateAroundMe = new Vector3(centre.x, y, centre.z );



			for (int j = 0; j < numBalls; j++)
			{

				float xRadians = rotation * Mathf.Deg2Rad;

				location.z = yRadius * Mathf.Sin((xRadians * j) + offset) + centre.z;

				location.x = yRadius * Mathf.Cos((xRadians * j) + offset) + centre.x;

				Transform point = Instantiate(prefab, location, controllerTransform.rotation);
				points[index] = point;
				index++;


			}


		}

	}
}
