using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareCalculate
{
    public static bool InSquare(Vector2 Pos, Vector2 SquarePos, Vector2 Pivot, Vector2 Size, float Error = 0)
    {
        Vector2 Offset = Pivot - new Vector2(0.5f, 0.5f);
        Vector2 mPos = SquarePos - (Offset * Size / 2f);

        if(InRange(Pos.x , mPos.x - (Size.x  / 2f) - Error, mPos.x + (Size.x / 2f) + Error))
        {
            if(InRange(Pos.y, mPos.y - (Size.y / 2f) - Error, mPos.y + (Size.y / 2f) + Error))
            {
                return true;
            }
        }
        return false;
    }
    public static bool InSquare(Vector2 Pos, RectTransform Rect, float Error = 0)
    {
        return InSquare(Pos, Rect.position, Rect.pivot, Rect.sizeDelta, Error);
    }

    public static Vector2 SquareLocal(Vector2 Pos, Vector2 SquarePos, Vector2 Pivot, Vector2 Size, Vector2 TargetPivot)
    {
        Vector2 Offset = Pivot - new Vector2(0.5f, 0.5f);
        Vector2 TOffset = TargetPivot - new Vector2(0.5f, 0.5f);
        Vector2 mPos = SquarePos - (Offset * Size / 2f);
        //var LocalPos = (Input.mousePosition - LBright.position) + (new Vector3(LBright.sizeDelta.x, LBright.sizeDelta.y, 0) * 0.5f);
        return Pos - mPos + (Size * TOffset);
    }
    public static Vector2 SquareLocal(Vector2 Pos, RectTransform Rect, Vector2 TargetPivot)
    {
        return SquareLocal(Pos, Rect.position, Rect.pivot, Rect.sizeDelta, TargetPivot);
    }

    public static Vector2 SquareRate(Vector2 Pos, Vector2 SquarePos, Vector2 Pivot, Vector2 Size, Vector2 TargetPivot , bool Lock = true)
    {
        Vector2 LLocal = SquareLocal(Pos, SquarePos, Pivot, Size, TargetPivot);
        if(Lock)
        {
            Vector2 LRate = LLocal / Size;
            LRate.x = Mathf.Clamp(LRate.x, 0, 1);
            LRate.y = Mathf.Clamp(LRate.y, 0, 1);
            return LRate;
        }
        else
        {
            return LLocal / Size;
        }
    }
    public static Vector2 SquareRate(Vector2 Pos, RectTransform Rect, Vector2 TargetPivot, bool Lock = true)
    {
        return SquareRate(Pos, Rect.position, Rect.pivot, Rect.sizeDelta, TargetPivot, Lock);
    }

    public static Vector2 SquareBorder(Vector2[] Pos, Vector2 SquarePos, Vector2 Pivot, Vector2 Size)//Return Close Pos[0] + In Square
    {
        var Points = SquareBorders(Pos, SquarePos, Pivot, Size);

        if(Pos.Length >= 2)
        {
            if (Points.Count > 0)
            {
                {/*
                    Vector2 FarPos;
                    if (Vector2.SqrMagnitude(SquarePos - Pos[0]) > Vector2.SqrMagnitude(SquarePos - Pos[1]))
                    {
                        FarPos = Pos[0];
                    }
                    else
                    {
                        FarPos = Pos[1];
                    }*/
                }//Set FarPos

                float Dis = Vector2.Distance(Pos[0], Points[0]);//Test => FarPos >> Pos[0]
                Vector2 result = Points[0];

                for (int i = 1; i < Points.Count; i++)
                {
                    if (Dis > Vector2.Distance(Pos[0], Points[i]))
                    {
                        Dis = Vector2.Distance(Pos[0], Points[i]);
                        result = Points[i];
                    }
                }

                return result;
            }
            else
            {
                Vector2 FarPos;
                {
                    if (Vector2.SqrMagnitude(SquarePos - Pos[0]) > Vector2.SqrMagnitude(SquarePos - Pos[1]))
                    {
                        FarPos = Pos[0];
                    }
                    else
                    {
                        FarPos = Pos[1];
                    }
                }//Set FarPos

                return SquareBorder(new Vector2[] { SquarePos, FarPos }, SquarePos, Pivot, Size);
            }
        }
        else
        {
            return SquarePos;
        }//Pos.Length < 2
    }
    public static List<Vector2> SquareBorders(Vector2[] Pos, Vector2 SquarePos, Vector2 Pivot, Vector2 Size, bool InSqare = true)
    {
        float Slope = 0;
        float beta = 0;
        Vector2 Offset = Pivot - new Vector2(0.5f, 0.5f);
        Vector2 mPos = SquarePos - Offset * Size;
        List<Vector2> ListPos = new List<Vector2>();
        List<Vector2> Ltemp = new List<Vector2>();

        if (Pos.Length < 2)
            return Ltemp;

        if ((Pos[1].x - Pos[0].x) == 0)
        {
            return new List<Vector2> { new Vector2(Pos[0].x, mPos.y - (Size.y / 2f)), new Vector2(Pos[1].x, mPos.y + (Size.y / 2f))};
        }//infinity
        else if ((Pos[1].y - Pos[0].y) == 0)
        {
            return new List<Vector2> { new Vector2(mPos.x - (Size.x / 2f), Pos[0].y), new Vector2(mPos.x + (Size.x / 2f), Pos[1].y)};
        }//0
        else
        {
            Slope = (Pos[1].y - Pos[0].y) / (Pos[1].x - Pos[0].x);
            beta = (-1 * Slope * Pos[0].x) + Pos[0].y;

            //사분면 x OR y 좌표 
            Vector2 DL = mPos - (Size / 2f);
            Vector2 UR = mPos + (Size / 2f);

            {
                ListPos.Add(new Vector2(DL.x, Slope * DL.x + beta));
                ListPos.Add(new Vector2((DL.y - beta) / Slope, DL.y));
                ListPos.Add(new Vector2(UR.x, Slope * UR.x + beta));
                ListPos.Add(new Vector2((UR.y - beta) / Slope, UR.y));
            }//절편 (intercept)

            if(InSqare)
            {
                for (int i = 0; i < ListPos.Count; i++)
                {
                    if (InSquare(ListPos[i], mPos, new Vector2(0.5f,0.5f), Size, 0))//Already Correction Position
                    {
                        Ltemp.Add(ListPos[i]);
                    }
                }
                return Ltemp;
            }else
            {
                return ListPos;
            }
        }
        //return Ltemp;
    }
    static bool InRange(float vaule, float start, float End)
    {
        if(vaule >= start)
        {
            if(vaule <= End)
            {
                return true;
            }
        }
        return false;
    }
}
