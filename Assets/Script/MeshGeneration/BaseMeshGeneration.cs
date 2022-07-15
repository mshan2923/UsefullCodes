using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BaseMeshGeneration : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreatShape();
        UpadateMesh();
    }

    void CreatShape()
    {
        vertices = new Vector3[]
        {
            new Vector3 (0,0,0),
            new Vector3 (0,0,1),
            new Vector3 (1,0,0),
            new Vector3 (1,0,1)
        };

        triangles = new int[]
        {
            0, 1, 2,
            1, 3, 2

            //Quad가   1 - 3 이렇게 생겼을때
            //         | \ |
            //         0 - 2
            //triangles = {시계} (0,1,2,1,3,2) OR (0,1,2,2,1,3)
        };
    }

    void UpadateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();//조명 업데이트

        // 위치,회전,스케일 수정가능,Rigid Body 적용가능
    }

}
