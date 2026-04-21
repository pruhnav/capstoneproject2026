using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SimpleOBJLoader
{
    public static GameObject LoadOBJ(string path)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        List<int> triangles = new List<int>();

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            if (line.StartsWith("v "))
            {
                string[] parts = line.Split(' ');
                vertices.Add(new Vector3(
                    float.Parse(parts[1]),
                    float.Parse(parts[2]),
                    float.Parse(parts[3])
                ));
            }
            else if (line.StartsWith("vt "))
            {
                string[] parts = line.Split(' ');
                uvs.Add(new Vector2(
                    float.Parse(parts[1]),
                    float.Parse(parts[2])
                ));
            }
            else if (line.StartsWith("vn "))
            {
                string[] parts = line.Split(' ');
                normals.Add(new Vector3(
                    float.Parse(parts[1]),
                    float.Parse(parts[2]),
                    float.Parse(parts[3])
                ));
            }
            else if (line.StartsWith("f "))
            {
                string[] parts = line.Split(' ');
                for (int i = 1; i <= 3; i++)
                {
                    string[] comps = parts[i].Split('/');
                    int vertIndex = int.Parse(comps[0]) - 1;
                    triangles.Add(vertIndex);
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        if (normals.Count == vertices.Count)
            mesh.SetNormals(normals);
        else
            mesh.RecalculateNormals();

        if (uvs.Count == vertices.Count)
            mesh.SetUVs(0, uvs);

        mesh.RecalculateBounds();

        GameObject obj = new GameObject(Path.GetFileNameWithoutExtension(path));
        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();

        mf.mesh = mesh;
        mr.material = new Material(Shader.Find("Standard"));

        return obj;
    }
}
