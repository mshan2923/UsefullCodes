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
    public Vector2 PenelSize;
    public bool Vertical = true;

    [Space(10)]
    public Map<GameObject, float> ScrollList = new Map<GameObject, float>();
    public FoldPanel FoldSlotObject;

    [Space(5), Header("Direction - L , R : (Axis, Perp) | T , B : (Perp , Axis)")]
    public float SlotAxisSize = 30f;
    public float SlotPerpendicularSize = 50f;// disable to use SlotStretch , if Vertical - Slot Horizontal Size
    [Space(10)]
    public float ListPadding = 0.5f;
    public bool SlotStretch = true;
    public float SlotPadding = 0.5f;

    [Space(10)]
    public bool StartSpace = false;//처음에 여백을 줌
    public float BetweenSpace = 0;
    public bool AutoScrollToChange = true;

    [SerializeField]
    float ScrollEndVaule = 0;

    [Space(10)]
    public float FoldOpenSize = 100;
    public FoldPanel.FoldDirection FoldDirection_Vertical = FoldPanel.FoldDirection.TopToButtom;
    public FoldPanel.FoldDirection FoldDirection_Horizontal = FoldPanel.FoldDirection.LeftToRight;

    void Start()
    {
        scroll = gameObject.GetComponent<ScrollRect>();

        var contentRect = Content.GetComponent<RectTransform>();
        PenelSize = WidgetExpand.GetWorldSize(gameObject.GetComponent<RectTransform>());

        if (Vertical)
        {
            WidgetExpand.SetTransform(contentRect, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

            contentRect.anchorMin = new Vector2(0, ListPadding);
            contentRect.anchorMax = new Vector2(1, ListPadding);
            contentRect.pivot = new Vector2(0.5f, ListPadding);// ListPadding : 0.5 =>Middle Stretch
        }else
        {
            WidgetExpand.SetTransform(contentRect, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));


            contentRect.anchorMin = new Vector2(ListPadding, 0);
            contentRect.anchorMax = new Vector2(ListPadding, 1);
            contentRect.pivot = new Vector2(ListPadding, 0.5f);// ListPadding : 0.5 =>Center Stretch
        }//content 높이 작게 , 정렬
    }

    public void SetPadding( RectTransform rect, float left, float top, float right, float bottom)//Setting Rect Position & Size
    {
        rect.offsetMax = new Vector2(-right, -top);
        rect.offsetMin = new Vector2(left, bottom);
    }
    //Template --> SetPadding(Temp, Temp.offsetMin.x, -Temp.offsetMax.y, -Temp.offsetMax.x, -Temp.offsetMin.y);

    public void Set(GameObject obj)
    {
        

        var objRect = obj.GetComponent<RectTransform>();
        FoldPanel LFold = obj.GetComponent<FoldPanel>();

        if (LFold != null)
        {
            LFold.TitleHeight = SlotAxisSize;//titleHeight;

            if (Vertical)
            {
                if (FoldDirection_Vertical == FoldPanel.FoldDirection.TopToButtom || FoldDirection_Vertical == FoldPanel.FoldDirection.ButtomToTop)
                    LFold.PanelSize = new Vector2((SlotStretch ? PenelSize.x : SlotPerpendicularSize), (LFold.IsOpen ? FoldOpenSize : LFold.TitleHeight));
                else
                    LFold.PanelSize = new Vector2(FoldOpenSize, SlotPerpendicularSize);

                LFold.Direction = FoldDirection_Vertical;
            }
            else
            {
                if (FoldDirection_Horizontal == FoldPanel.FoldDirection.LeftToRight || FoldDirection_Horizontal == FoldPanel.FoldDirection.RightToLeft)
                    LFold.PanelSize = new Vector2((LFold.IsOpen ? FoldOpenSize : LFold.TitleHeight), (SlotStretch ? PenelSize.y : SlotPerpendicularSize));
                else
                    LFold.PanelSize = new Vector2(SlotPerpendicularSize, FoldOpenSize);

                LFold.Direction = FoldDirection_Horizontal;
            }

        }

        if (objRect != null)
        {
            objRect.SetParent(Content.transform);
            float SlotWidth = 0;
            float SlotHeight = 0;

            if (LFold != null)
            {
                LFold.ReDraw();
            }

            if (Vertical)
            {
                {
                    objRect.anchorMin = new Vector2(0, 1);
                    objRect.anchorMax = Vector2.one;
                    objRect.pivot = new Vector2(0.5f, 1);//Top Stretch

                    if (SlotStretch)
                    {
                        SlotWidth = PenelSize.x;

                    }
                    else
                    {
                        SlotWidth = LFold == null ? SlotPerpendicularSize : LFold.PanelSize.x;
                        //objRect.anchorMin = new Vector2(SlotPadding, 1);
                        //objRect.anchorMax = new Vector2(0.5f, 1);
                        //objRect.pivot = new Vector2(0.5f, 1);// SlotPadding : 0.5 =>Top Center
                    }

                    SlotHeight = LFold == null ? SlotAxisSize : LFold.PanelSize.y;
                }//Set scrollWidth , slotHeight

                WidgetExpand.SetTransform(objRect, new Vector2(0, -ScrollEndVaule), new Vector2(SlotWidth, SlotHeight), new Vector2(SlotPadding, 0));
            }
            else
            {
                {
                    objRect.anchorMin = Vector2.zero;
                    objRect.anchorMax = new Vector2(0, 1);
                    objRect.pivot = new Vector2(0, 0.5f);//Left Stretch

                    if (SlotStretch)
                    {
                        SlotHeight = PenelSize.y;

                    }
                    else
                    {
                        SlotHeight = LFold == null ? SlotPerpendicularSize : LFold.PanelSize.y;
                        //objRect.anchorMin = new Vector2(0, SlotPadding);
                        //objRect.anchorMax = new Vector2(0, SlotPadding);
                        //objRect.pivot = new Vector2(0, 0.5f);// SlotPadding : 0.5 =>Middle Left
                    }

                    SlotWidth = LFold == null ? SlotAxisSize : LFold.PanelSize.x;//Temp.sizeDelta.x;
                }//Set ScrollHeight, slotWidth

                WidgetExpand.SetTransform(objRect, new Vector2(ScrollEndVaule, 0), new Vector2(SlotWidth, SlotHeight), new Vector2(0, SlotPadding));
            }

        }
    }
    public void Add(GameObject obj)
    {
        var objRect = obj.GetComponent<RectTransform>();
        var contentRect = Content.GetComponent<RectTransform>();
        Vector2 PastPos = contentRect.anchoredPosition;

        if (ScrollList.Count == 0)
        {
            ScrollEndVaule = StartSpace? BetweenSpace : 0;

            WidgetExpand.SetTransform(contentRect, Vector2.zero, PenelSize, new Vector2(0.5f, 0));
        }

        Set(obj);

        if (Vertical)
        {
            //objRect.sizeDelta = new Vector2(SlotPerpendicularSize, SlotAxisSize);

            ScrollList.Add(obj, objRect.rect.height);
            ScrollEndVaule += (objRect.rect.height + BetweenSpace);

            SetPadding(contentRect, contentRect.offsetMin.x, -contentRect.offsetMax.y, contentRect.offsetMax.x, (-ScrollEndVaule + contentRect.offsetMax.y));
            

            if (AutoScrollToChange)
            {
                scroll.verticalNormalizedPosition = 0;
                //Content.transform.position += new Vector3(0, (Temp.rect.height + BetweenSpace));
            }else
            {
                contentRect.anchoredPosition = PastPos;
            }
        }else
        {
            //objRect.sizeDelta = new Vector2(SlotAxisSize, SlotPerpendicularSize);

            ScrollList.Add(obj, objRect.rect.width);
            ScrollEndVaule += (objRect.rect.width + BetweenSpace);

            SetPadding(contentRect, contentRect.offsetMin.x, -contentRect.offsetMax.y, (-contentRect.offsetMin.x - ScrollEndVaule), -contentRect.offsetMin.y);
            

            if (AutoScrollToChange )//scroll.verticalScrollbar.IsActive() &&
            {
                scroll.horizontalNormalizedPosition = 1;      
            }
            else
            {
                contentRect.anchoredPosition = PastPos;
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
    public void AddFold( bool isOpen = false)
    {
        var obj = GameObject.Instantiate(FoldSlotObject);
        obj.TitleHeight = SlotAxisSize;//titleHeight;

        Add(obj.gameObject);

        //PanelSize를 방향 기준으로 설정

        /*
        if (Vertical)
        {
            if (FoldDirection_Vertical == FoldPanel.FoldDirection.TopToButtom || FoldDirection_Vertical == FoldPanel.FoldDirection.ButtomToTop)
                obj.PanelSize = new Vector2((SlotStretch ? ScrollSize.x : SlotPerpendicularSize), FoldOpenSize);
            else
                obj.PanelSize = new Vector2(FoldOpenSize, SlotPerpendicularSize);

            obj.Direction = FoldDirection_Vertical ;
        }
        else
        {
            if (FoldDirection_Vertical == FoldPanel.FoldDirection.LeftToRight && FoldDirection_Vertical == FoldPanel.FoldDirection.RightToLeft)
                obj.PanelSize = new Vector2(FoldOpenSize, (SlotStretch ? ScrollSize.x : SlotPerpendicularSize));
            else
                obj.PanelSize = new Vector2(SlotAxisSize, FoldOpenSize);

            obj.Direction = FoldDirection_Horizontal;
        }   */
        

        obj.SetOpen(isOpen, false);
        obj.OpenEvnet += new FoldPanel.OpenDelegate(OpenEvent);

        RePosition();
    }//=======================if else 부분 Set으로 이동

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
                if (ScrollList.GetKey(index).GetComponent<FoldPanel>() != null)
                {
                    {
                        for (int i = 0; i < ScrollList.GetKey(index).transform.childCount; i++)
                        {
                            RemoveAnimationEvent(ScrollList.GetKey(index).transform.GetChild(i).gameObject, destroy);
                        }
                    }//자식 제거
                    
                }//Remove
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

        ScrollEndVaule = StartSpace? BetweenSpace : 0;


        for (int i = 0; i < ScrollList.Count; i++)
        {
            if (ScrollList.GetKey(i) == null)
            {
                ScrollEndVaule += ScrollList.GetVaule(i);
            }
            else
            {
                if (ScrollList.GetKey(i).GetComponent<FoldPanel>() != null)
                {
                    ScrollList.GetKey(i).GetComponent<FoldPanel>().Direction = Vertical ? FoldDirection_Vertical : FoldDirection_Horizontal;
                    ScrollList.GetKey(i).GetComponent<FoldPanel>().ReDraw();
                }

                Set(ScrollList.GetKey(i));

                objRect = ScrollList.GetKey(i).GetComponent<RectTransform>();

                if (Vertical)
                {
                    //WidgetExpand.SetPosition(objRect, Vector2.down * ScrollEndVaule, new Vector2(0.5f, 0));

                    ScrollList.SetVaule(i, objRect.rect.height);
                    ScrollEndVaule += (objRect.rect.height + BetweenSpace);

                    contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, ScrollEndVaule);

                    if (AutoScrollToChange)
                    {
                        scroll.verticalNormalizedPosition = 0;                        
                    }
                }
                else
                {
                    //WidgetExpand.SetPosition(objRect, Vector2.right * ScrollEndVaule, new Vector2(0, 0.5f));

                    ScrollList.SetVaule(i, objRect.rect.width);
                    ScrollEndVaule += (objRect.rect.width + BetweenSpace);

                    contentRect.sizeDelta = new Vector2(ScrollEndVaule, contentRect.sizeDelta.y);

                    if (AutoScrollToChange)
                    {
                        scroll.horizontalNormalizedPosition = 1;
                    }
                }
            }
        }
    }//ExpandSlot 인지 검사후 크기조절후 자식오브젝트 (비)활성화 + 슬롯 크기 X?

    public void OpenEvent(GameObject obj, bool Open)
    {
        if (ScrollList.GetKey().Exists(t => t == obj))
        {
            RePosition();
            return;
        }

        {
            /*
        int Lindex = ScrollList.GetKey().FindIndex(t => t == obj);

        if (Lindex >= 0)
        {
            RePosition();

            //ScrollList.GetKey(Lindex).transform.GetChild
            var Lfold = obj.GetComponent<FoldPanel>();
            var contentRect = Content.GetComponent<RectTransform>();

            Vector3 scrollPos = contentRect.anchoredPosition;


            if (! AutoScrollToChange)
            {
                var ContentRect = Content.GetComponent<RectTransform>();// 이거말고 좌상단 기준으로 
                Vector2 LeftTop = new Vector2(ContentRect.offsetMin.x, ContentRect.offsetMax.y);

                if (Vertical)
                {
                    //contentRect.anchoredPosition += (1 - ListPadding) * (Open ? -1 : 1) * new Vector2(0, Lfold.FoldPanelSize.y - Lfold.TitleHeight);
                    //MoveContentToOpenFold(Lfold, FoldDirection_Vertical, Open);
                }
                else
                {
                    //contentRect.anchoredPosition += (1 - ListPadding) * (Open ? 1 : -1) * new Vector2(Lfold.FoldPanelSize.x - Lfold.TitleHeight, 0);
                    //MoveContentToOpenFold(Lfold, FoldDirection_Horizontal, Open);
                }

            }
        
        }
             * */
        }//Disable
    }
    void SetOpen(GameObject obj, bool Open)
    {
        int Lindex = ScrollList.GetKey().FindIndex(t => t == obj);

        if (Lindex >= 0)
        {
            var foldObj = obj.GetComponent<FoldPanel>();
            var ContentRect = Content.GetComponent<RectTransform>();// 이거말고 좌상단 기준으로 
            Vector2 LeftTop = new Vector2(ContentRect.offsetMin.x, -ContentRect.offsetMax.y);

            foldObj.SetOpen(Open, false);
            //ScrollList.GetKey(Lindex).transform.GetChild

            if (Vertical)
            {
                WidgetExpand.SetTransform(ContentRect, LeftTop, new Vector2(0, ScrollEndVaule), Vector2.zero);
            }else
            {
                WidgetExpand.SetTransform(ContentRect, LeftTop, new Vector2(ScrollEndVaule, 0), Vector2.zero);
            }

            RePosition();
        }
    }//Not Use
    void MoveContentToOpenFold(FoldPanel fold, FoldPanel.FoldDirection direction, bool Open)
    {
        var contentRect = Content.GetComponent<RectTransform>();

        switch (direction)
        {
            case FoldPanel.FoldDirection.RightToLeft:
                {
                    contentRect.anchoredPosition += (ListPadding) * (Open ? -1 : 1) * new Vector2(fold.PanelSize.x - fold.TitleHeight, 0);
                    break;
                }
            case FoldPanel.FoldDirection.LeftToRight:
                {
                    contentRect.anchoredPosition += (1 - ListPadding) * (Open ? 1 : -1) * new Vector2(fold.PanelSize.x - fold.TitleHeight, 0);
                    break;
                }
            case FoldPanel.FoldDirection.TopToButtom:
                {
                    contentRect.anchoredPosition += (1 - ListPadding) * (Open ? -1 : 1) * new Vector2(0, fold.PanelSize.y - fold.TitleHeight);
                    break;
                }
            case FoldPanel.FoldDirection.ButtomToTop:
                {
                    contentRect.anchoredPosition += (ListPadding) * (Open ? -1 : 1) * new Vector2(0, fold.PanelSize.y - fold.TitleHeight);
                    break;
                }
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
            owner.AddFold( false);
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
