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

    IEnumerator CreatShape()
    {
        vertices = new Vector3[(Size.x + 1) * (Size.z + 1)];

        for (int i = 0, z = 0; z <= Size.z; z++)
        {
            for (int x = 0; x <= Size.x; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

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

                    yield return new WaitForSeconds(0.01f);
                }
                vert++;
            }
        }
    }

    void UpadateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

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
