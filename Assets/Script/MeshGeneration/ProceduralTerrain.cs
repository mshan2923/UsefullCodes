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
    Color[] colors;//---��Ʈ�������� ���̴����� Ư�� �ؽ��� ������ �ֳ�

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
    Vector3[] CalculateNormals()
    {
        Vector3[] LNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;

        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangIndex = i * 3;
            int VerIndA = triangles[normalTriangIndex];
            int VerIndB = triangles[normalTriangIndex + 1];
            int VerIndC = triangles[normalTriangIndex + 2];

            Vector3 trianleNormal = SufaceNormalFromIndices(VerIndA, VerIndB, VerIndC);
            LNormals[VerIndA] = (LNormals[VerIndA] + trianleNormal).normalized;
            LNormals[VerIndB] = (LNormals[VerIndB] + trianleNormal).normalized;
            LNormals[VerIndC] = (LNormals[VerIndC] + trianleNormal).normalized;
        }


        return LNormals;
    }
    Vector3 SufaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        Vector3 PointA = vertices[indexA];
        Vector3 pointB = vertices[indexB];
        Vector3 pointC = vertices[indexC];

        Vector3 sideAB = pointB - PointA;
        Vector3 sideAC = pointC - PointA;

        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public void UpadateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = UVs;
        mesh.colors = colors;

        //mesh.RecalculateNormals();//�Ǳ� ������
        mesh.normals = CalculateNormals();

        // ��ġ,ȸ��,������ ��������,Rigid Body ���밡��
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