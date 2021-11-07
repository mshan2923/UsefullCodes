using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabWidget : MonoBehaviour
{
    public bool IsThisButtonListPanel = true;

    public int CloseSlotIndex = -1;
    public IntMap<GameObject> SlotWidget;//ButtonIndex , SlotWidget Prefab
    public IntMap<UnityEvent> SlotEvent;

    public IntMap<GameObject> SpawnWidget = new();

    [Header("Optional")]
    public RectTransform Parent;
    public Rect SlotWidgetRect;

    GraphicRaycaster gr;

    void Start()
    {
        if (IsThisButtonListPanel)
        {
            Button temp;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                temp = gameObject.transform.GetChild(i).gameObject.GetComponent<Button>();
                temp.onClick.AddListener(ReceiveEvent);
            }
        }

        gr = FindObjectOfType<GraphicRaycaster>();

        {
            /*
            if (SlotEvent.Count > 0)
            {
                SlotEvent.Get(0).vaule.AddListener(TestCode);
            }
            */
        }//Testing Code / Disable - Working
    }
    
    void ReceiveEvent()
    {
        //추가 컨포넌트 없이 구분할려면 마우스위치 으로 가리키고 있는 오브젝트으로
        GameObject Target = WidgetExpand.GetBehideObject(gr, typeof(Button));
        int LIndex = -1;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject == Target)
                LIndex = i;
        }

        if (CloseSlotIndex == LIndex)
        {
            Destroy(gameObject);
        }

        if (SlotWidget.FindToKey(LIndex) != null)
        {
            GameObject Lobj;

            //Spawn OR Active
            if (SpawnWidget.Exist(LIndex))//! SpawnWidget.Exist(LIndex / SpawnWidget.FindToKey(LIndex)
            {
                Lobj = SpawnWidget.FindToKey(LIndex);
            }
            else
            {
                //Spawn to FirstTime
                Lobj = Instantiate(SlotWidget.FindToKey(LIndex));
                SpawnWidget.Add(LIndex, Lobj);
            }
            if (SpawnWidget.FindToKey(LIndex) == null)
            {
                //Spawn to Destory Past Widget
                Lobj = Instantiate(SlotWidget.FindToKey(LIndex));
                SpawnWidget.SetToKey(LIndex, Lobj);
            }

            ActiveWidget(Lobj);
        }
        if (SlotEvent.FindToKey(LIndex) != null)
        {
            //Invoke Event
            SlotEvent.FindToKey(LIndex).Invoke();
        }
    }

    void ActiveWidget(GameObject obj)
    {
        var Lrect = obj.GetComponent<RectTransform>();



        if (Parent == null)
        {
            //Fullscreen

            Lrect.anchorMin = Vector2.one * 0.5f;
            Lrect.anchorMax = Vector2.one * 0.5f;
            Lrect.pivot = Vector2.one * 0.5f;

            obj.transform.SetParent(MainCanvasSingleton.Instance.MainCanvas.transform);

            WidgetExpand.SetTransform(Lrect, SlotWidgetRect.position, (new Vector2(Screen.width, Screen.height) + SlotWidgetRect.size), Vector2.one * 0.5f);
            //WidgetExpand.SetPadding(Lrect, Offset.xMin, Offset.yMin, Offset.xMax, Offset.yMax);
        }
        else
        {
            Lrect.anchorMin = Vector2.zero;
            Lrect.anchorMax = Vector2.one;
            Lrect.pivot = Vector2.one * 0.5f;

            obj.transform.SetParent(Parent);
            WidgetExpand.SetTransform(Lrect, SlotWidgetRect.position, (SlotWidgetRect.size), Vector2.one * 0.5f);
            //WidgetExpand.SetPadding(Lrect, Offset.xMin, Offset.yMin, Offset.xMax, Offset.yMax);
        }
    }

    void TestCode()
    {
        Debug.Log("Testing");
    }
}
