using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ListScroll : MonoBehaviour
{
    public struct FoldSlotData
    {
        public float TitileSize;
        public float ContentSize;
        public bool IsOpen;
    }


    public GameObject Content;
    ScrollRect scroll;
    public bool Vertical = true;

    [Space(10)]
    public Map<GameObject, float> ScrollList = new Map<GameObject, float>();
    Map<int, FoldSlotData> FoldData = new Map<int, FoldSlotData>();
    public FoldPanel FoldSlotObject;

    [Space(5)]
    public float SlotSize = 50f;// disable to use SlotStretch
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

        var contentRect = Content.GetComponent<RectTransform>();
        if (Vertical)
        {
            SetPadding(contentRect, contentRect.offsetMin.x, 0, contentRect.offsetMax.x, 0);

            contentRect.anchorMin = new Vector2(0, ListPadding);
            contentRect.anchorMax = new Vector2(1, ListPadding);
            contentRect.pivot = new Vector2(0.5f, ListPadding);// ListPadding : 0.5 =>Middle Stretch
        }else
        {
            SetPadding(contentRect, 0, -contentRect.offsetMax.y, 0, -contentRect.offsetMin.y);


            contentRect.anchorMin = new Vector2(ListPadding, 0);
            contentRect.anchorMax = new Vector2(ListPadding, 1);
            contentRect.pivot = new Vector2(ListPadding, 0.5f);// ListPadding : 0.5 =>Center Stretch
        }//content 높이 작게 , 정렬
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
    }
    //Template --> SetPadding(Temp, Temp.offsetMin.x, -Temp.offsetMax.y, -Temp.offsetMax.x, -Temp.offsetMin.y);

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
                        scrollWidth = SlotSize;
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
                        ScrollHeight = SlotSize;
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
            ScrollEndVaule = BetweenSpace;
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
        if (ScrollList.Count > 0)
        {
            if (ScrollList.GetKey(ScrollList.Count - 1) == null)
            {
                ScrollList.SetVaule(ScrollList.Count - 1, ScrollList.GetVaule(ScrollList.Count - 1) + SpaceSize);
            }
            else
            {
                ScrollList.Add(null, SpaceSize);
            }
        }else
        {
            ScrollList.Add(null, SpaceSize);
        }

        ScrollEndVaule += SpaceSize;
    }
    public void AddFold(float titileSize, float contentSize, bool isOpen = false)
    {
        var obj = GameObject.Instantiate(FoldSlotObject);
        obj.TitleSize = titileSize;
        obj.ContentSize = contentSize;
        obj.SetOpen(isOpen);


        var objRect = obj.gameObject.GetComponent<RectTransform>();
        objRect.sizeDelta = new Vector2(objRect.sizeDelta.x, (isOpen ? (titileSize + contentSize) : titileSize));


        var slotData = new FoldSlotData
        {
            TitileSize = titileSize,
            ContentSize = (titileSize + contentSize),
            IsOpen = isOpen
        };
        FoldData.Add(ScrollList.Count, slotData);
        Add(obj.gameObject);
        obj.OpenEvnet += new FoldPanel.OpenDelegate(OpenEvent);
    }
    //AddFoldSlot , RemoveFoldSlot

    public void Remove(int index, bool destroy = true)
    {
        if (index < 0 || index >= ScrollList.Count)
        {
            return;
        }

        var Lobj = ScrollList.GetKey(index);

        if (Lobj != null)
        {

            RemoveAnimationEvent(Lobj, destroy);

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

            {
                if (FoldData.GetKey().Exists(t => t == index))
                {
                    {
                        for (int i = 0; i < ScrollList.GetKey(index).transform.childCount; i++)
                        {
                            //if (destroy)
                            //GameObject.Destroy(ScrollList.GetKey(index).transform.GetChild(i).gameObject);
                            RemoveAnimationEvent(ScrollList.GetKey(index).transform.GetChild(i).gameObject, destroy);
                        }
                    }//자식 제거
                    
                    FoldData.Remove(index);
                }//Remove

                for (int i = 0; i < FoldData.Count; i++)
                {
                    if (FoldData.GetKey(i) > index)
                    {
                        FoldData.SetKey(i, FoldData.GetKey(i) - 1);
                    }
                }
            }//Change ExpandSlots SlotIndex
        }else
        {
            ScrollEndVaule -= ScrollList.GetVaule(index);
        }

        ScrollList.Remove(index);
        RePosition();
    }//ExpandSlots SlotIndex 변경 (제거대상, 인덱스 변경 유무) , 자식도 같이 제거
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
    public virtual void RemoveAnimationEvent(GameObject Obj, bool destory)
    {
        if (destory)
        {
            GameObject.Destroy(Obj);
        }
    }//기본값 - GameObject.Destory

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

                    //SetPadding(contentRect, contentRect.offsetMin.x, -contentRect.offsetMax.y, contentRect.offsetMax.x, (-ScrollEndVaule + contentRect.offsetMax.y));
                    contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, ScrollEndVaule);

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

                    //SetPadding(contentRect, contentRect.offsetMin.x, -contentRect.offsetMax.y, (-contentRect.offsetMin.x - ScrollEndVaule), -contentRect.offsetMin.y);
                    contentRect.sizeDelta = new Vector2(ScrollEndVaule, contentRect.sizeDelta.y);

                    if (AutoScrollToChange)//scroll.verticalScrollbar.IsActive() &&
                    {
                        scroll.horizontalNormalizedPosition = 1;
                    }
                }
            }
        }
    }//ExpandSlot 인지 검사후 크기조절후 자식오브젝트 (비)활성화 + 슬롯 크기 X?

    public void OpenEvent(GameObject obj, bool Open)
    {
        int Lindex = ScrollList.GetKey().FindIndex(t => t == obj);

        if (Lindex >= 0)
        {
            if (FoldData.GetKey().Exists(t => t == Lindex))
            {
                var temp = FoldData.GetVaule(FoldData.GetKey().FindIndex(v => v == Lindex));
                temp.IsOpen = Open;
                FoldData.SetVaule(Lindex, temp);
            }

            //ScrollList.GetKey(Lindex).transform.GetChild

            RePosition();
        }
    }
    public void SetOpen(GameObject obj, bool Open)
    {
        int Lindex = ScrollList.GetKey().FindIndex(t => t == obj);

        if (Lindex >= 0)
        {
            var foldObj = obj.GetComponent<FoldPanel>();
            if (FoldData.GetKey().Exists(t => t == Lindex) && foldObj != null)
            {
                var temp = FoldData.GetVaule(FoldData.GetKey().FindIndex(v => v == Lindex));
                temp.IsOpen = Open;
                FoldData.SetVaule(Lindex, temp);

                foldObj.SetOpen(Open);
            }

            //ScrollList.GetKey(Lindex).transform.GetChild

            RePosition();
        }
    }

}

#if UNITY_EDITOR
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
        if (GUILayout.Button("Add FoldSlot"))
        {
            owner.AddFold(20, 100, false);
        }

        if (GUILayout.Button("Redraw"))
        {
            owner.RePosition();
        }

        if (GUILayout.Button("Random Remove"))
        {
            owner.Remove(Random.Range(0 , owner.ScrollList.Count - 1));
        }
    }
}
#endif
