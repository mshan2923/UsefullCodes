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

    // ** 찾을때 사각형기준 모서리만 / 중점은 주변의 버텍스 위치를 기반해서 자동 보간될꺼임 
    // **** 터레인은 상하로만 움직이고 , SwarmController로 위치를 받아서 계산해서 index를 찾음 (일일이 찾으면....)
    // ***** 이작업은 JobSystem으로 작동해서 더더욱 빠르게
    // ==> 배틀필드4 처럼 폭8시 지형 살짝 파지는것에 구현 가능 ,
    // ===>더더욱 발전시켜서 나무를 밸때 도끼 찍힌부분만 들어가면서 터렐레이션 처럼 적용

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
            }//직접 구현 - 테스트용 ()

        }//Set Triangles

    }//생성은 한번만 하면 되니까 + Job에서 못하는거

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
    }//보통의 방법
    public void UpadateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();//조명 업데이트

        // 위치,회전,스케일 수정가능,Rigid Body 적용가능
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
    /// 중심점 찾을때 쓰임
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

        //가까운 버택스가 아니라 몇번째 면인지로 나옴
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