using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class treeGenerator : MonoBehaviour
{
    [SerializeField]
    Mesh mesh;

    [SerializeField]
    Material material;

    [SerializeField, Range(1, 8)]
    int depth = 4;

    static Vector3[] directions = {
        Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
    };

    static Vector3[] directions2 = new Vector3[3];
    static Quaternion[] rotations2 = new Quaternion[2];

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

    public SteamVR_Behaviour_Pose controllerPoseL, controllerPoseR;
    public Transform head;
    int numberOfChildren = 2;

    private void Awake()
    {
        float dist = Vector3.Distance(controllerPoseL.transform.localPosition, controllerPoseR.transform.localPosition);
        Vector3 dir = Vector3.Normalize(new Vector3(Mathf.Sin(dist), Mathf.Cos(dist), 0));
        directions2[0]= Vector3.up + dir;
        directions2[1] = Vector3.up -dir;
        directions2[2] = Vector3.Normalize(new Vector3(0, Mathf.Sin(dist), Mathf.Cos(dist)));


        parts = new FractalPart[depth][];
        for (int i = 0, length = 1; i < parts.Length; i++, length *= numberOfChildren)
        {
            parts[i] = new FractalPart[length];
        }

        float scale = 1f;
        parts[0][0] = CreatePart(0, 0, scale);
        for (int li = 1; li < parts.Length; li++)
        {
            scale *= 0.5f;
            FractalPart[] levelParts = parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi += numberOfChildren)
            {
                for (int ci = 0; ci < numberOfChildren; ci++)
                {
                    levelParts[fpi + ci] = CreatePart(li, ci, scale);
                }
            }
        }
    }
    void Start()
    {
        name = "Fractal " + depth;

        if (depth <= 1)
        {
            return;
        }


    }

    void Update()
    {
        /*  directions2[0] = GetDirections(controllerPoseL, controllerPoseR);
          directions2[1] = -1f * directions2[0];*/
        float dist = Vector3.Distance(controllerPoseL.transform.position, controllerPoseR.transform.position);

        Vector3 dir = new Vector3(Mathf.Sin(dist), Mathf.Cos(dist), 0);

        directions2[0] = dir;
        directions2[1] = new Vector3(-Mathf.Sin(dist), Mathf.Cos(dist), 0);
        directions2[2] = Vector3.Normalize(new Vector3(0, Mathf.Sin(dist), Mathf.Cos(dist)));

        Quaternion deltaRotation = Quaternion.Euler(0f, 22.5f * Time.deltaTime, 0f);
        //Quaternion deltaRotation = Quaternion.identity;
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
                part.direction = directions2[part.childIndx];
                part.rotation *= deltaRotation;
                //part.rotation = controllerPoseR.transform.rotation;
                part.transform.localRotation = parentTransform.localRotation * part.rotation;
                part.transform.localPosition =
                    parentTransform.localPosition +
                    parentTransform.localRotation *
                        //(3* dist * part.transform.localScale.x * part.direction);
                (3  * part.transform.localScale.x * part.direction);
                levelParts[fpi] = part;
                Debug.Log("direction=" + part.direction);
            }
        }
    }

    /*treeGenerator CreateChild(Vector3 direction, Quaternion rotation)
    {
        treeGenerator child = Instantiate(this);
        child.depth = depth - 1;
        child.transform.localPosition = 0.75f * direction;
        child.transform.localRotation = rotation;
        child.transform.localScale = 0.5f * Vector3.one;
        return child;
    }*/

        FractalPart CreatePart(int levelIndex, int childIndex, float scale)
    {
        var go = new GameObject("Fractal Part L" + levelIndex + " C" + childIndex);
        go.transform.localScale = scale * Vector3.one;
        go.transform.SetParent(transform, false);
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>().material = material;

        return new FractalPart
        {
            //direction = directions[childIndex],
            direction = directions2[childIndex],
            rotation = rotations[childIndex],
            transform = go.transform,
            childIndx = childIndex
        };
    }

    
}
