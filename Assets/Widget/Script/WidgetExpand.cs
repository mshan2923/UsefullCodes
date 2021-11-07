using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WidgetExpand
{
    public static void SetPadding(RectTransform rect, float left, float top, float right, float bottom)//Setting Rect Position & Size
    {
        rect.offsetMax = new Vector2(-right, -top);
        rect.offsetMin = new Vector2(left, bottom);

        //��Ŀ -> ��Ŀ�� ������,  ������Ÿ�����
        //�Ϲ� -> �θ� Rect����
    }
    //Template --> SetPadding(Temp, Temp.offsetMin.x, -Temp.offsetMax.y, -Temp.offsetMax.x, -Temp.offsetMin.y);

    public static void SetTransform(RectTransform rect , Vector2 LocalPosition, Vector2 RectSize, Vector2 DrawPivot)
    {
        Vector2 ParnetSize = GetWorldSize((RectTransform) rect.parent);
        
        if (IsXStretch(rect) && IsYStretch(rect))
        {
            Vector2 LT = ((ParnetSize - RectSize) * DrawPivot) + LocalPosition;
            Vector2 RB = ((ParnetSize - RectSize) * (Vector2.one - DrawPivot)) - LocalPosition;

            SetPadding(rect, LT.x, LT.y, RB.x, RB.y);
        }
        else if (IsXStretch(rect))
        {
            SetPadding(rect, (LocalPosition + (ParnetSize - RectSize) * DrawPivot).x, -(LocalPosition.y + RectSize.y * (DrawPivot.y)),
            (LocalPosition + (ParnetSize - RectSize) * (Vector2.one - DrawPivot)).x, (LocalPosition.y - RectSize.y * (1 - DrawPivot.y)));
        }
        else if (IsYStretch(rect))
        {
            SetPadding(rect, (LocalPosition.x - RectSize.x * (DrawPivot.x)), (((ParnetSize - RectSize) * DrawPivot) + LocalPosition).y,
            -(LocalPosition.x + RectSize.x * (1 - DrawPivot.x)), (((ParnetSize - RectSize) * (Vector2.one - DrawPivot)) - LocalPosition).y);
        }
        else
        {
            SetPadding(rect, (LocalPosition.x - RectSize.x * (DrawPivot.x)), -(LocalPosition.y + RectSize.y * (DrawPivot.y)),
            -(LocalPosition.x + RectSize.x * (1 - DrawPivot.x)), (LocalPosition.y - RectSize.y * (1 - DrawPivot.y)));

            //DrawRate == Vector2.Zero ->> LeftTop
        }

        //Debug.Log("Parent : " + rect.parent + " - " + ParnetSize + " \n Rect : " + rect.gameObject + " - " + RectSize);

    }//DrawRate == Vector2.Zero ->> LeftTop (Origin : Parent Pivot)
    public static void SetTransform(RectTransform rect, Vector2 LocalPosition, Vector2 ParentSize, Vector2 RectSize, Vector2 DrawPivot)
    {
        if (IsXStretch(rect) && IsYStretch(rect))
        {
            Vector2 LT = ((ParentSize - RectSize) * DrawPivot) + LocalPosition;
            Vector2 RB = ((ParentSize - RectSize) * (Vector2.one - DrawPivot)) - LocalPosition;

            SetPadding(rect, LT.x, LT.y, RB.x, RB.y);
        }
        else if (IsXStretch(rect))
        {
            SetPadding(rect, (LocalPosition + (ParentSize - RectSize) * DrawPivot).x, -(LocalPosition.y + RectSize.y * (DrawPivot.y)),
            (LocalPosition + (ParentSize - RectSize) * (Vector2.one - DrawPivot)).x, (LocalPosition.y - RectSize.y * (1 - DrawPivot.y)));
        }
        else if (IsYStretch(rect))
        {
            SetPadding(rect, (LocalPosition.x - RectSize.x * (DrawPivot.x)), (((ParentSize - RectSize) * DrawPivot) + LocalPosition).y,
            -(LocalPosition.x + RectSize.x * (1 - DrawPivot.x)), (((ParentSize - RectSize) * (Vector2.one - DrawPivot)) - LocalPosition).y);
        }
        else
        {
            SetPadding(rect, (LocalPosition.x - RectSize.x * (DrawPivot.x)), -(LocalPosition.y + RectSize.y * (DrawPivot.y)),
            -(LocalPosition.x + RectSize.x * (1 - DrawPivot.x)), (LocalPosition.y - RectSize.y * (1 - DrawPivot.y)));

            //DrawRate == Vector2.Zero ->> LeftTop
        }

        //Debug.Log("Parent : " + rect.parent + " - " + ParnetSize + " \n Rect : " + rect.gameObject + " - " + RectSize);
    }

    public static void SetWorldTransform(RectTransform rect, Vector2 Position, Vector2 RectSize, Vector2 DrawPivot)
    {
        Vector2 LPos = Position + ((Vector2.one + (rect.pivot * -2)) * RectSize) - new Vector2(rect.parent.position.x, rect.parent.position.y);
        SetTransform(rect, LPos, RectSize, DrawPivot);
        //Anchored ��-�� OR ��-�� ���̰� 1�ΰ�� Stretch

    }//DrawPivot == Vector.zero == LeftTop (Origin : Screen Left Buttom)
    public static void SetPosition(RectTransform rect, Vector2 LocalPosition, Vector2 DrawPivot)
    {
        SetTransform(rect, LocalPosition, GetWorldSize(rect), DrawPivot);
    }

    public static Vector2 GetWorldSize(RectTransform rect)
    {
        Vector3[] rectCorners = new Vector3[4];
        rect.GetWorldCorners(rectCorners);
        //0 : ���� �Ʒ� / 1: ���� �� / 2: ������ �� / 3: ������ �Ʒ�

        return new Vector2(Mathf.Abs((rectCorners[2] - rectCorners[0]).x), Mathf.Abs((rectCorners[2] - rectCorners[0]).y));
    }//Notworking Stretch
    [System.Obsolete("--")]
    public static void SetWorldSize(RectTransform rect , Vector2 Size)
    {
        Vector2 RectSize = GetWorldSize(rect);

        if (IsXStretch(rect) && IsYStretch(rect))
        {
            rect.sizeDelta = Size - RectSize;
        }else if (IsXStretch(rect))
        {
            rect.sizeDelta = new Vector2((Size.x - RectSize.x), Size.y);
        }else if (IsYStretch(rect))
        {
            rect.sizeDelta = new Vector2(Size.x, (Size.y - RectSize.y));
        }else
        {
            rect.sizeDelta = Size;
        }
    }
    public static bool IsXStretch(RectTransform rect)
    {
        return Mathf.Approximately(Mathf.Abs(rect.anchorMin.x - rect.anchorMax.x), 1);
    }
    public static bool IsYStretch(RectTransform rect)
    {
        return Mathf.Approximately(Mathf.Abs(rect.anchorMin.y - rect.anchorMax.y), 1);
    }

    public static bool ContainWidget(GameObject Obj, Vector3 pos)
    {
        var Lrect = Obj.GetComponent<RectTransform>();
        Vector3 ObjPos = Obj.transform.position;

        if (Lrect != null)
        {
            if (Lrect.rect.xMin < (pos.x - ObjPos.x) && (pos.x - ObjPos.x) < Lrect.rect.xMax)
            {
                if (Lrect.rect.yMin < (pos.y - ObjPos.y) && (pos.y - ObjPos.y) < Lrect.rect.yMax)
                {
                    return true;
                }
            }
            return false;
        }else
        {
            return false;
        }
    }//Pos is WorldPosition ( Recommand => Input.mousePosition)

    public static List<RaycastResult> WidgetLineTrace(GraphicRaycaster gr)
    {
        var ped = new PointerEventData(null)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new();
        gr.Raycast(ped, results);

        return results;
    }
    public static GameObject GetBehideObject(GraphicRaycaster gr, System.Type Fillter)
    {
        List<RaycastResult> results = WidgetLineTrace(gr);
        for (int i = 0; i < results.Count; i++)
        {
            if (Fillter == null)
            {
                return results[0].gameObject;
            }

            if (results[i].gameObject.GetComponent(Fillter))// && results[i].gameObject != Target
            {
                return results[i].gameObject;
            }//Fillter�� �ش� ������Ʈ�� �����͸� + Target�� �ƴ� ù��° ������Ʈ�� ����
        }

        return null;
    }
}
