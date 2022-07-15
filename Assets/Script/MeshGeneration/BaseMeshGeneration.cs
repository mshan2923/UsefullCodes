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

            //Quad��   1 - 3 �̷��� ��������
            //         | \ |
            //         0 - 2
            //triangles = {�ð�} (0,1,2,1,3,2) OR (0,1,2,2,1,3)
        };
    }

    void UpadateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();//���� ������Ʈ

        // ��ġ,ȸ��,������ ��������,Rigid Body ���밡��
    }

}
