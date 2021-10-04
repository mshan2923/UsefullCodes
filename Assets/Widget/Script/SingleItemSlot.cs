using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static InventorySystem;

public class SingleItemSlot : MonoBehaviour, IChangeItem
{
    public GameObject SpawnPath;
    public InventorySlotData Default;
    public InventorySlotData Data;

    RectTransform ItemRect;
    DragNDrop itemDND;


    void Start()
    {
        Redraw();
    }

    public void Redraw()
    {
        if (SpawnPath == null)
        {
            SpawnPath = gameObject;
        }

        {
            List<GameObject> Lchild = new List<GameObject>();

            for (int i = 0; i < SpawnPath.transform.childCount; i++)
            {
                Lchild.Add(SpawnPath.transform.GetChild(i).gameObject);
                //DestroyImmediate(StoragePanel.GetChild(0).gameObject);
            }

            for (int i = 0; i < Lchild.Count; i++)
            {
                DestroyImmediate(Lchild[i]);
            }

        }// 오브젝트 제거

        if (Default.Object == null)
            return;

        Data.Object = GameObject.Instantiate(Default.Object);
        ItemRect = Data.Object.GetComponent<RectTransform>();

        ItemRect.SetParent(SpawnPath.transform);

        ItemRect.sizeDelta = SpawnPath.GetComponent<RectTransform>().sizeDelta;
        ItemRect.anchoredPosition = Vector2.zero;

        Data.Object.GetComponent<Image>().material = Data.material;
        Data.Object.GetComponent<Image>().color = Data.color;


        if (Data.Object.TryGetComponent<DragNDrop>(out itemDND))
        {
            itemDND.DragNDropEvent += new DragNDrop.DragNDropDelegate(ReceiveDragNDrop);
        }
    }
    public InventorySlotData ChangeItem(GameObject DataOnwer, InventorySlotData LData, GameObject Target)
    {       
        InventorySlotData Temp = new InventorySlotData();
        Temp.Copy(Data);

        if (LData == null)
            Data.Copy(Default);
        else
            Data.Copy(LData);

        Redraw();
        return Temp;
    }
    public bool ReceiveDragNDrop(GameObject DragObject, DragNDrop.MouseState state, GameObject PointingObject, ref bool DontChangePos)
    {
        switch (state)
        {
            case DragNDrop.MouseState.Down:
                {
                    return true;
                }
            case DragNDrop.MouseState.Press:
                {
                    gameObject.transform.SetAsLastSibling();//Layer Draw 우선순위 변경
                    return true;
                }
            case DragNDrop.MouseState.Up:
                {
                    if (PointingObject != null)
                    {
                        var iChangItem = PointingObject.GetComponentInParent<IChangeItem>();
                        {
                            if (iChangItem != null)
                            {
                                if (iChangItem != gameObject.GetComponent<IChangeItem>())
                                {
                                    var ChangeItem = iChangItem.ChangeItem(gameObject, Data, PointingObject);
                                    {
                                        Debug.Log("Single Local Item : " + Data.color + " Retrun to Inven : " + ChangeItem.color);

                                        if (ChangeItem != null)
                                            Data.Copy(ChangeItem);
                                        else
                                            Data.Copy(Default);
                                    }
                                    Redraw();
                                    return true;
                                }
                            }
                        }
                        break;
                    }else
                    {
                        return false;
                    }
                }
        }
        return false;
    }
}
