using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerMeshGenerator
{
	public void GenerateMesh(List<Vector3> points)
	{
		// Generate vertices
		Vector3[] vertices = new Vector3[points.Count * 2];
		for (int i = 0, ind = 0; i < points.Count-1; i++, ind += 2)
		{
			Vector3 diff = points[i+1] - points[i];
			Vector3 cross = Vector3.Cross(diff, Vector3.up).normalized * 0.5f;

			vertices[i] = points[i] - cross;
			vertices[points.Count + i] = points[i] + cross;

			Debug.DrawLine(vertices[i], vertices[points.Count + i]);
			//vertices[i] = points[i];
			//vertices[points.Count + i] = points[i];
		}

		// Generate triangles
		int xSize = points.Count-2;
		int ySize = 1;

		int[] triangles = new int[(points.Count) * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		}


		// Find mesh
		Mesh mesh = GameObject.FindGameObjectWithTag("ArcMesh").GetComponent<MeshFilter>().mesh = new Mesh();
		mesh.name = "Procedural Arc Mesh";

		mesh.vertices = vertices;
		mesh.triangles = triangles;

		mesh.RecalculateNormals();
	}
}
