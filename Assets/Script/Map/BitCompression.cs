using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class BitCompression// : MonoBehaviour
{
    /*
    public List<List<int>> Example = new();
    private void Start()
    {
        Example = BitCompression_Inti(Vector2.one * 50);

    }
    
    void Update()
    {
        // 1<<31 되면 음수값인데? , 모두 더하면(0~31) : -1 
        //  1 << (0 ~ 30) 을 전부 더하면 int.max

        Example = BitCompression_Inti(Vector2.one * 50);

        BC_Set(ref Example, Vector2Int.one, true);

        Debug.Log(BC_Get(Example, Vector2Int.one));

        BC_Set(ref Example, Vector2Int.one , false);

        Debug.Log(BC_Get(Example, Vector2Int.one));


        BC_SetRange(ref Example, Vector2Int.zero, Vector2Int.one * 35, false);

        Debug.Log(BC_Get(Example, Vector2Int.one * 2));
        Debug.Log(BC_Get(Example, Vector2Int.one * 35));

        {

            BC_Set(ref Example, Vector2Int.one * 2, true);
            BC_Reverse(ref Example);

            Debug.Log(BC_Get(Example, Vector2Int.one * 2));
            Debug.Log(BC_Get(Example, Vector2Int.one * 35));
        }
    }*///Test Code (MonoBehaviour)

    public static List<List<int>> BitCompression_Inti(Vector2 Size, bool Full = true)
    {
        List<List<int>> Result = new();
        for (int x = 0; x < Size.x; x++)
        {
            Result.Add(new List<int>());
            for (int y = 0; y <= (Size.y / 31); y++)
            {
                if (Full)
                {
                    Result[Result.Count - 1].Add(int.MaxValue);
                }else
                {
                    Result[Result.Count - 1].Add(0);
                }
            }
        }
        return Result;
    }
    public static void BC_Clear(ref List<List<int>> list, bool Full = true)
    {
        for (int x = 0; x < list.Count; x++)
        {
            list.Add(new List<int>());
            for (int y = 0; y <= list[x].Count; y++)
            {
                if (Full)
                {
                    list[x].Add(int.MaxValue);
                }
                else
                {
                    list[x].Add(0);
                }
            }
        }
    }

    public static int BC_Get(List<List<int>> list, Vector2Int index)
    {
        if (list != null)
        {
            //Debug.Log("Size : " + index + " / y =" + (index.y / 31) + " * 31 + " + (index.y % 31));
            return list[index.x][index.y / 31] & (1 << (index.y % 31));
        }else
        {
            Debug.LogAssertion("Null");
            return 0;
        }
    }
    public static void BC_Set(ref List<List<int>> list, Vector2Int index, bool Vaule)
    {
        if (Vaule)
        {
            list[index.x][index.y / 31] |= (1 << (index.y % 31));
        }
        else
        {
            list[index.x][index.y / 31] &= ~(1 << (index.y % 31));
        }
    }
    public static void BC_SetRange(ref List<List<int>> list, Vector2Int MinIndex, Vector2Int MaxIndex, bool Vaule)
    {
        List<int> bitVaule = new();
        for (int i = 0; i <= (MaxIndex.y / 31); i++)
        {
            bitVaule.Add(0);
        }

        for (int i = MinIndex.y; i <= MaxIndex.y; i++)
        {
            bitVaule[i / 31] += (1 << (i % 31));
        }

        Debug.Log(MinIndex.x + " / " + MaxIndex.x);

        for (int i = MinIndex.x; i <= MaxIndex.x; i++)
        {
            for (int y = 0; y < bitVaule.Count; y++)
            {
                if (Vaule)
                {
                    list[i][y] |= bitVaule[y];
                }
                else
                {
                    list[i][y] &= ~bitVaule[y];
                }
            }
        }
    }
    public static void BC_Reverse(ref List<List<int>> list)
    {
        for (int x = 0; x < list.Count; x++)
        {
            for (int y = 0; y < list[x].Count; y++)
            {
                list[x][y] = ~list[x][y];
            }
        }
    }
}
