using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class tease : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public GameObject pf;
    public Transform head;
    private Transform hand;
	public int granularity = 20;
	const float pi = Mathf.PI;
	private Transform[] points;


	// Start is called before the first frame update
	void Start()
    {
        controllerPose.transform.parent = head.transform;
        InvokeRepeating("LaunchProjectile", 2.0f, 0.3f);

        hand = controllerPose.transform;

        Debug.Log("hand transform position = " + hand.position);
        Debug.Log("hand transform lcoal position = " + hand.localPosition);

		//Vector3 velo = controllerPose.GetVelocity();

		//InvokeRepeating("LaunchGridProjectile", 1.0f, 1f);

		//calculate sphere starting points (to be offset by head position)
		//CreateSphereOfBalls()


	}

    // Update is called once per frame
    void Update()
    {
        
    }

    void LaunchProjectile()
    {
        GameObject instance = (GameObject)Instantiate(pf, hand.position, hand.rotation);
		Destroy(instance, 1f);
    }

    //very bad gets too big and crashes fast
    void LaunchGridProjectile()
    {
        //make grid
        for (int x = -10; x < 10; x++)
        {
            for(int y = -10; y < 10; y++)
            {
                for (int z = -10; z < 10; z++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    GameObject instance = (GameObject)Instantiate(pf, pos, hand.rotation);
					Destroy(instance, 1);
                }
            }
        }
    }

	private void CreateSphereOfBalls(float offset, float radius, Transform prefab, ref Transform[] points, Vector3 centre)
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

				Transform point = Instantiate(prefab, location, Quaternion.identity);
				points[index] = point;
				index++;


			}


		}

	}

}
