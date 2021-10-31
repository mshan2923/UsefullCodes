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

        //앵커 -> 앵커의 시작점,  상대적거리으로
        //일반 -> 부모 Rect기준
    }
    //Template --> SetPadding(Temp, Temp.offsetMin.x, -Temp.offsetMax.y, -Temp.offsetMax.x, -Temp.offsetMin.y);

    public static void SetTransform(RectTransform rect , Vector2 LocalPosition, Vector2 RectSize, Vector2 DrawPivot)
    {
        if (IsXStretch(rect))
        {
            SetPadding(rect, 0, -(LocalPosition.y + RectSize.y * (DrawPivot.y)),
            0, (LocalPosition.y - RectSize.y * (1 - DrawPivot.y)));
        }
        else if (IsYStretch(rect))
        {
            SetPadding(rect, (LocalPosition.x - RectSize.x * (DrawPivot.x)), 0,
            -(LocalPosition.x + RectSize.x * (1 - DrawPivot.x)), 0);
        }
        else
        {
            SetPadding(rect, (LocalPosition.x - RectSize.x * (DrawPivot.x)), -(LocalPosition.y + RectSize.y * (DrawPivot.y)),
            -(LocalPosition.x + RectSize.x * (1 - DrawPivot.x)), (LocalPosition.y - RectSize.y * (1 - DrawPivot.y)));
            //DrawRate == Vector2.Zero ->> LeftTop
        }

    }

    public static void SetWorldTransform(RectTransform rect, Vector2 Position, Vector2 RectSize, Vector2 DrawPivot)
    {
        Vector2 LPos = Position + ((Vector2.one + (rect.pivot * -2)) * RectSize) - new Vector2(rect.parent.position.x, rect.parent.position.y);
        SetTransform(rect, LPos, RectSize, DrawPivot);
        //Anchored 왼-오 OR 상-하 차이가 1인경우 Stretch
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
            }//Fillter의 해당 컴포넌트를 가진것만 + Target이 아닌 첫번째 오브젝트를 리턴
        }

        return null;
    }
}
