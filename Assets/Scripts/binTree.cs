using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class binTree : MonoBehaviour
{
    [SerializeField]
    Mesh mesh;

    [SerializeField]
    Material material;

    [SerializeField, Range(1, 8)]
    int depth = 4;

    [SerializeField, Range(1,5)]
    int numberOfChildren = 2;

    static Vector3[] directions;

    public SteamVR_Behaviour_Pose controllerPoseL, controllerPoseR;
    public Transform head;

    [SerializeField]
    float radius = 2;

    float angle = 90f;
    static Quaternion[] rotations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f)
    };

    struct FractalPart
    {
        public int childIndx;
        public Vector3 direction;
        public Quaternion rotation;
        public Transform transform;
    }

    FractalPart[][] parts;

    private void Awake()
    {
        directions = new Vector3[numberOfChildren];
        float dist = calcDistanceBetweenControllers(controllerPoseL, controllerPoseR);
        SetDirections(controllerPoseL, controllerPoseR, radius, dist);
        SetRotations(directions, dist);

        parts = new FractalPart[depth][];
        for (int i = 0, length = 1; i < parts.Length; i++, length *= numberOfChildren)
        {
            parts[i] = new FractalPart[length];
        }

        float scale = 0.6f;
        parts[0][0] = CreatePart(0, 0, scale, false);
        for (int li = 1; li < parts.Length; li++)
        {
            scale *= 0.5f;
            FractalPart[] levelParts = parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi += numberOfChildren)
            {
                for (int ci = 0; ci < numberOfChildren; ci++)
                {
                    levelParts[fpi + ci] = CreatePart(li, ci, scale, li >1); //ci>0 => dont display first child layer
                }
            }
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float dist = calcDistanceBetweenControllers(controllerPoseL, controllerPoseR);
        SetDirections(controllerPoseL, controllerPoseR, radius, dist);
        SetRotations(directions, dist);
        Quaternion deltaRotation = Quaternion.Euler(0f, 22.5f * Time.deltaTime, 0f);

        FractalPart rootPart = parts[0][0];
        rootPart.rotation *= deltaRotation;
        //rootPart.rotation = controllerPoseR.transform.rotation;
        rootPart.transform.localRotation = rootPart.rotation;
        parts[0][0] = rootPart;

        for (int li = 1; li < parts.Length; li++)
        {
            FractalPart[] parentParts = parts[li - 1];
            FractalPart[] levelParts = parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi++)
            {
                Debug.Log("fpi=" + fpi);
                Transform parentTransform = parentParts[fpi / numberOfChildren].transform;
                FractalPart part = levelParts[fpi];
                part.direction = directions[part.childIndx];
                part.rotation *= deltaRotation;
                //part.rotation = controllerPoseR.transform.rotation;
                part.transform.localRotation = parentTransform.localRotation * part.rotation;
                part.transform.localPosition =
                    parentTransform.localPosition +
                    parentTransform.localRotation *
                //(3* dist * part.transform.localScale.x * part.direction);
                (3 * part.transform.localScale.x * part.direction);
                levelParts[fpi] = part;
                Debug.Log("direction=" + part.direction);
            }
        }
    }

    Vector3 calcDirectionsInZPlane(float angle, float radius)
    {
        return new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
    }

    float calcDistanceBetweenControllers(SteamVR_Behaviour_Pose L, SteamVR_Behaviour_Pose R)
    {
        return Vector3.Distance(L.transform.localPosition, R.transform.localPosition);
    }

    void SetDirections(SteamVR_Behaviour_Pose L, SteamVR_Behaviour_Pose R, float radius, float dist) {        
        //float dist = calcDistanceBetweenControllers(L, R);
        directions[0] = calcDirectionsInZPlane(dist, radius);
        directions[1] = new Vector3(-directions[0].x, directions[1].y, 0);
    }

    void SetRotations(Vector3[] directions, float angle)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            rotations[i] = Quaternion.AngleAxis(angle, directions[i]);
        }

    }

    FractalPart CreatePart(int levelIndex, int childIndex, float scale, bool visible)
    {
        var go = new GameObject("Fractal Part L" + levelIndex + " C" + childIndex);
        go.transform.localScale = scale * Vector3.one;
        go.transform.SetParent(transform, false);
        if (visible)
        {
            go.AddComponent<MeshFilter>().mesh = mesh;
            go.AddComponent<MeshRenderer>().material = material;
        }
        

        return new FractalPart
        {
            direction = directions[childIndex],
            rotation = rotations[childIndex],
            transform = go.transform,
            childIndx = childIndex
        };
    }
}
