using UnityEngine;
using System.Collections;

public enum ArcType
{
	Parabolic,
	Rocket,
	Square,
	Knuckle
}

public class ArcVertexGenerator : MonoBehaviour
{
	private Vector3[] vertices;
	private Mesh mesh;

	private int ySize = 1;
	private int xSize = 15;


	void Awake()
	{
		GenerateMesh();
	}

	void GenerateMesh()
	{
		vertices = new Vector3[(xSize+1) * (ySize+1)];
		int[] triangles = new int[xSize * 6];

		for (int ti = 0, vi = 0, x = 0; x < xSize; x++, ti += 6, vi++)
		{
			triangles[ti] = vi;
			triangles[ti + 3] = triangles[ti + 2] = vi + 1;
			triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
			triangles[ti + 5] = vi + xSize + 2;
		}

		for (int i = 0, y = 0; y <= ySize; y++)
		{
			for (int x = 0; x <= xSize; x++, i++)
			{
				//vertices[i] = new Vector3(x, y);
				vertices[i] = new Vector3(x, y+0.5f);
			}
		}



		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Grid";

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}


	void OnDrawGizmos()
	{
		if (vertices == null)
			return;

		Gizmos.color = Color.black;

		for (int i = 0; i < vertices.Length; i++)
		{
			//Gizmos.DrawSphere(vertices[i], 0.1f);
		}
	}
}
