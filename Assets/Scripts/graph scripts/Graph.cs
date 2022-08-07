using UnityEngine;
using Valve.VR;
using UnityEngine.Networking;
public class Graph : NetworkBehaviour {

	//public Transform pointPrefab;
	public GameObject pointPrefab;

	[Range(10, 100)]
	public int resolution = 10;

	public GraphFunctionName function;

	public GraphFunctionNameSteamInputs fsteam;

	//Transform[] points;
	GameObject[] points;

	public Transform head;
	public SteamVR_Behaviour_Pose controllerL, controllerR;

	public float size = 6;

	public bool inputFunction;

	void Awake () {

		/*float step = 2f / resolution;
		Vector3 scale = Vector3.one * step ;
		transform.position = head.position;

		points = new Transform[resolution * resolution];
		for (int i = 0; i < points.Length; i++) {
			Transform point = Instantiate(pointPrefab);
			point.localScale = scale;
			point.SetParent(transform, false);
			points[i] = point;
		}*/
	}

    void Start()
    {
		CmdSpawnCubes();
    }

    void Update () {

		if (!isLocalPlayer)
        {
			return;
        }

		float t = Time.time; 

		GraphFunctionSteamInputs fI = fSteams[(int)fsteam];
		GraphFunction f = functions[(int)function];


       /* float step = 2f / resolution;
        for (int i = 0, z = 0; z < resolution; z++)
        {
            float v = (z + 0.5f) * step - 1f;
            for (int x = 0; x < resolution; x++, i++)
            {
                float u = (x + 0.5f) * step - 1f;

                if (inputFunction)
                {
                    points[i].transform.localPosition = fI(controllerL, controllerR, u, v, t) * 5;
                }
                else
                {
                    points[i].transform.localPosition = f(u, v, t) * 5;
                }
            }
        }*/

        CmdUpdateCubePositions(t);


    }
	[Command]
	void CmdSpawnCubes()
    {
		float step = 2f / resolution;
		Vector3 scale = Vector3.one * step;
		transform.position = head.position;

		//points = new Transform[resolution * resolution];
		points = new GameObject[resolution * resolution];
		for (int i = 0; i < points.Length; i++)
		{
			GameObject point = Instantiate(pointPrefab);
			point.transform.localScale = scale;
			point.transform.SetParent(transform, false);
			points[i] = point;
			NetworkServer.Spawn(point);

		}
	}

	[Command]
	void CmdUpdateCubePositions(float t)
    {
		float step = 2f / resolution;
		for (int i = 0, z = 0; z < resolution; z++)
		{
			float v = (z + 0.5f) * step - 1f;
			for (int x = 0; x < resolution; x++, i++)
			{
				float u = (x + 0.5f) * step - 1f;

					
				points[i].transform.localPosition = SimpleSin(controllerL, controllerR, u, v, t) * 5;
				

			}
		}
	}

	static GraphFunction[] functions = {
		SineFunction, Sine2DFunction, MultiSineFunction, MultiSine2DFunction,
		Ripple, Cylinder, Sphere, Torus
	};

	static GraphFunctionSteamInputs[] fSteams = { SimpleSin,  MultiSineFunctionSI };

	const float pi = Mathf.PI;

	//[Command]
	 static Vector3 SimpleSin(SteamVR_Behaviour_Pose cL, SteamVR_Behaviour_Pose cR, float u, float v, float t)
    {
		Vector3 p;
		p.x = Mathf.Sin(cL.transform.position.x*u );
		p.y = Mathf.Sin(pi * (cL.transform.position.y*u ));
		//p.y = Mathf.Sin(pi * (cL.GetVelocity().y * u + t));
		//p.y *= cL.GetVelocity().z;
		p.z = Mathf.Sin(cL.transform.position.z * v);
		return p;
    }

	static Vector3 MultiSineFunctionSI(SteamVR_Behaviour_Pose cL, SteamVR_Behaviour_Pose cR, float x, float z, float t)
	{
		Vector3 p;
		p.x = x;
		p.y = Mathf.Sin(pi * (cL.GetVelocity().x*x ));
		//p.y += Mathf.Sin(2f * pi * (x + 2f * t)) / 2f;
		p.y += Mathf.Sin(pi*(2f * cR.GetVelocity().x * x +t  )/2f);
		p.y *= 2f / 3f;
		p.z = z;
		return p;
	}

	static Vector3 SineFunction (float x, float z, float t) {
		Vector3 p;
		p.x = x;
		p.y = Mathf.Sin(pi * (x + t));
		p.z = z;
		return p;
	}

	static Vector3 MultiSineFunction (float x, float z, float t) {
		Vector3 p;
		p.x = x;
		p.y = Mathf.Sin(pi * (x + t));
		p.y += Mathf.Sin(2f * pi * (x + 2f * t)) / 2f;
		p.y *= 2f / 3f;
		p.z = z;
		return p;
	}

	static Vector3 Sine2DFunction (float x, float z, float t) {
		Vector3 p;
		p.x = x;
		p.y = Mathf.Sin(pi * (x + t));
		p.y += Mathf.Sin(pi * (z + t));
		p.y *= 0.5f;
		p.z = z;
		return p;
	}

	static Vector3 MultiSine2DFunction (float x, float z, float t) {
		Vector3 p;
		p.x = x;
		p.y = 4f * Mathf.Sin(pi * (x + z + t / 2f));
		p.y += Mathf.Sin(pi * (x + t));
		p.y += Mathf.Sin(2f * pi * (z + 2f * t)) * 0.5f;
		p.y *= 1f / 5.5f;
		p.z = z;
		return p;
	}

	static Vector3 Ripple (float x, float z, float t) {
		Vector3 p;
		float d = Mathf.Sqrt(x * x + z * z);
		p.x = x;
		p.y = Mathf.Sin(pi * (4f * d - t));
		p.y /= 1f + 10f * d;
		p.z = z;
		return p;
	}

	static Vector3 Cylinder (float u, float v, float t) {
		Vector3 p;
		float r = 0.8f + Mathf.Sin(pi * (6f * u + 2f * v + t)) * 0.2f;
		p.x = r * Mathf.Sin(pi * u);
		p.y = v;
		p.z = r * Mathf.Cos(pi * u);
		return p;
	}

	static Vector3 Sphere (float u, float v, float t) {
		Vector3 p;
		float r = 0.8f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
		r += Mathf.Sin(pi * (4f * v + t)) * 0.1f;
		float s = r * Mathf.Cos(pi * 0.5f * v);
		p.x = s * Mathf.Sin(pi * u);
		p.y = r * Mathf.Sin(pi * 0.5f * v);
		p.z = s * Mathf.Cos(pi * u);
		return p;
	}

	static Vector3 Torus (float u, float v, float t) {
		Vector3 p;
		float r1 = 0.65f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
		float r2 = 0.2f + Mathf.Sin(pi * (4f * v + t)) * 0.05f;
		float s = r2 * Mathf.Cos(pi * v) + r1;
		p.x = s * Mathf.Sin(pi * u);
		p.y = r2 * Mathf.Sin(pi * v);
		p.z = s * Mathf.Cos(pi * u);
		return p;
	}

	


}