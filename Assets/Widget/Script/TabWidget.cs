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
    public IntMap<UnityEvent> SlotEvent;//==========Need Test

    [Header("Optional")]
    public RectTransform Parent;
    public Rect Offset;

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
        if (SlotWidget.FindToKey(LIndex) != null)//SlotWidget.GetKey().Exists(t => t == LIndex)
        {
            //Spawn OR Active
        }
        if (SlotEvent.FindToKey(LIndex) != null)//SlotEvent.GetKey().Exists(t => t == LIndex)
        {
            //Invoke Event
            SlotEvent.FindToKey(LIndex).Invoke();
        }
    }
}
