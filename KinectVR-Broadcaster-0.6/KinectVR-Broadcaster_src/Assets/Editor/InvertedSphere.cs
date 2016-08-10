using UnityEngine;
using UnityEditor;

public class InvertedSphere : EditorWindow
{
    private string st = "1.0";

    [MenuItem("GameObject/Create Other/Inverted Sphere...")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(InvertedSphere));
    }

    public void OnGUI()
    {
        GUILayout.Label("Enter sphere size:");
        st = GUILayout.TextField(st);

        float f;
        if (!float.TryParse(st, out f))
            f = 1.0f;
        if (GUILayout.Button("Create Inverted Sphere"))
        {
            CreateInvertedSphere(f);
        }
    }

    private void CreateInvertedSphere(float size)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        MeshFilter mf = go.GetComponent<MeshFilter>();
        Mesh mesh = mf.sharedMesh;

        GameObject goNew = new GameObject();
        goNew.name = "Inverted Sphere";
        MeshFilter mfNew = goNew.AddComponent<MeshFilter>();
        mfNew.sharedMesh = new Mesh();


        //Scale the vertices;
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = vertices[i] * size;
        mfNew.sharedMesh.vertices = vertices;

        // Reverse the triangles
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int t = triangles[i];
            triangles[i] = triangles[i + 2];
            triangles[i + 2] = t;
        }
        mfNew.sharedMesh.triangles = triangles;

        // Reverse the normals;
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] = -normals[i];
        mfNew.sharedMesh.normals = normals;


        mfNew.sharedMesh.uv = mesh.uv;
        mfNew.sharedMesh.uv2 = mesh.uv2;
        mfNew.sharedMesh.RecalculateBounds();

        // Add the same material that the original sphere used
        MeshRenderer mr = goNew.AddComponent<MeshRenderer>();
        mr.sharedMaterial = go.GetComponent<Renderer>().sharedMaterial;

        DestroyImmediate(go);
    }
}