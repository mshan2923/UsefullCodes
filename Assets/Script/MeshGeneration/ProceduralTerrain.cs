using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class ProceduralTerrain : MonoBehaviour
{
    Mesh mesh;

    [SerializeField] Vector3[] vertices;
    [SerializeField] int[] triangles;
    [SerializeField] Vector2[] UVs;
    Color[] colors;//---비트연산으로 쉐이더에서 특정 텍스쳐 입힐수 있네

    [GradientUsage(true)]
    public Gradient gradient;

    float MinTerrainHeight;
    float MaxTerrainHeight;

    public Vector3Int Size = Vector3Int.one * 10;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        StartCoroutine(CreatShape());

        if (runInEditMode)
            UpadateMesh();
    }

    private void Update()
    {
        UpadateMesh();
    }
    public void Reset()
    {
        StartCoroutine(CreatShape());
        UpadateMesh();
    }

    IEnumerator CreatShape()
    {
        vertices = new Vector3[(Size.x + 1) * (Size.z + 1)];

        for (int i = 0, z = 0; z <= Size.z; z++)
        {
            for (int x = 0; x <= Size.x; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
                vertices[i] = new Vector3(x, y, z);

                {
                    if (y > MaxTerrainHeight)
                        MaxTerrainHeight = y;
                    if (y < MinTerrainHeight)
                        MinTerrainHeight = y;
                }

                i++;
            }
        }//Set Vertex Position

        {
            
            triangles = new int[Size.x * Size.z * 6];

            int vert = 0;
            int tris = 0;
            for (int z = 0; z < Size.z; z++)
            {
                for (int x = 0; x < Size.x; x++)
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + Size.x + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + Size.x + 1;
                    triangles[tris + 5] = vert + Size.x + 2;

                    vert++;
                    tris += 6;
                }
                vert++;
            }
        }//Set Triangles

        {
            UVs = new Vector2[vertices.Length];

            for(int i =  0, z = 0; z <= Size.z; z++)
            {
                for (int x = 0; x <= Size.x; x++)
                {
                    UVs[i] = new Vector2((float)x / Size.x, (float)z / Size.z);
                    i++;
                }
            }
        }//Set UV

        {
            colors = new Color[vertices.Length];

            for (int i = 0, z = 0; z <= Size.z; z++)
            {
                for (int x = 0; x <= Size.x; x++)
                {
                    float height = Mathf.InverseLerp(MinTerrainHeight, MaxTerrainHeight, vertices[i].y);
                    colors[i] = gradient.Evaluate(height);
                    i++;

                    yield return new WaitForSeconds(0.01f);
                }
            }
        }//VertexColor
    }

    public void UpadateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = UVs;
        mesh.colors = colors;

        mesh.RecalculateNormals();//조명 업데이트

        // 위치,회전,스케일 수정가능,Rigid Body 적용가능
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }
}


[UnityEditor.CustomEditor(typeof(ProceduralTerrain))]
public class ProceduralTerrainEditor : UnityEditor.Editor
{
    ProceduralTerrain Onwer;
    private void OnEnable()
    {
        Onwer = target as ProceduralTerrain;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Update"))
        {
            Onwer.Reset();
        }
    }
}