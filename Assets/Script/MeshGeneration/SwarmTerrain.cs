using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class SwarmTerrain : MonoBehaviour
{
    Mesh mesh;

    [SerializeField] Vector3[] vertices;
    [SerializeField] int[] triangles;

    public Vector2Int Size = Vector2Int.one * 10;
    public Vector2 Interval = Vector2.one;

    // ** ã���� �簢������ �𼭸��� / ������ �ֺ��� ���ؽ� ��ġ�� ����ؼ� �ڵ� �����ɲ��� 
    // **** �ͷ����� ���Ϸθ� �����̰� , SwarmController�� ��ġ�� �޾Ƽ� ����ؼ� index�� ã�� (������ ã����....)
    // ***** ���۾��� JobSystem���� �۵��ؼ� ������ ������
    // ==> ��Ʋ�ʵ�4 ó�� ��8�� ���� ��¦ �����°Ϳ� ���� ���� ,
    // ===>������ �������Ѽ� ������ �붧 ���� �����κи� ���鼭 �ͷ����̼� ó�� ����

    void OnEnable()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //CreateTerrain();
        CreateSwarm();
        UpadateMesh();
    }

    void Update()
    {
        
    }
    public void CreateSwarm()
    {
        vertices = new Vector3[(Size.x + 1) * (Size.y + 1) + Size.x * Size.y];

        {
            int i = 0;
            for (int z = 0; z <= Size.y; z++)
            {
                for (int x = 0; x <= Size.x; x++)
                {
                    vertices[i] = new Vector3(x * Interval.x, 0, z * Interval.y);
                    i++;
                }
            }
            for (int z = 0; z < Size.y; z++)
            {
                for (int x = 0; x < Size.x; x++)
                {
                    vertices[i] = new Vector3((2 * x + 1) * 0.5f * Interval.x, 0, (2 * z + 1) * 0.5f * Interval.y);
                    i++;
                }
            }
        }//Set Position

        {
            //x+1 -- x+2
            // |  n  |
            // 0 --  1
            triangles = new int[Size.x * Size.y * 12];

            int vert = 0;
            int tris = 0;
            int GridEnd = (Size.x + 1) * (Size.y + 1);
            for (int i = 0, z = 0; z < Size.y; z++)
            {
                for (int x = 0; x < Size.x; x++, i++)
                {
                    
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + Size.x + 1;
                    triangles[tris + 2] = i + GridEnd;

                    triangles[tris + 3] = vert + Size.x + 1;
                    triangles[tris + 4] = vert + Size.x + 2;
                    triangles[tris + 5] = i + GridEnd;
                    
                    triangles[tris + 6] = vert + Size.x + 2;
                    triangles[tris + 7] = vert + 1;
                    triangles[tris + 8] = i + GridEnd;

                    triangles[tris + 9] = vert + 1;
                    triangles[tris + 10] = vert + 0;
                    triangles[tris + 11] = i + GridEnd;

                    vert++;
                    tris += 12;
                }
                vert++;
            }

            {
                /*
                vert = 0;
                triangles[0] = vert + 0;
                triangles[1] = vert + Size.x + 1;
                triangles[2] = vert + GridEnd;

                triangles[3] = vert + Size.x + 1;
                triangles[4] = vert + Size.x + 2;
                triangles[5] = vert + GridEnd;

                triangles[6] = vert + Size.x + 2;
                triangles[7] = vert + 1;
                triangles[8] = vert + GridEnd;

                triangles[9] = vert + 1;
                triangles[10] = vert + 0;
                triangles[11] = vert + GridEnd;

                
                vert += 2;
                triangles[12] = vert + 0;
                triangles[13] = vert + Size.x + 1;
                triangles[14] = vert + GridEnd;
                
                triangles[15] = vert + Size.x + 1;
                triangles[16] = vert + Size.x + 2;
                triangles[17] = vert + GridEnd;

                triangles[18] = vert + Size.x + 2;
                triangles[19] = vert + 1;
                triangles[20] = vert + GridEnd;

                triangles[21] = vert + 1;
                triangles[22] = vert + 0;
                triangles[23] = vert + GridEnd;
                */
            }//���� ���� - �׽�Ʈ�� ()

        }//Set Triangles

    }//������ �ѹ��� �ϸ� �Ǵϱ� + Job���� ���ϴ°�

    void CreateTerrain()
    {
        if (vertices.Length != ((Size.x + 1) * (Size.y + 1)))
        {
            vertices = new Vector3[(Size.x + 1) * (Size.y + 1)];
        }

        for (int i = 0, z = 0; z <= Size.y; z++)
        {
            for (int x = 0; x <= Size.x; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        {
            if (triangles.Length != (Size.x * Size.y * 6))
            {
                triangles = new int[Size.x * Size.y * 6];
            }

            int vert = 0;
            int tris = 0;
            for (int z = 0; z < Size.y; z++)
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
        }
    }//������ ���
    public void UpadateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();//���� ������Ʈ

        // ��ġ,ȸ��,������ ��������,Rigid Body ���밡��
    }

    public int GetVertexIndex(Vector3 LocalPosition)
    {
        return SwarmTerrain.GetVertexIndex(Size, Interval, LocalPosition);
    }
    public int GetAreaIndex(Vector3 LocalPosition)
    {
        return SwarmTerrain.GetAreaIndex(Size, Interval, LocalPosition);
    }
    /// <summary>
    /// LocalPosition is Origin : Vertex[0]
    /// </summary>
    public static int GetVertexIndex(Vector2Int Size, Vector2 Interval, Vector3 LocalPosition)
    {
        int x = Mathf.RoundToInt((LocalPosition.x) / Interval.x);
        int z = Mathf.RoundToInt((LocalPosition.z) / Interval.y);//(Interval.x * 0.5f)

        Debug.Log(LocalPosition + "\n" + x + ", 0, " + z);
        if (x <= Size.x && z <= Size.y)
        {
            return (Size.x + 1) * z + x;
        }
        else
        {
            return -1;
        }
    }
    /// <summary>
    /// �߽��� ã���� ����
    /// </summary>
    public static int GetAreaIndex(Vector2Int Size, Vector2 Interval, Vector3 LocalPosition)
    {
        int x = Mathf.RoundToInt((LocalPosition.x - (Interval.x * 0.5f)) / Interval.x);
        int z = Mathf.RoundToInt((LocalPosition.z - (Interval.y * 0.5f)) / Interval.y);


        if (x < Size.x && z < Size.y)
        {
            return Size.x * z + x;
        }
        else
        {
            return -1;
        }

        //����� ���ý��� �ƴ϶� ���° �������� ����
    }
}

[UnityEditor.CustomEditor(typeof(SwarmTerrain))]
public class SwarmTerrainEditor : UnityEditor.Editor
{
    SwarmTerrain Onwer;
    private void OnEnable()
    {
        Onwer = target as SwarmTerrain;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("ReCalculate"))
        {
            Onwer.CreateSwarm();
            Onwer.UpadateMesh();
        }

        if (GUILayout.Button("Get Index"))
        {
            Debug.Log("Vertex : " + SwarmTerrain.GetVertexIndex(Onwer.Size, Onwer.Interval, Vector3.zero - Onwer.transform.position));
        }
    }
}