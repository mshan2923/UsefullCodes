using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [System.Serializable]
    public class InventorySlotData
    {
        public GameObject Object;
        public Material material;
        public Color color = Color.white;
        public int Data = -1;

        public bool IsFold = false;
        public bool IsFoldOpen = false;
    }

    //gameobject - Background Panel
    RectTransform MainPanel;
    public RectTransform TitlePanel;
    public RectTransform StoragePanel;

    public Vector2 InventoryPanelSize = new Vector2(250, 350);//==========
    public float TitleHeight = 30f;
    public Vector2 SlotSize = new Vector2(30, 30);
    public Vector2 SlotOffset = new Vector2(10, 10);
    public float SlotStartHeight = 0.5f;

    public bool UseFold = true;
    public InventorySlotData DefaultSlot = new InventorySlotData();
    public int InventorySlotAmount = 10;
    Map<int ,InventorySlotData> InventoryDatas = new Map<int, InventorySlotData>();//int -> FoldIndex
    public List<string> FoldName = new List<string>();


    void Start()
    {
        MainPanel = gameObject.GetComponent<RectTransform>();
        Redraw();
    }

    // Update is called once per frame
    void Update()
    {
        //Title으로 드래그드랍 구현
    }

    public void SetPadding(RectTransform rect, float left, float top, float right, float bottom)//Setting Rect Position & Size
    {
        rect.offsetMax = new Vector2(-right, -top);
        rect.offsetMin = new Vector2(left, bottom);
    }
    //Template --> SetPadding(Temp, Temp.offsetMin.x, -Temp.offsetMax.y, -Temp.offsetMax.x, -Temp.offsetMin.y);


    public void Redraw()
    {
        {
            InventoryDatas.Clear();

            for (int i = 0; i < StoragePanel.childCount; i++)
            {
                Destroy(StoragePanel.GetChild(i).gameObject);
            }

        }// 오브젝트 제거

        {
            MainPanel.sizeDelta = InventoryPanelSize;
            TitlePanel.sizeDelta = new Vector2(TitlePanel.sizeDelta.x, TitleHeight);
            SetPadding(StoragePanel, StoragePanel.offsetMin.x, TitleHeight, -StoragePanel.offsetMax.x, -StoragePanel.offsetMin.y);

        }//Title 크기 , Storage 크기

        {
            int XAmount = Mathf.FloorToInt(MainPanel.sizeDelta.x /  (SlotSize.x + SlotOffset.x));
            int YAmount = Mathf.CeilToInt((float)InventorySlotAmount / XAmount);

            if (MainPanel == null)
            {
                MainPanel = gameObject.GetComponent<RectTransform>();
            }

            Vector2 StartPos = StoragePanel.position - new Vector3(((XAmount + 1) * (SlotSize.x + SlotOffset.x) * 0.5f), 0); ;//메인페널 가운데 ,
            {
                float HighestPoint = StoragePanel.position.y + (MainPanel.sizeDelta.y - TitleHeight) * 0.5f - (SlotSize.y * 0.5f);
                float LowestPoint = StoragePanel.position.y - (MainPanel.sizeDelta.y - TitleHeight) * 0.5f + (YAmount * (SlotSize.y + SlotOffset.y) - SlotOffset.y);

                StartPos = new Vector2(StartPos.x, (HighestPoint - LowestPoint) * SlotStartHeight + LowestPoint);
            }//최솟값이 Slot의 높이 , 최댓값이 Storage높이 - Slots높이

            int LAmount = 1;
            for (int i = 0; i < YAmount ; i++)//좌우 끝쪽은 사용X
            {
                for (int j = 1; j < (XAmount + 1) && (LAmount <= InventorySlotAmount); j++, LAmount++)
                {
                    var obj = GameObject.Instantiate(DefaultSlot.Object);
                    obj.transform.SetParent(StoragePanel.gameObject.transform);
                    obj.transform.position = StartPos + new Vector2((SlotSize.x + SlotOffset.x) * j, (SlotSize.y + SlotOffset.y) * -i);

                    obj.GetComponent<RectTransform>().sizeDelta = SlotSize;
                }
            }
        }//Slot 위치지정 , 머티리얼 + 색상 적용

        //=============>InventoryDatas.Add();
    }
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(InventorySystem))]
public class InventoryEditor : UnityEditor.Editor
{
    InventorySystem Onwer;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Onwer = target as InventorySystem;
        if (GUILayout.Button("Redraw"))
        {
            Onwer.Redraw();
        }
    }
}
#endif 