using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class PillarMeshGeneration : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Vector3[] normals;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreatShape();
    }

    void Update()
    {
        UpadateMesh();
    }
    void CreatShape()
    {
        vertices = new Vector3[]
        {
            new Vector3 (0,0,0),
            new Vector3 (0,0,1),
            new Vector3 (1,0,0),
            new Vector3 (1,0,1),

            new Vector3 (0,1,0),
            new Vector3 (0,1,1),
            new Vector3 (1,1,0),
            new Vector3 (1,1,1)
        };//�簢������ �ð�������� ��ġ�ص� ���õ� , ������ �ﰢ������

        triangles = new int[]
        {
            0, 1, 2,            1, 3, 2//�ð���� - Normal�� ����
            //3,1,2 , 2,1,0//�ݽð���� 

            //Quad��   1 - 3 �̷��� ��������
            //         | \ |
            //         0 - 2
            //triangles = {�ð�} (0,1,2,1,3,2) OR (0,1,2,2,1,3)
        };

        CalculateTriangles(4, 2);
    }
    //========================================== �ٰ��� �ڵ� �Ųٱ� �����
    public void CalculateTriangles(int Side, int Layer)
    {
        List<int> Temp = new List<int>();

        for(int i = 0; i < triangles.Length; i++)
        {
            //Temp.Add(triangles[i]);
        }
        {
            //            0, 1, 2,
            //              1, 3, 2
            Temp.Add(0);
            Temp.Add(1);
            Temp.Add(2);
            Temp.Add(2);
            Temp.Add(1);
            Temp.Add(3);
        }//Square Button

        int index = 0;

        for (int L = 0; L < (Layer - 1); L++)
        {
            //for (int s = 0; s < (Side - 1); s++)
            {
                /*
                index = (L * Side) + s;

                // index + 0 - 1
                //         | \ |
                //      Side - Side + 1

                if (s % 2 == 0 )
                {
                    Temp.Add(index);
                    Temp.Add(index + 1);
                    Temp.Add(index + Side + 1);

                    Temp.Add(index + Side + 1);
                    Temp.Add(index + Side + 0);
                    Temp.Add(index);
                }

                // index�� 1 �϶� �밢������ ����..
                //   ������ ~ 0 �� ��
                */
            }//...

            {
                index = (L * Side) + 0;

                Temp.Add(index);
                Temp.Add(index + 1);
                Temp.Add(index + Side + 1);

                Temp.Add(index + Side + 1);
                Temp.Add(index + Side + 0);
                Temp.Add(index);

                //===

                Temp.Add(index);
                Temp.Add(index + 2);
                Temp.Add(index + Side + 2);

                Temp.Add(index + Side + 2);
                Temp.Add(index + Side + 0);
                Temp.Add(index);
            }// 0

            {
                index = (L * Side) + 3;

                Temp.Add(index);
                Temp.Add(index - 1);
                Temp.Add(index + Side - 1);

                Temp.Add(index + Side - 1);
                Temp.Add(index + Side + 0);
                Temp.Add(index);

                //===

                Temp.Add(index);
                Temp.Add(index - 2);
                Temp.Add(index + Side - 2);

                Temp.Add(index + Side - 2);
                Temp.Add(index + Side + 0);
                Temp.Add(index);
            }// 3
        }
        
        {
            //            0, 1, 2,
            //            2 , 1, 3
            Temp.Add(4);
            Temp.Add(5);
            Temp.Add(6);
            Temp.Add(6);
            Temp.Add(5);
            Temp.Add(7);
        }//Square Top
        
        triangles = new int[Temp.Count];
        triangles = Temp.ToArray();

        //normals = new Vector3[triangles.Length];
        for  (int i = 0; i < triangles.Length; i++)
        {
        //    normals[i] = (vertices[i] - Vector3.one * 0.5f).normalized;
        }
        //======================================= Normal ����
        Debug.Log(Temp.Count);
    }
    void UpadateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.normals = normals;

        //mesh.normals
        //Debug.Log("Normals : " + mesh.normals.Length);

        mesh.RecalculateNormals();//���� ������Ʈ
        mesh.Optimize();
        // ��ġ,ȸ��,������ ��������,Rigid Body ���밡��
    }
}


[UnityEditor.CustomEditor(typeof(PillarMeshGeneration))]
public class PillarMeshGenerationEditor : UnityEditor.Editor
{
    PillarMeshGeneration onwer;
    private void OnEnable()
    {
        onwer = target as PillarMeshGeneration;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("ReCalculate"))
        {
            onwer.CalculateTriangles(4, 2);
        }
    }
}
