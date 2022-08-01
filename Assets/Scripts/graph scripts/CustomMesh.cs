using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CustomMesh : MonoBehaviour
{

    public int xSize, ySize, zSize;



	private Mesh mesh;
	private Vector3[] vertices;

    const float pi = Mathf.PI;

    float step;

    //private Mesh meshOtherSide;

	private void Awake()
	{
        step = 2f / (xSize+ 1);
        Generate();

     //   UpdateVertices();
	}


	private void Generate()
	{
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Grid";

        //GetComponent<MeshFilter>().mesh = meshOtherSide = new Mesh();

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		Vector2[] uv = new Vector2[vertices.Length];
		Vector4[] tangents = new Vector4[vertices.Length];
		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
		for (int i = 0, y = 0; y <= ySize; y++)
		{
			for (int x = 0; x <= xSize; x++, i++)
			{
				vertices[i] = new Vector3(x, y ,1f);
				uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
				tangents[i] = tangent;
			}
		}
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.tangents = tangents;

		int[] triangles = new int[xSize * ySize * 6*2];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
		{
			for (int x = 0; x < xSize; x++, ti += 6, vi++)
			{
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		}
        //
        int startingPoint = (xSize * ySize * 6);
        int endingPoint = xSize * ySize * 6 * 2;
        for (int t2 = startingPoint, i = 0; t2 < endingPoint; t2 += 3)
        {
            for (int k = 0; k < 3; k++)
                if (i % 3 == 0)
                {
                    triangles[t2] = triangles[i];
                }
                else if (i % 3 == 1)
                {
                    triangles[t2 + 2] = triangles[i];
                }
                else if (i % 3 == 2)
                {
                    triangles[t2 + 1] = triangles[i];
                }
                else { Debug.Log("you did something wrong"); }
            i++;
        }
        mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}


    void UpdateVertices()
    {
        float t = Time.time;
        Vector3[] UpdatingVertices = mesh.vertices;

        for (int i = 0, z = 0; z <= xSize; z++)
        {
            float v = (z + 0.5f) * step - 1f;
            for (int x = 0; x <= ySize; x++, i++)
            {
                float u = (x + 0.5f) * step - 1f;
                UpdatingVertices[i] = figure8(u, v, t);
            }
        }
        mesh.vertices = UpdatingVertices;
        mesh.RecalculateNormals();

    }

    Vector3 figure8(float u, float v, float t)
    {
        Vector3 p;
        float r = 3;
        u *= pi;
        v *= pi;
        p.x = (r + Mathf.Cos(u / 2f) * Mathf.Sin(v) - Mathf.Sin(u / 2f) * Mathf.Sin(2 * v)) * Mathf.Cos(u);
        p.y = (r + Mathf.Cos(u / 2f) * Mathf.Sin(v) - Mathf.Sin(u / 2f) * Mathf.Sin(2 * v)) * Mathf.Sin(u);
        p.z = Mathf.Sin(u / 2f) * Mathf.Sin(v) + Mathf.Cos(u / 2f) * Mathf.Sin(2 * v);
        return p;
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}