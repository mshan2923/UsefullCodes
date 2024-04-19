using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class SwarmTerrain : MonoBehaviour
{
    Mesh mesh;

    [SerializeField] Vector3[] vertices;
    [SerializeField] int[] triangles;
    [SerializeField] Vector2[] UVs;
    [SerializeField] Color[] colors;

    public Vector2Int Size = Vector2Int.one * 10;
    public Vector2 Interval = Vector2.one;

    public float MinTerrainHeight = 0;
    public float MaxTerrainHeight = 1;
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0,1), new Keyframe(1,0));

    public GameObject[] TempDebugObj;

    // ** 찾을때 사각형기준 모서리만 / 중점은 주변의 버텍스 위치를 기반해서 자동 보간될꺼임 
    // **** 터레인은 상하로만 움직이고 , SwarmController로 위치를 받아서 계산해서 index를 찾음 (일일이 찾으면....)
    // ***** 이작업은 JobSystem으로 작동해서 더더욱 빠르게
    // ==> 배틀필드4 처럼 폭8시 지형 살짝 파지는것에 구현 가능 ,
    // ===>더더욱 발전시켜서 나무를 밸때 도끼 찍힌부분만 들어가면서 터렐레이션 처럼 적용

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
            }//직접 구현 - 테스트용 ()

        }//Set Triangles

        {
            UVs = new Vector2[vertices.Length];
            colors = new Color[vertices.Length];

            var ToCount = new Vector2((1 / Interval.x), (1 / Interval.y));

            for (int i = 0; i < vertices.Length; i++)
            {
                UVs[i] = new Vector2((vertices[i].x * ToCount.x) / Size.x, (vertices[i].z * ToCount.y) / Size.y);

                colors[i] = Color.white * Mathf.InverseLerp(MinTerrainHeight, MaxTerrainHeight, vertices[i].y);
                /*
                if (vertices[i].y >= MaxTerrainHeight)
                {
                    colors[i] = Color.white;
                }else if (vertices[i].y <= MinTerrainHeight)
                {
                    colors[i] = Color.red;
                }else
                {
                    colors[i] = Color.white * Mathf.InverseLerp(MinTerrainHeight, MaxTerrainHeight, vertices[i].y);
                }*/
            }
        }//Set UV
        //============== 버텍스컬러로 비활성화 지역을 투명화

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
        mesh.uv = UVs;

        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = Color.white * Mathf.InverseLerp(MinTerrainHeight, MaxTerrainHeight, vertices[i].y);
        }

        mesh.colors = colors;

        //mesh.RecalculateNormals();//조명 업데이트
        mesh.normals = CalculateNormals();

        // 위치,회전,스케일 수정가능,Rigid Body 적용가능
    }

    public Vector3 GetVertexPosition(int index)
    {
        return vertices[index];
    }
    [System.Obsolete("Change To SetVertexHeight(Vector3, Vector3, float)")]
    public void SetVertexHeight(int[] GridVertex , float height)//....GetVertexIndexs 내장해야될듯
    {
        //GridVertex Get form GetVertexIndexs , GetSelectVertexCircle

        int AreaOrigin = -1;
        Debug.Log("Length : " + GridVertex.Length);
        for (int i = 0; i < GridVertex.Length; i++)
        {
            vertices[GridVertex[i]].y = height;

            AreaOrigin = (Size.x + 1) * (Size.y + 1);
            AreaOrigin += GetAreaIndex(vertices[GridVertex[i]] + new Vector3(Interval.x, 0, Interval.y) * 0.5f);//===========되긴되지만 경계부분이...
                                                                                                                //==> VertexPos 으로 최솟 최댓값 위치 구하고 리스트 재생성
            if (AreaOrigin >= 0)
            {
                vertices[AreaOrigin].y = height;//경계부분 AreaCenterIndex는 제외 , 대신 다른곳에 인덱스를 저장 (경계는 높이를 다르게)
            }

            Debug.Log("AreaOrigin : " + AreaOrigin + "\n" + GridVertex[i] + " To " + GridIndex2World(GridVertex[i]));
        }
    }
    public void SetVertexHeight(Vector3 LocalA, Vector3 LocalB, float Height, float BorderHeightRate = 0.5f)
    {
        Vector2Int PosA = GetVertexPosition(Interval, LocalA);
        Vector2Int PosB = GetVertexPosition(Interval, LocalB);

        //GridIndex는 GetVertexIndexs , AreaCenterIndex는 
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
                    //==== 되긴되는데 경계부분만 BorderHeightRate 적용

                    
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
    }//되는데 사각형 , 원형은...

    //SetVertexHeightCircle //----> 감쇠거리.... 흠...
    /// <summary>
    /// 사다리꼴 모양
    /// </summary>
    public void SetVertexHeightCircle(Vector3 LocalA, Vector3 LocalB, float Height, float BorderHeightRate)
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
    // 경계부분 없애고, 중심부터 낮아짐
    public void SetVertexHeightCircle(Vector3 LocalA, Vector3 LocalB, float Height)
    {
        Vector2Int PosA = GetVertexPosition(Interval, LocalA);
        Vector2Int PosB = GetVertexPosition(Interval, LocalB);

        //List<int> result = new();
        Bounds SelectBounds = new Bounds()
        {
            center = new Vector3(Mathf.Abs(PosA.x + PosB.x) * 0.5f, 0, Mathf.Abs(PosA.y + PosB.y) * 0.5f),
            size = new Vector3(Mathf.Abs(PosA.x - PosB.x), 0, Mathf.Abs(PosA.y - PosB.y))
        };

        for (int y = Mathf.Min(PosA.y, PosB.y) - 1; y <= Mathf.Max(PosA.y, PosB.y); y++)
        {
            for (int x = Mathf.Min(PosA.x, PosB.x) - 1; x <= Mathf.Max(PosA.x, PosB.x); x++)
            {
                //result[i] = GetVertexPos2Index(Size, new Vector2Int(x, y));

                int vertexIndex = GetVertexPos2Index(Size, new Vector2Int(x, y));

                float temp = Math.CircleRadiusRate(SelectBounds, new Vector3(x, 0, y));
                if (temp <= 1)
                {
                    vertices[vertexIndex].y = Height * curve.Evaluate(temp);
                }


                int areaIndex = ((Size.x + 1) * (Size.y + 1)) + GetAreaIndex(vertices[vertexIndex] + (new Vector3(Interval.x, 0, Interval.y) * 0.5f));

                temp = Math.CircleRadiusRate(SelectBounds, new Vector3(x + 0.5f, 0, y + 0.5f));
                if (temp <= 1)
                {
                    vertices[areaIndex].y = Height * curve.Evaluate(temp);
                }//사각 중점
            }
        }
    }

    #region Calculate Fuction
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

        UnityEditor.EditorGUILayout.HelpBox("ToDo : SetVertexHeightCircle  감쇠거리.... 흠...", UnityEditor.MessageType.Info);

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
                        Onwer.World2LocalGrid(Onwer.TempDebugObj[1].transform.position), (Onwer.GetVertexPosition(Temp[0]).y + 0.25f));//
                    Onwer.UpadateMesh();
                }
            }
        }
    }
}