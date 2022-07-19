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

    public GameObject[] TempDebugObj;

    // ** ã���� �簢������ �𼭸��� / ������ �ֺ��� ���ؽ� ��ġ�� ����ؼ� �ڵ� �����ɲ��� 
    // **** �ͷ����� ���Ϸθ� �����̰� , SwarmController�� ��ġ�� �޾Ƽ� ����ؼ� index�� ã�� (������ ã����....)
    // ***** ���۾��� JobSystem���� �۵��ؼ� ������ ������
    // ==> ��Ʋ�ʵ�4 ó�� ��8�� ���� ��¦ �����°Ϳ� ���� ���� ,
    // ===>������ �������Ѽ� ������ �붧 ���� �����κи� ���鼭 �ͷ����̼� ó�� ����

    void OnEnable()
    {
        mesh = new Mesh();

        if (runInEditMode)
        {
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }else
        {
            GetComponent<MeshFilter>().mesh = mesh;
        }

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

    public Vector3 GetVertexPosition(int index)
    {
        return vertices[index];
    }
    //============ ���ý��� �����϶� �簢���� ������ ���������ϴϱ� ...
    [System.Obsolete("Change To SetVertexHeight(Vector3, Vector3, float)")]
    public void SetVertexHeight(int[] GridVertex , float height)//....GetVertexIndexs �����ؾߵɵ�
    {
        //GridVertex Get form GetVertexIndexs , GetSelectVertexCircle

        int AreaOrigin = -1;
        Debug.Log("Length : " + GridVertex.Length);
        for (int i = 0; i < GridVertex.Length; i++)
        {
            vertices[GridVertex[i]].y = height;

            AreaOrigin = (Size.x + 1) * (Size.y + 1);
            AreaOrigin += GetAreaIndex(vertices[GridVertex[i]] + new Vector3(Interval.x, 0, Interval.y) * 0.5f);//===========�Ǳ������ ���κ���...
                                                                                                                //==> VertexPos ���� �ּ� �ִ� ��ġ ���ϰ� ����Ʈ �����
            if (AreaOrigin >= 0)
            {
                vertices[AreaOrigin].y = height;//���κ� AreaCenterIndex�� ���� , ��� �ٸ����� �ε����� ���� (���� ���̸� �ٸ���)
            }

            Debug.Log("AreaOrigin : " + AreaOrigin + "\n" + GridVertex[i] + " To " + GridIndex2World(GridVertex[i]));
        }
    }
    public void SetVertexHeight(Vector3 LocalA, Vector3 LocalB, float Height, float BorderHeightRate = 0.5f)
    {
        Vector2Int PosA = GetVertexPosition(Interval, LocalA);
        Vector2Int PosB = GetVertexPosition(Interval, LocalB);

        //GridIndex�� GetVertexIndexs , AreaCenterIndex�� 
        //int[] result = new int[(Mathf.Abs(PosA.x - PosB.x) + 1) * (Mathf.Abs(PosA.y - PosB.y) + 1)];

        for (int y = Mathf.Min(PosA.y, PosB.y) - 1; y <= Mathf.Max(PosA.y, PosB.y); y++)
        {
            for (int x = Mathf.Min(PosA.x, PosB.x) - 1; x <= Mathf.Max(PosA.x, PosB.x); x++)
            {
                if (x >= 0 && y >= 0)
                {
                    int vertexIndex = GetVertexPos2Index(Size, new Vector2Int(x, y));

                    if (y >= Mathf.Min(PosA.y, PosB.y) && x >= Mathf.Min(PosA.x, PosB.x))
                        vertices[vertexIndex].y = Height;

                    int areaIndex = ((Size.x + 1) * (Size.y + 1)) + GetAreaIndex(vertices[vertexIndex] + (new Vector3(Interval.x, 0, Interval.y) * 0.5f));
                    //==== �Ǳ�Ǵµ� ���κи� BorderHeightRate ����

                    
                    if (Math.InRange(x, (Mathf.Min(PosA.x, PosB.x) - 1), (Mathf.Max(PosA.x, PosB.x)), false) 
                        && Math.InRange(y, (Mathf.Min(PosA.y, PosB.y) - 1), (Mathf.Max(PosA.y, PosB.y)), false))
                    {
                        vertices[areaIndex].y = Height;
                    }
                    else
                    {
                        vertices[areaIndex].y = Height * BorderHeightRate;
                    }

                    //Debug.Log("Vertex : " + vertexIndex + " / Area : " + areaIndex);
                }
            }
        }//Vertex
    }//�Ǵµ� �簢�� , ������...

    //SetVertexHeightCircle //----> GetSelectVertexCircle �� ������ ��� Vertex �Ÿ� ��.... �׽�Ʈ �ʿ�
    public void SetVertexHeightCircle(Vector3 LocalA, Vector3 LocalB, float Height, float BorderHeightRate = 0.5f)
    {
        Vector2Int PosA = GetVertexPosition(Interval, LocalA);
        Vector2Int PosB = GetVertexPosition(Interval, LocalB);

        List<int> result = new();
        Bounds SelectBounds = new Bounds()
        {
            center = new Vector3(Mathf.Abs(PosA.x + PosB.x) * 0.5f, 0, Mathf.Abs(PosA.y + PosB.y) * 0.5f),
            size = new Vector3(Mathf.Abs(PosA.x - PosB.x), 0, Mathf.Abs(PosA.y - PosB.y))
        };
        Bounds BorderBound = new Bounds()
        {
            center = SelectBounds.center,
            size = SelectBounds.size + new Vector3(1, 0, 1)//new Vector3(Interval.x, 0, Interval.y)
        };

        for (int y = Mathf.Min(PosA.y, PosB.y) - 1; y <= Mathf.Max(PosA.y, PosB.y); y++)
        {
            for (int x = Mathf.Min(PosA.x, PosB.x) - 1; x <= Mathf.Max(PosA.x, PosB.x); x++)
            {
                //result[i] = GetVertexPos2Index(Size, new Vector2Int(x, y));

                int vertexIndex = GetVertexPos2Index(Size, new Vector2Int(x, y));

                if (Math.InCircle(SelectBounds, new Vector3(x, 0, y)))
                {
                    vertices[vertexIndex].y = Height;

                }
                else if (Math.InCircle(BorderBound, new Vector3(x, 0, y)))
                {
                    vertices[vertexIndex].y = Height * BorderHeightRate;
                }


                int areaIndex = ((Size.x + 1) * (Size.y + 1)) + GetAreaIndex(vertices[vertexIndex] + (new Vector3(Interval.x, 0, Interval.y) * 0.5f));

                if (Math.InCircle(SelectBounds, new Vector3(x + 0.5f, 0, y + 0.5f)))
                {
                    vertices[areaIndex].y = Height;
                }else if (Math.InCircle(BorderBound, new Vector3(x + 0.5f, 0, y + 0.5f)))
                {
                    vertices[areaIndex].y = Height * BorderHeightRate;
                }
            }
        }
    }

    #region Calculate Fuction
    public int GetVertexIndex(Vector3 LocalPosition)
    {
        return SwarmTerrain.GetVertexIndex(Size, Interval, LocalPosition);
    }
    public int GetAreaIndex(Vector3 LocalPosition)
    {
        return SwarmTerrain.GetAreaIndex(Size, Interval, LocalPosition);
    }
    /// <summary>
    /// Select Pos is Local Position
    /// </summary>
    public int[] GetVertexIndexs(Vector3 SelectA, Vector3 SelectB)
    {
        Vector2Int PosA = GetVertexPosition(Interval, SelectA);
        Vector2Int PosB = GetVertexPosition(Interval, SelectB);

        //bool Reverse = GetVertexPos2Index(Size, PosA) > GetVertexPos2Index(Size, PosB);

        int[] result = new int[(Mathf.Abs(PosA.x - PosB.x) + 1) * (Mathf.Abs(PosA.y - PosB.y) + 1)];
        for(int i = 0, y = Mathf.Min(PosA.y, PosB.y); y <= Mathf.Max(PosA.y, PosB.y); y++)
        {
            for (int x = Mathf.Min(PosA.x, PosB.x); x <= Mathf.Max(PosA.x, PosB.x); x++)
            {
                result[i] = GetVertexPos2Index(Size, new Vector2Int(x, y));
                i++;
            }
        }
        
        return result;
    }
    public List<int> GetSelectVertexCircle(Vector3 SelectA, Vector3 SelectB)
    {
        //var Temp = GetVertexIndexs(SelectA, SelectB);

        Vector2Int PosA = GetVertexPosition(Interval, SelectA);
        Vector2Int PosB = GetVertexPosition(Interval, SelectB);

        List<int> result = new();
        Bounds bounds = new Bounds()
        {
            center = new Vector3(Mathf.Abs(PosA.x + PosB.x) * 0.5f, 0, Mathf.Abs(PosA.y + PosB.y) * 0.5f),
            size = new Vector3(Mathf.Abs(PosA.x - PosB.x) * 1f, 0, Mathf.Abs(PosA.y - PosB.y) * 1f)
        };

        for (int i = 0, y = Mathf.Min(PosA.y, PosB.y); y <= Mathf.Max(PosA.y, PosB.y); y++)
        {
            for (int x = Mathf.Min(PosA.x, PosB.x); x <= Mathf.Max(PosA.x, PosB.x); x++)
            {
                //result[i] = GetVertexPos2Index(Size, new Vector2Int(x, y));
                if (Math.InCircle(bounds, new Vector3(x, 0, y)))
                {
                    result.Add(GetVertexPos2Index(Size, new Vector2Int(x, y)));
                }
                i++;
            }
        }

        return result;
    }
    public Vector3 World2LocalGrid(Vector3 Pos)
    {
        return Pos - new Vector3(vertices[0].x, 0, vertices[0].z) - gameObject.transform.position;
    }
    public Vector3 LocalGrid2World(Vector3 Pos)
    {
        return Pos + new Vector3(vertices[0].x, 0, vertices[0].z) + gameObject.transform.position;
    }
    public Vector3 GridIndex2World(int index)
    {
        return vertices[index] + gameObject.transform.position;
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
    public static Vector2Int GetVertexPosition(Vector2 Interval, Vector3 LocalPosition)
    {
        //int x = Mathf.RoundToInt((LocalPosition.x) / Interval.x);
        //int z = Mathf.RoundToInt((LocalPosition.z) / Interval.y);

        //Debug.Log(LocalPosition + "\n" + x + ", 0, " + z);
        return new Vector2Int(Mathf.RoundToInt((LocalPosition.x) / Interval.x), Mathf.RoundToInt((LocalPosition.z) / Interval.y));
    }

    public static int GetVertexPos2Index(Vector2Int Size, Vector2Int Pos)
    {
        return (Size.x + 1) * Pos.y + Pos.x;
    }
    public static Vector2Int Index2VertexPos(Vector2Int Size, int index)
    {
        return new Vector2Int((index % (Size.x + 1)), (index / (Size.x + 1)));
    }
    #endregion
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
            if (Onwer.TempDebugObj[0] != null)
            {
                int temp = SwarmTerrain.GetVertexIndex(Onwer.Size, Onwer.Interval, Onwer.World2LocalGrid(Onwer.TempDebugObj[0].transform.position));
                Debug.Log("Vertex : " + temp + " / VertexPos : " + SwarmTerrain.Index2VertexPos(Onwer.Size, temp) + "\n " +
                    SwarmTerrain.GetVertexPos2Index(Onwer.Size, SwarmTerrain.Index2VertexPos(Onwer.Size, temp)));

            }
            //Index2VertexPos
        }
        if (GUILayout.Button("Get Indexs"))
        {
            if (Onwer.TempDebugObj.Length >= 2)
            {
                if (Onwer.TempDebugObj[0] != null && Onwer.TempDebugObj[1] != null)
                {
                    //World2LocalGrid

                    /*
                    var Temp = Onwer.GetVertexIndexs(Onwer.World2LocalGrid(Onwer.TempDebugObj[0].transform.position),
                        Onwer.World2LocalGrid(Onwer.TempDebugObj[1].transform.position));
                    string result = "Vertexs : ";
                    for (int i = 0; i < Temp.Length; i++)
                    {
                        result += Temp[i] + ", ";
                    }
                    Debug.Log("Length : " + Temp.Length + "\n" + result);
                    */

                    var Temp = Onwer.GetSelectVertexCircle(Onwer.World2LocalGrid(Onwer.TempDebugObj[0].transform.position),
                                    Onwer.World2LocalGrid(Onwer.TempDebugObj[1].transform.position));

                    for (int i = 0; i < Temp.Count; i++)
                    {
                        Debug.DrawLine(Onwer.GridIndex2World(Temp[i]), Onwer.GridIndex2World(Temp[i]) + Vector3.up, Color.red, 10f);
                    }
                    //GetSelectVertexCircle

                    Onwer.SetVertexHeightCircle(Onwer.World2LocalGrid(Onwer.TempDebugObj[0].transform.position),
                        Onwer.World2LocalGrid(Onwer.TempDebugObj[1].transform.position), Onwer.GetVertexPosition(Temp[0]).y + 0.25f);
                    Onwer.UpadateMesh();
                }
            }
        }
    }
}