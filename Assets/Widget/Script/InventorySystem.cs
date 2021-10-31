using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySystem : MonoBehaviour , InventorySystem.IChangeItem
{
    public interface IChangeItem
    {
        InventorySlotData ChangeItem(GameObject DataOnwer, InventorySlotData Data, GameObject Target);//Trade InventorySlotData
    }

    [System.Serializable]
    public class InventorySlotData
    {
        public GameObject Object;
        public Material material;
        public Color color = Color.white;
        public int Data = -1;//외부 데이터 저장공간 인덱스
        public bool IsDefaultVaule = true;

        //public bool IsFold = false;
        //public bool IsFoldOpen = false;
        public void Copy(InventorySlotData Source, bool WithoutObject = true)
        {
            if (! WithoutObject)
            {
                Object = Source.Object;
            }
            material = Source.material;
            color = Source.color;
            Data = Source.Data;
        }

        public static void SwapWithoutObject(InventorySlotData a, InventorySlotData b)
        {
            InventorySlotData Temp = a;

            a = b;
            a.Object = Temp.Object;

            b = Temp;
            b.Object = a.Object;
        }
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
    public bool UseFold = false;//추가할때 FoldIndex가 0 이상이면 활성화
    public FoldPanel.FoldDirection foldDirection = FoldPanel.FoldDirection.TopToButtom;
    public float FoldTitleHeight = 30f;
    public float FoldHeight = 100;

    [Space(10), Header("Data"), SerializeField]
    private InventorySlotData defaultSlot = new InventorySlotData();
    public InventorySlotData DefaultSlot 
    { 
        get
        {
            var Temp = new InventorySlotData();
            Temp.Copy(defaultSlot, false);

            return Temp;
        }
    }//자동으로 참조되서...

    public int InventorySlotAmount = 10;
    public Map<int ,InventorySlotData> InventoryDatas = new Map<int, InventorySlotData>();//int -> FoldIndex
    [SerializeField]
    Map<int, List<int>> SortInventory = new Map<int, List<int>>();//Map <FoldIndex, InventoryDatas-index>

    [Space(5)]
    public List<string> FoldName = new List<string>();// -1, 0 1 2 ... / 오브젝트는 GetChild으로
    public string DefaultFoldName = "Other";
    public Color EmptyItemSlot = Color.gray;

    void Start()
    {
        MainPanel = gameObject.GetComponent<RectTransform>();

        Redraw();

    }

    // Update is called once per frame
    void Update()
    {
        //Title으로 드래그드랍 구현(내부 이동, 외부 이동)
    }

    //버튼이 Down, Up 상태랑 연결 , 누르고 있을때 버튼이 따라감

    #region InventoryDraw
    public void ConnectFoldOpenEvent()
    {
        if (UseFold)
        {
            for (int i = 0; i < StoragePanel.transform.childCount; i++)
            {
                var childObj = StoragePanel.transform.GetChild(i).gameObject;
                childObj.GetComponent<FoldPanel>().OpenEvnet += new FoldPanel.OpenDelegate(FoldOpenEvent);
            }
        }
    }
    public void FoldOpenEvent(GameObject Sender, bool OpenState)
    {
        FoldRePosition();
    }
    public void FoldRePosition()
    {
        float DrawHight = 0;
        if (UseFold)
        {
            for (int i = 0; i < StoragePanel.transform.childCount; i++)
            {
                var childObj = StoragePanel.transform.GetChild(i).gameObject;
                var childFold = childObj.GetComponent<FoldPanel>();
                childObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -DrawHight);//위치지정

                DrawHight += (childFold.IsOpen ? childFold.FoldPanelSize.y : childFold.TitleHeight) + SlotOffset.y;

            }
        }
    }
    public void Redraw()
    {
        {
            List<GameObject> Lchild = new List<GameObject>();

            for (int i = 0; i < StoragePanel.childCount; i++)
            {
                Lchild.Add(StoragePanel.GetChild(i).gameObject);
                //DestroyImmediate(StoragePanel.GetChild(0).gameObject);
            }

            for (int i = 0; i < Lchild.Count; i++)
            {
                DestroyImmediate(Lchild[i]);
            }

        }// 오브젝트 제거

        {
            if (MainPanel == null)
            {
                MainPanel = gameObject.GetComponent<RectTransform>();
            }

            MainPanel.sizeDelta = InventoryPanelSize;
            TitlePanel.sizeDelta = new Vector2(TitlePanel.sizeDelta.x, TitleHeight);
            //WidgetExpandScript.SetTransform(StoragePanel, new Vector2(0, TitleHeight), (InventoryPanelSize - new Vector2(0, TitleHeight)), new Vector2(0.5f, 0));
            WidgetExpand.SetPadding(StoragePanel, 0, TitleHeight, 0, 0);

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


            {
                SortInventory.Clear();

                for (int i = 0; i < InventoryDatas.Count; i++)
                {
                    bool Ladd = false;
                    int foldIndex = -1;
                    //크기가 넘으면이 아니라 일정값 초과시 FoldName.Count으로 (-1 + FoldName.Count 이상이면 FoldName.Count으로 고정)

                    if (Mathf.Max(-1, InventoryDatas.GetKey(i)) < (-1 + FoldName.Count))
                    {
                        Ladd = SortInventory.GetKey().Exists(t => t == InventoryDatas.GetKey(i));
                        foldIndex = InventoryDatas.GetKey(i);
                    }
                    else
                    {
                        Ladd = SortInventory.GetKey().Exists(t => t == FoldName.Count);
                        foldIndex = FoldName.Count;
                    }


                    foldIndex = Mathf.Clamp(foldIndex, -1, FoldName.Count);
                    if (Ladd)
                    {
                        int invenIdex = SortInventory.GetKey().FindIndex(t => t == foldIndex);
                        var Lvaule = SortInventory.GetVaule(invenIdex);
                        Lvaule.Add(i);

                        SortInventory.SetVaule(invenIdex, Lvaule);
                    }
                    else
                    {
                        SortInventory.Add(foldIndex, new List<int> { i });
                    }
                }
            }//Set SortInventory  // i -> 음수는 추가할때 -1 고정 , FoldName에서 index가 없으면 Other으로 - i 대신 FoldName.Count


            {
                Vector2 DrawPos = Vector2.zero;// new Vector2(StoragePanel.offsetMin.x, -StoragePanel.offsetMax.y);//StoragePanel 좌상단
                RectTransform rectParent = null;
                FoldPanel Lfold = null;
                int widthAmount = Mathf.RoundToInt((InventoryPanelSize.x - SlotOffset.x) / (SlotSize.x + SlotOffset.x));
                int LineAmount = 1;

                if (UseFold)
                {
                    //FoldName의 index가 없으면  Other 으로 분류
                    for (int i = -1; i <= FoldName.Count; i++)
                    {
                        int LfoldIndex = SortInventory.GetKey().FindIndex(k => k == i);

                        if (LfoldIndex >= 0)
                        {
                            LineAmount = Mathf.CeilToInt((float)SortInventory.GetVaule(LfoldIndex).Count / widthAmount) + 0;

                            string LfoldTitle = "";
                            {
                                if (i + 1 >= FoldName.Count)
                                {
                                    LfoldTitle = DefaultFoldName;
                                }
                                else
                                {
                                    LfoldTitle = FoldName[i + 1] + " FoldIndex : " + i;
                                }
                            }//Set FoldTitle

                            DrawPos = CreateFold(DrawPos, LineAmount, LfoldTitle, ref Lfold);//Mathf.ceil(아이템수 / 가로 슬롯수) + 1(여백줄)
                            rectParent = Lfold.FoldContent;
                            Lfold.OpenEvnet += new FoldPanel.OpenDelegate(FoldOpenEvent);

                            {
                                GameObject LitemObj = null;
                                var LPos = SlotOffset * new Vector2(1, -1);

                                for (int v = 0; v < LineAmount * widthAmount; v++)//Mathf.ceil(아이템수 / 가로 슬롯수) * 여백줄
                                {
                                    if (v == 0)
                                    {
                                        LPos = CreateItem(LPos, rectParent, ref LitemObj);
                                    }
                                    else if (v % widthAmount == 0)
                                    {
                                        LPos = new Vector2(SlotOffset.x, (LPos.y - (SlotSize.y + SlotOffset.y)));
                                        LPos = CreateItem(LPos, rectParent, ref LitemObj);
                                    }//줄에서 처음일때 / Item 생성시  NextLine - true
                                    else
                                    {
                                        LPos = CreateItem(LPos, rectParent, ref LitemObj);
                                    }

                                    if (v < SortInventory.GetVaule(LfoldIndex).Count)
                                    {
                                        int LinvenIndex = SortInventory.GetVaule(LfoldIndex)[v];
                                        var LinvenSlot = InventoryDatas.GetVaule(LinvenIndex);
                                        LinvenSlot.Object = LitemObj;
                                        InventoryDatas.SetVaule(LinvenIndex, LinvenSlot);

                                        LitemObj.GetComponent<Image>().material = LinvenSlot.material;
                                        LitemObj.GetComponent<Image>().color = LinvenSlot.color;

                                        LitemObj.GetComponentInChildren<Text>().text = i + " - " + v;
                                    }
                                    else
                                    {
                                        LitemObj.GetComponent<Image>().material = null;
                                        LitemObj.GetComponent<Image>().color = EmptyItemSlot;//빈 슬롯일때
                                    }

                                    DragNDrop itemDND;
                                    if (LitemObj.TryGetComponent<DragNDrop>(out itemDND))
                                    {
                                        itemDND.DragNDropEvent += new DragNDrop.DragNDropDelegate(ReceiveDragNDrop);
                                    }
                                }
                            }//Draw Items
                        }
                    }
                }
                else
                {
                    LineAmount = Mathf.CeilToInt((float)InventorySlotAmount / widthAmount);
                    rectParent = StoragePanel;

                    GameObject LitemObj = null;
                    var LPos = SlotOffset * new Vector2(1, -1);

                    for (int v = 0; v < LineAmount * widthAmount; v++)//인벤 슬롯 크기를 넘을때 표시할껀지 , 한다면 색상
                    {
                        if (v < InventoryDatas.Count && InventoryDatas.Count < InventorySlotAmount)
                        {
                            AddItem(-1, null, false);
                        }

                        if (v == 0)
                        {
                            LPos = CreateItem(LPos, rectParent, ref LitemObj);
                        }
                        else if (v % widthAmount == 0)
                        {
                            LPos = new Vector2(SlotOffset.x, (LPos.y - (SlotSize.y + SlotOffset.y)));
                            LPos = CreateItem(LPos, rectParent, ref LitemObj);
                        }
                        else
                        {
                            LPos = CreateItem(LPos, rectParent, ref LitemObj);
                        }


                        if (v < InventoryDatas.Count)
                        {
                            var Ltemp = InventoryDatas.GetVaule(v);
                            Ltemp.Object = LitemObj;
                            InventoryDatas.SetVaule(v, Ltemp);

                            LitemObj.GetComponent<Image>().material = InventoryDatas.GetVaule(v).material;
                            LitemObj.GetComponent<Image>().color = InventoryDatas.GetVaule(v).color;
                        }
                        else
                        {
                            LitemObj.GetComponent<Image>().material = null;
                            LitemObj.GetComponent<Image>().color = EmptyItemSlot;//빈 슬롯일때
                        }

                        DragNDrop itemDND;
                        if (LitemObj.TryGetComponent<DragNDrop>(out itemDND))
                        {
                            itemDND.DragNDropEvent += new DragNDrop.DragNDropDelegate(ReceiveDragNDrop);
                        }
                    }
                }

            }//Spawn Fold , Slot

        }//Slot 위치지정 , 머티리얼 + 색상 적용

    }
    Vector2 CreateFold(Vector2 DrawPos, int DrawLine, string FoldTitle ,ref FoldPanel Fold)
    {
        RectTransform rectParent;

        if (UseFold)
        {
            var Lfold = GameObject.Instantiate(FoldObject);
            Fold = Lfold;

            rectParent = Lfold.gameObject.GetComponent<RectTransform>();
            Lfold.transform.SetParent(StoragePanel);

            {
                rectParent.anchorMin = new Vector2(0, 1);
                rectParent.anchorMax = new Vector2(1, 1);
                rectParent.pivot = new Vector2(0.5f, 1);
            }//TopStretch으로 앵커

            rectParent.anchoredPosition = new Vector3(0, DrawPos.y);

            {
                Lfold.Direction = foldDirection;
                Lfold.TitleHeight = FoldTitleHeight;
                Lfold.FoldPanelSize = new Vector2(StoragePanel.rect.width, ((SlotSize.y + SlotOffset.y) * DrawLine + FoldTitleHeight + SlotOffset.y));
                Lfold.FoldButton.GetComponentInChildren<Text>().text = FoldTitle;
            }//폴드 방향, 폴드 크기, 폴드 이름
            Lfold.ReDraw();

            DrawPos = new Vector2(0, DrawPos.y - (Lfold.TitleHeight + SlotOffset.y));//new Vector2(0, DrawPos.y + (SlotSize.y + SlotOffset.y) * DrawLine);//new Vector2(0, Slot 높이 * 즐 갯수)

        }//폴드의 생성 + 크기할당  , 부모 지정
        else
        {
            //DrawPos += (SlotSize + SlotOffset) * 0.5f;//=====new Vector2(0, Slot 높이 * 즐 갯수)
            rectParent = StoragePanel;
            Fold = null;
        }

        return DrawPos;
    }
    Vector2 CreateItem(Vector2 DrawPos , RectTransform ParentFold, ref GameObject ItemObj)
    {
        GameObject spawn = DefaultSlot.Object;
        if (spawn == null)
            Debug.LogError("Can't Spawn - Corrupted DefaultSlot.Object");

        var Lslot = GameObject.Instantiate(spawn);
        var slotRect = Lslot.GetComponent<RectTransform>();
        ItemObj = Lslot;

        Lslot.transform.SetParent(ParentFold);

        {
            slotRect.anchorMin = new Vector2(0, 1);
            slotRect.anchorMax = new Vector2(0, 1);
            slotRect.pivot = new Vector2(0, 1);
        }//TopLeft으로 앵커

        slotRect.anchoredPosition = DrawPos;
        slotRect.sizeDelta = SlotSize;


        return DrawPos + new Vector2(SlotSize.x + SlotOffset.x, 0);
    }

    public void AddSlot(int Amount)
    {
        InventorySlotAmount += Amount;
    }
    public bool AddItem(int foldIndex, InventorySlotData data = null, bool redraw = true)
    {
        int Lindex = -1;
        if (foldIndex >= 0)
        {
            Lindex = foldIndex;
        }else
        {
            Lindex = -1;
        }

        if (data == null)
        {
            data = DefaultSlot;
            data.IsDefaultVaule = true;
        }
        else
        {
            data.IsDefaultVaule = false;
        }

        if (InventoryDatas.Count >= InventorySlotAmount)
        {
            int EmptyIndex = InventoryDatas.GetVaule().FindIndex(t => t.IsDefaultVaule);
            if (EmptyIndex >= 0)
            {
                InventoryDatas.SetKey(EmptyIndex, foldIndex);
                InventoryDatas.SetVaule(EmptyIndex, data);

                if (redraw)
                    Redraw();
                return true;
            }//가득 찼지만 , 기본값인 슬롯에 덮어쓰기
            return false;
        }

        InventoryDatas.Add(Lindex, data);

        if (redraw)
            Redraw();
        return true;
    }
    public bool AddItem(int foldIndex, int OutsideData)
    {
        var Lslot = DefaultSlot;
        Lslot.Data = OutsideData;
        Lslot.color = new Color(Random.value, Random.value, Random.value);

        return AddItem(foldIndex, Lslot);
    }
    public void AddFold(string name)
    {
        FoldName.Add(name);
        //=======FoldPanel 생성후 등록
    }
    #endregion

    public InventorySlotData ChangeItem(GameObject DataOnwer, InventorySlotData Data, GameObject Target)
    {
        int itemIndex = InventoryDatas.GetVaule().FindIndex(t => t.Object == Target);
        InventorySlotData SendVaule = new InventorySlotData();

        if (itemIndex >= 0)
        {
            SendVaule.Copy(InventoryDatas.GetVaule(itemIndex));

            var LocalVaule = InventoryDatas.GetVaule(itemIndex);
            LocalVaule.Copy(Data);
            InventoryDatas.SetVaule(itemIndex, LocalVaule);

            if (DataOnwer == gameObject)
            {

            }
            else
            {

            }
        }

        Redraw();
        return SendVaule;
    }//Empty
    public bool ReceiveDragNDrop(GameObject DragObject, DragNDrop.MouseState state, GameObject PointingObject, ref bool DontChangePos)
    {
        switch (state)
        {
            case DragNDrop.MouseState.Down:
                break;
            case DragNDrop.MouseState.Press:
                {
                    //PointingObject가 색변하거나 상호작용
                    gameObject.transform.SetAsLastSibling();//Layer Draw 우선순위 변경
                    break;
                }
            case DragNDrop.MouseState.Up:
                {
                    DontChangePos = true;

                    int DragIndex = InventoryDatas.GetVaule().FindIndex(d => d.Object == DragObject);
                    int TargetIndex = InventoryDatas.GetVaule().FindIndex(t => t.Object == PointingObject);
                    InventorySlotData TempDragItem = null;
                    Vector3 TempPos;


                    if (DragIndex >= 0 && TargetIndex >= 0)
                    {
                        TempPos = DragObject.transform.position;
                        DragObject.transform.position = PointingObject.transform.position;
                        PointingObject.transform.position = TempPos;


                        TempDragItem = InventoryDatas.GetVaule(DragIndex);
                        InventoryDatas.SetVaule(DragIndex, InventoryDatas.GetVaule(TargetIndex));
                        InventoryDatas.SetVaule(TargetIndex, TempDragItem);
                    }
                    else if (DragIndex >= 0 && TargetIndex == -1)
                    {
                        if (PointingObject != null)
                        {
                            var iChangItem = PointingObject.GetComponentInParent<IChangeItem>();
                            if (iChangItem != null)
                            {
                                if (iChangItem == gameObject.GetComponent<IChangeItem>())
                                {
                                    int TempFoldIndex = InventoryDatas.GetKey(DragIndex);
                                    TempDragItem = InventoryDatas.GetVaule(DragIndex);

                                    InventoryDatas.Remove(DragIndex);
                                    AddItem(TempFoldIndex, TempDragItem);

                                    return true;
                                }else
                                {
                                    var ChangeItem = iChangItem.ChangeItem(gameObject, InventoryDatas.GetVaule(DragIndex), PointingObject);
                                    var LVaule = InventoryDatas.GetVaule(DragIndex);

                                    LVaule.Copy(ChangeItem);
                                    InventoryDatas.SetVaule(DragIndex, LVaule);

                                    Redraw();
                                    return true;
                                }//Trade Other Inventory
                            }
                        }//DragObject 와 PointingObject가 같은 인터페이스를 쓰는경우 맨뒤로 옮기기 / 다르다면 인터페이스 보내기

                        return false;
                    }
                    else if (DragIndex == -1 && TargetIndex >= 0)
                    {
                        //외부 데이터 - 인터페이스로 처리해서...
                    }else
                    {
                        // 딴걸 끌어왔거나
                        return false;
                    }
                    break;
                }
                
        }

        return true;
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
        if (GUILayout.Button("Add Slot"))
        {
            Onwer.AddSlot(1);
        }
        if (GUILayout.Button("Default Add Item"))
        {
            Onwer.AddItem(Random.Range(-1, 2));
        }
        if (GUILayout.Button("Random FoldIndex Add Item"))
        {
            Onwer.AddItem(Random.Range(-1, 2), 0);
        }
        if (GUILayout.Button("Reset Inventory"))
        {
            Onwer.InventorySlotAmount = 10;
            Onwer.InventoryDatas.Clear();
            Onwer.Redraw();
        }
    }
}
#endif 