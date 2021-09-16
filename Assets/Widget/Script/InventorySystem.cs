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
        public int Data = -1;//�ܺ� ������ ������� �ε���

        public bool IsFold = false;
        public bool IsFoldOpen = false;
    }

    //gameobject - Background Panel
    RectTransform MainPanel;
    public RectTransform TitlePanel;
    public RectTransform StoragePanel;

    [Header("Panel , Slot")]
    public Vector2 InventoryPanelSize = new Vector2(250, 350);//==========
    public float TitleHeight = 30f;
    public Vector2 SlotSize = new Vector2(30, 30);
    public Vector2 SlotOffset = new Vector2(10, 10);
    public float SlotStartHeight = 0.5f;

    [Space(10), Header("Fold")]
    public FoldPanel FoldObject;
    public bool UseFold = false;//�߰��Ҷ� FoldIndex�� 0 �̻��̸� Ȱ��ȭ
    public FoldPanel.FoldDirection foldDirection = FoldPanel.FoldDirection.TopToButtom;
    public float FoldTitleHeight = 30f;
    public float FoldHeight = 100;

    [Space(10), Header("Data")]
    public InventorySlotData DefaultSlot = new InventorySlotData();
    public int InventorySlotAmount = 10;
    Map<int ,InventorySlotData> InventoryDatas = new Map<int, InventorySlotData>();//int -> FoldIndex
    Map<int, List<int>> SortInventory = new Map<int, List<int>>();

    [Space(5)]
    public Map<string, GameObject> FoldName = new Map<string, GameObject>();


    void Start()
    {
        MainPanel = gameObject.GetComponent<RectTransform>();
        Redraw();
    }

    // Update is called once per frame
    void Update()
    {
        //Title���� �巡�׵�� ����(���� �̵�, �ܺ� �̵�)
    }

    public void SetPadding(RectTransform rect, float left, float top, float right, float bottom)//Setting Rect Position & Size
    {
        rect.offsetMax = new Vector2(-right, -top);
        rect.offsetMin = new Vector2(left, bottom);

        //��Ŀ -> ��Ŀ�� ������,  ������Ÿ�����
        //�Ϲ� -> �θ� Rect����
    }
    //Template --> SetPadding(Temp, Temp.offsetMin.x, -Temp.offsetMax.y, -Temp.offsetMax.x, -Temp.offsetMin.y);


    public void Redraw()
    {
        {
            InventoryDatas.Clear();

            int Lamount = StoragePanel.childCount;
            for (int i = 0; i < Lamount; i++)
            {
                DestroyImmediate(StoragePanel.GetChild(0).gameObject);
            }

        }// ������Ʈ ����

        {
            if (MainPanel == null)
            {
                MainPanel = gameObject.GetComponent<RectTransform>();
            }

            MainPanel.sizeDelta = InventoryPanelSize;
            TitlePanel.sizeDelta = new Vector2(TitlePanel.sizeDelta.x, TitleHeight);
            //SetPadding(StoragePanel, StoragePanel.offsetMin.x, TitleHeight, -StoragePanel.offsetMax.x, -StoragePanel.offsetMin.y);
            WidgetExpandScript.SetTransform(StoragePanel, new Vector2(0, TitleHeight), (InventoryPanelSize - new Vector2(0, TitleHeight)), new Vector2(0.5f, 0));

        }//Title ũ�� , Storage ũ��

        {
            int XAmount = Mathf.FloorToInt(MainPanel.sizeDelta.x /  (SlotSize.x + SlotOffset.x));
            int YAmount = Mathf.CeilToInt((float)InventorySlotAmount / XAmount);

            if (MainPanel == null)
            {
                MainPanel = gameObject.GetComponent<RectTransform>();
            }

            Vector2 StartPos = StoragePanel.position - new Vector3(((XAmount + 1) * (SlotSize.x + SlotOffset.x) * 0.5f), 0); ;//������� ��� ,
            {
                float HighestPoint = StoragePanel.position.y + (MainPanel.sizeDelta.y - TitleHeight) * 0.5f - (SlotSize.y * 0.5f);
                float LowestPoint = StoragePanel.position.y - (MainPanel.sizeDelta.y - TitleHeight) * 0.5f + (YAmount * (SlotSize.y + SlotOffset.y) - SlotOffset.y);

                StartPos = new Vector2(StartPos.x, (HighestPoint - LowestPoint) * SlotStartHeight + LowestPoint);
            }//�ּڰ��� Slot�� ���� , �ִ��� Storage���� - Slots����


            {
                SortInventory.Clear();

                for (int i = 0; i < InventoryDatas.Count; i++)
                {
                    //InventoryDatas.GetKey(i);
                    if (SortInventory.GetKey().Exists(t => t == InventoryDatas.GetKey(i)))
                    {
                        int sortIndex = SortInventory.GetKey().FindIndex(t => t == InventoryDatas.GetKey(i));

                        var Lvaule = SortInventory.GetVaule(sortIndex);
                        Lvaule.Add(i);

                        SortInventory.SetVaule(sortIndex, Lvaule);
                    }
                    else
                    {
                        SortInventory.Add(InventoryDatas.GetKey(i), new List<int> { i });
                    }
                }//Set SortInventory

                Vector2 DrawPos = new Vector2();// new Vector2(StoragePanel.offsetMin.x, -StoragePanel.offsetMax.y);//StoragePanel �»��
                RectTransform rectParent;

                if (SortInventory.Count == 0 )
                {
                    DrawPos = CreateFold(DrawPos, out rectParent);

                    //�����и� �߰�
                }
                else
                {
                    for (int i = 0; i < SortInventory.Count; i++)
                    {
                        DrawPos = CreateFold(DrawPos, out rectParent);

                        for (int v = 0; v < SortInventory.GetVaule(i).Count; v++)// + ���� �������� �ִ밹�� �߰�
                        {
                            {

                            }//LeftTop���� ��Ŀ

                            //if (DrawPos + ����ũ�� < �κ��丮 �ʺ�) {+ ���Ի��� , �θ�����(rectParent) }
                            //else { �����ٷ� �Ѿ + ���Ի��� , �θ�����(rectParent)}
                        }
                    }
                }
            }

        }//Slot ��ġ���� , ��Ƽ���� + ���� ����

        //=============>InventoryDatas.Add();
    }
    Vector2 CreateFold(Vector2 DrawPos, out RectTransform rectParent)
    {
        if (UseFold)
        {
            var Lfold = GameObject.Instantiate(FoldObject);
            rectParent = Lfold.gameObject.GetComponent<RectTransform>();

            {
                rectParent.anchorMin = new Vector2(0, 1);
                rectParent.anchorMax = new Vector2(1, 1);
                rectParent.pivot = new Vector2(0.5f, 1);
            }//TopStretch���� ��Ŀ

            rectParent.localPosition = new Vector3(DrawPos.x, (DrawPos.y));// ======================�ᱹ SetPadding����?
            rectParent.sizeDelta = new Vector2(StoragePanel.rect.width, FoldTitleHeight);// ���� + Slot Line�� ���� ����

            {
                Lfold.Direction = foldDirection;
                Lfold.TitleHeight = FoldTitleHeight;
                Lfold.FoldPanelSize = new Vector2(StoragePanel.rect.width, FoldTitleHeight);
                Lfold.FoldButton.GetComponentInChildren<Text>().text = " Temp Fold";
            }//���� ����, ���� ũ��, ���� �̸�

            DrawPos = rectParent.offsetMin + (SlotSize + SlotOffset) * 0.5f;
            Lfold.transform.SetParent(StoragePanel);

        }//������ ���� + ũ���Ҵ� , ������(Fold ���ϴ� + Slotũ�� * 0.5f)���� , �θ� ����
        else
        {
            DrawPos += (SlotSize + SlotOffset) * 0.5f;//DrawPos +=  Slotũ�� * 0.5f)
            rectParent = StoragePanel;
        }//�Լ�ȭ

        return DrawPos;
    }
    public void AddSlot(int Amount, InventorySlotData data = null)
    {
        InventorySlotAmount += Amount;
        for (int i = 0; i < Amount; i++)
        {
            if (data == null)
            {
                InventoryDatas.Add(-1, DefaultSlot);
            }else
            {
                InventoryDatas.Add(-1, data);
            }
        }
        Redraw();
    }
    public bool AddItem(int foldIndex, InventorySlotData data = null)
    {
        if (foldIndex >= 0)
        {
            UseFold = true;
        }

        if(InventoryDatas.Count >= InventorySlotAmount)
        {
            return false;
        }

        if (data == null)
        {
            InventoryDatas.Add(foldIndex, DefaultSlot);
        }
        else
        {
            InventoryDatas.Add(foldIndex, data);
        }

        Redraw();
        return true;
    }
    public void AddFold(string name)
    {
        //=======FoldPanel ������ ���
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