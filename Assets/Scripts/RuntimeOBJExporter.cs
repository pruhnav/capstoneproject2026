using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class RuntimeOBJExporter
{
    private struct OBJMesh
    {
        public string name;
        public Mesh mesh;
        public Material[] materials;
        public Matrix4x4 transform;
    }

    public string Export(GameObject root)
    {
        List<OBJMesh> meshes = new List<OBJMesh>();
        CollectMeshes(root, meshes);

        StringBuilder sb = new StringBuilder();
        StringBuilder mtl = new StringBuilder();

        sb.AppendLine("# Exported OBJ");
        sb.AppendLine("mtllib model.mtl");

        int vertexOffset = 1;

        foreach (var m in meshes)
        {
            sb.AppendLine($"o {m.name}");

            // Vertices
            foreach (var v in m.mesh.vertices)
            {
                Vector3 worldV = m.transform.MultiplyPoint3x4(v);
                sb.AppendLine($"v {worldV.x.ToString(CultureInfo.InvariantCulture)} {worldV.y.ToString(CultureInfo.InvariantCulture)} {worldV.z.ToString(CultureInfo.InvariantCulture)}");
            }

            // Normals
            foreach (var n in m.mesh.normals)
            {
                Vector3 worldN = m.transform.MultiplyVector(n).normalized;
                sb.AppendLine($"vn {worldN.x.ToString(CultureInfo.InvariantCulture)} {worldN.y.ToString(CultureInfo.InvariantCulture)} {worldN.z.ToString(CultureInfo.InvariantCulture)}");
            }

            // UVs
            foreach (var uv in m.mesh.uv)
            {
                sb.AppendLine($"vt {uv.x.ToString(CultureInfo.InvariantCulture)} {uv.y.ToString(CultureInfo.InvariantCulture)}");
            }

            // Submeshes
            for (int sub = 0; sub < m.mesh.subMeshCount; sub++)
            {
                Material mat = m.materials.Length > sub ? m.materials[sub] : null;
                string matName = mat != null ? mat.name : "default";

                sb.AppendLine($"usemtl {matName}");
                sb.AppendLine($"g {m.name}_{sub}");

                int[] tris = m.mesh.GetTriangles(sub);

                for (int i = 0; i < tris.Length; i += 3)
                {
                    int a = tris[i] + vertexOffset;
                    int b = tris[i + 1] + vertexOffset;
                    int c = tris[i + 2] + vertexOffset;

                    sb.AppendLine($"f {a}/{a}/{a} {b}/{b}/{b} {c}/{c}/{c}");
                }

                // Write material to MTL
                if (mat != null)
                {
                    mtl.AppendLine($"newmtl {matName}");
                    if (mat.HasProperty("_Color"))
                    {
                        Color col = mat.color;
                        mtl.AppendLine($"Kd {col.r} {col.g} {col.b}");
                    }
                    mtl.AppendLine();
                }
            }

            vertexOffset += m.mesh.vertexCount;
        }

        return sb.ToString();
    }

    private void CollectMeshes(GameObject obj, List<OBJMesh> list)
    {
        MeshFilter mf = obj.GetComponent<MeshFilter>();
        MeshRenderer mr = obj.GetComponent<MeshRenderer>();

        if (mf != null && mr != null)
        {
            OBJMesh m = new OBJMesh
            {
                name = obj.name,
                mesh = mf.sharedMesh,
                materials = mr.sharedMaterials,
                transform = obj.transform.localToWorldMatrix
            };
            list.Add(m);
        }

        foreach (Transform child in obj.transform)
            CollectMeshes(child.gameObject, list);
    }
}
