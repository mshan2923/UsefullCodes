using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ListScroll : MonoBehaviour
{
    public GameObject Content;
    ScrollRect scroll;
    public bool Vertical = true;

    [Space(10)]
    public Map<GameObject, float> ScrollList = new Map<GameObject, float>();
    public float ListPadding = 0.5f;
    public bool SlotStretch = true;
    public float SlotPadding = 0.5f;

    [Space(10)]
    public bool StartSpace = false;//처음에 여백을 줌
    public float BetweenSpace = 0;
    public bool AutoScrollToChange = true;

    [SerializeField]
    float ScrollEndVaule = 0;

    void Start()
    {
        scroll = gameObject.GetComponent<ScrollRect>();

        //====content 높이 작게 , 정렬
        var contentRect = Content.GetComponent<RectTransform>();
        if (Vertical)
        {
            contentRect.anchorMin = new Vector2(0, ListPadding);
            contentRect.anchorMax = new Vector2(1, ListPadding);
            contentRect.pivot = new Vector2(0.5f, ListPadding);// ListPadding : 0.5 =>Middle Stretch
        }else
        {
            contentRect.anchorMin = new Vector2(ListPadding, 0);
            contentRect.anchorMax = new Vector2(ListPadding, 1);
            contentRect.pivot = new Vector2(ListPadding, 0.5f);// ListPadding : 0.5 =>Center Stretch
        }
    }

    public void SetPadding( RectTransform rect, float horizontal, float vertical)
    {
        rect.offsetMax = new Vector2(-horizontal, -vertical);
        rect.offsetMin = new Vector2(horizontal, vertical);
    }//Not Anchored

    public void SetPadding( RectTransform rect, float left, float top, float right, float bottom)//Setting Rect Position & Size
    {
        rect.offsetMax = new Vector2(-right, -top);
        rect.offsetMin = new Vector2(left, bottom);
    }//Template --> SetPadding(Temp, Temp.offsetMin.x, -Temp.offsetMax.y, Temp.offsetMax.x, -Temp.offsetMin.y);

    public void Set(GameObject obj)
    {
        var Temp = obj.GetComponent<RectTransform>();

        if (Temp != null)
        {
            Temp.SetParent(Content.transform);


            if (Vertical)
            {
                float scrollWidth = 0;
                float slotHeight = 0;
                {

                    if (SlotStretch)
                    {
                        scrollWidth = 0;
                        Temp.anchorMin = new Vector2(0, 1);
                        Temp.anchorMax = Vector2.one;
                        Temp.pivot = new Vector2(0.5f, 1);//Top Stretch
                    }
                    else
                    {
                        scrollWidth = Temp.rect.width;
                        Temp.anchorMin = new Vector2(SlotPadding, 1);
                        Temp.anchorMax = new Vector2(0.5f, 1);
                        Temp.pivot = new Vector2(0.5f, 1);// SlotPadding : 0.5 =>Top Center
                    }

                    slotHeight = Temp.rect.height;
                }//Set scrollWidth , slotHeight

                //Temp.localPosition = new Vector3(Temp.rect.x, -ScrollEndVaule, Temp.localPosition.z);//로컬 기준에서 내려가니 y = -높이
                SetPadding(Temp, (-0.5f * scrollWidth), ScrollEndVaule, (-0.5f * scrollWidth), (-ScrollEndVaule - slotHeight));
            }
            else
            {
                float ScrollHeight = 0;
                float slotWidth = 0;
                {
                    if (SlotStretch)
                    {
                        ScrollHeight = 0;
                        Temp.anchorMin = Vector2.zero;
                        Temp.anchorMax = new Vector2(0, 1);
                        Temp.pivot = new Vector2(0, 0.5f);//Left Stretch
                    }
                    else
                    {
                        ScrollHeight = Temp.rect.height;
                        Temp.anchorMin = new Vector2(0, SlotPadding);
                        Temp.anchorMax = new Vector2(0, SlotPadding);
                        Temp.pivot = new Vector2(0, 0.5f);// SlotPadding : 0.5 =>Middle Left
                    }

                    slotWidth = Temp.rect.width;
                }//Set ScrollHeight, slotWidth

                //Temp.localPosition = new Vector3(ScrollEndVaule, Temp.rect.y, Temp.localPosition.z);
                SetPadding(Temp, ScrollEndVaule, (-0.5f * ScrollHeight), (-ScrollEndVaule - slotWidth), (-0.5f * ScrollHeight));
            }
        }
    }
    public void Add(GameObject obj)
    {
        var objRect = obj.GetComponent<RectTransform>();
        var contentRect = Content.GetComponent<RectTransform>();

        if (ScrollList.Count == 0 && StartSpace)
        {
            ScrollEndVaule += BetweenSpace;
        }

        Set(obj);

        if (Vertical)
        {
            ScrollList.Add(obj, objRect.rect.height);
            ScrollEndVaule += (objRect.rect.height + BetweenSpace);

            SetPadding(contentRect, contentRect.offsetMin.x, -contentRect.offsetMax.y, contentRect.offsetMax.x, (-ScrollEndVaule + contentRect.offsetMax.y));

            if (AutoScrollToChange)
            {
                scroll.verticalNormalizedPosition = 0;
                //Content.transform.position += new Vector3(0, (Temp.rect.height + BetweenSpace));
            }
        }else
        {
            ScrollList.Add(obj, objRect.rect.width);
            ScrollEndVaule += (objRect.rect.width + BetweenSpace);

            SetPadding(contentRect, contentRect.offsetMin.x, -contentRect.offsetMax.y, (-contentRect.offsetMin.x - ScrollEndVaule), -contentRect.offsetMin.y);

            if (AutoScrollToChange )//scroll.verticalScrollbar.IsActive() &&
            {
                scroll.horizontalNormalizedPosition = 1;      
            }
        }
    }
    public void AddEmptySpace(float SpaceSize)
    {
        ScrollList.Add(null, SpaceSize);

        ScrollEndVaule += SpaceSize;
    }

    public void Remove(int index, bool destroy = true)
    {
        if (index < 0 || index >= ScrollList.Count)
        {
            return;
        }

        var Lobj = ScrollList.GetKey(index);


        ScrollList.Remove(index);

        if (Lobj != null)
        {
            if (destroy)
            {
                GameObject.Destroy(Lobj);
            }

            var contentRect = Content.GetComponent<RectTransform>();

            if (Vertical)
            {
                float Lheight = Lobj.GetComponent<RectTransform>().rect.height;
                ScrollEndVaule -= (Lheight + BetweenSpace);

                SetPadding(contentRect, contentRect.offsetMin.x, -contentRect.offsetMax.y, contentRect.offsetMax.x, (-ScrollEndVaule + contentRect.offsetMax.y));

                if (AutoScrollToChange)
                {
                    scroll.verticalNormalizedPosition = 0;
                    //Content.transform.position -= new Vector3(0, (Lheight + BetweenSpace));
                }
            }
            else
            {
                float Lweight = Lobj.GetComponent<RectTransform>().rect.width;
                ScrollEndVaule -= (Lweight + BetweenSpace);

                SetPadding(contentRect, contentRect.offsetMin.x, -contentRect.offsetMax.y, (-contentRect.offsetMin.x - ScrollEndVaule), -contentRect.offsetMin.y);

                if (AutoScrollToChange)
                {
                    scroll.horizontalNormalizedPosition = 1;
                }

            }
        }
    }
    public bool Remove(GameObject obj, bool destroy = true)
    {
        int Lindex = ScrollList.GetKey().FindIndex(t => t == obj);

        if (Lindex >= 0)
        {
            Remove(Lindex, destroy);
            return true;
        }else
        {
            return false;
        }
    }

    public void RePosition()
    {
        var contentRect = Content.GetComponent<RectTransform>();
        RectTransform objRect;

        if (Vertical)
        {
            contentRect.anchorMin = new Vector2(0, ListPadding);
            contentRect.anchorMax = new Vector2(1, ListPadding);
            contentRect.pivot = new Vector2(0.5f, ListPadding);// ListPadding : 0.5 =>Middle Stretch
        }
        else
        {
            contentRect.anchorMin = new Vector2(ListPadding, 0);
            contentRect.anchorMax = new Vector2(ListPadding, 1);
            contentRect.pivot = new Vector2(ListPadding, 0.5f);// ListPadding : 0.5 =>Center Stretch
        }
        ScrollEndVaule = 0;

        for (int i = 0; i < ScrollList.Count; i++)
        {
            if (ScrollList.Count == 0 && StartSpace)
            {
                ScrollEndVaule += BetweenSpace;
            }

            if (ScrollList.GetKey(i) == null)
            {
                ScrollEndVaule += ScrollList.GetVaule(i);
            }
            else
            {
                Set(ScrollList.GetKey(i));

                objRect = ScrollList.GetKey(i).GetComponent<RectTransform>();
                if (Vertical)
                {
                    ScrollList.SetVaule(i, objRect.rect.height);
                    ScrollEndVaule += (objRect.rect.height + BetweenSpace);

                    SetPadding(contentRect, contentRect.offsetMin.x, -contentRect.offsetMax.y, contentRect.offsetMax.x, (-ScrollEndVaule + contentRect.offsetMax.y));

                    if (AutoScrollToChange)
                    {
                        scroll.verticalNormalizedPosition = 0;
                        //Content.transform.position += new Vector3(0, (Temp.rect.height + BetweenSpace));
                    }
                }
                else
                {
                    ScrollList.SetVaule(i, objRect.rect.width);
                    ScrollEndVaule += (objRect.rect.width + BetweenSpace);

                    SetPadding(contentRect, contentRect.offsetMin.x, -contentRect.offsetMax.y, (-contentRect.offsetMin.x - ScrollEndVaule), -contentRect.offsetMin.y);

                    if (AutoScrollToChange)//scroll.verticalScrollbar.IsActive() &&
                    {
                        scroll.horizontalNormalizedPosition = 1;
                    }
                }
            }
        }
    }
}

[UnityEditor.CustomEditor(typeof(ListScroll))]
public class ListScrollEditor  : UnityEditor.Editor
{
    ListScroll owner;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        owner = target as ListScroll;

        if (GUILayout.Button("Add EmptySpace"))
        {
            owner.AddEmptySpace(10);
        }

        if (GUILayout.Button("Redraw"))
        {
            owner.RePosition();
        }
    }
}
