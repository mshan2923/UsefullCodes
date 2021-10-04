using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragNDrop : MonoBehaviour , IPointerDownHandler, IPointerUpHandler //IPointerMoveHandler -> �巡���ϴ� ���� �Ѿ�� �ߴ�
{
    public enum MouseState
    {
        Down, Press , Up
    }

    //�巡�׵�� �� ������ �̽�ũ��Ʈ �߰�
    RectTransform ParentRect;
    RectTransform CloneRect;

    public bool ShowDrag = true;
    public bool ForceDrop = false;
    //�巡�� , ��� ���� üũ - �巡�״� ���⼭ ���� , ����� ��������Ʈ �������� OR ���� ���
    public GameObject DragArea;//++ �巡�� ��� ����
    public bool DragClone = false;

    [Space(5)]
    bool Pressing = false;
    Vector2 MouseDownPosition = Vector2.zero;
    Vector2 WidgetDownPosition = Vector2.zero;

    public delegate bool DragNDropDelegate(GameObject DragObject , MouseState state, GameObject PointingObject, ref bool DontChangePos);
    public DragNDropDelegate DragNDropEvent;

    Coroutine DragLoop;
    public int LoopFrame = 3;
    int CountFrame = 0;

    GraphicRaycaster gr;
    GameObject CloneObject;
    public Component RaycasterFillter;

    public float CloneZPos = -1;

    void Start()
    {
        ParentRect = gameObject.GetComponent<RectTransform>();
        gr = FindObjectOfType<GraphicRaycaster>();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        MouseDownPosition = eventData.position;
        WidgetDownPosition = ParentRect.anchoredPosition;
        Pressing = true;

        if (DragNDropEvent != null)
        {
            bool DCP = false;
            DragNDropEvent.Invoke(gameObject, MouseState.Down, gameObject, ref DCP);
        }

        DragLoop = StartCoroutine(Dragging());

        if (DragClone)
        {
            CloneObject = GameObject.Instantiate(gameObject);
            CloneObject.transform.SetParent(gameObject.transform.parent);            

            CloneRect = CloneObject.GetComponent<RectTransform>();
            CloneRect.anchoredPosition = WidgetDownPosition;

            CloneObject.transform.SetAsLastSibling();//Layer Draw �켱���� ����
        }
        else
        {
            gameObject.transform.SetAsLastSibling();////Layer Draw �켱���� ����
        }
    }

    IEnumerator Dragging()
    {
        if (CountFrame == LoopFrame)
        {
            CountFrame = 0;

            if (Pressing && ShowDrag)
            {
                if (DragNDropEvent != null)
                {
                    bool DCP = false;
                    DragNDropEvent.Invoke(gameObject, MouseState.Press , GetBehideObject(DragClone ? CloneObject : gameObject, RaycasterFillter), ref DCP);
                }

                if(DragClone)
                {
                    if (CloneObject != null)
                        CloneRect.anchoredPosition = WidgetDownPosition + (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - MouseDownPosition);                    
                }else
                {
                    ParentRect.anchoredPosition = WidgetDownPosition + (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - MouseDownPosition);
                }

                
            }
        }

        CountFrame++;
        yield return new WaitForEndOfFrame();
        DragLoop = StartCoroutine(Dragging());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressing = false;
        StopCoroutine(DragLoop);

        bool Laccess = false;
        bool DCP = false;//DontChangePosition

        if (ForceDrop)
        {
            Laccess = true;
        }
        else
        {
            if (DragNDropEvent != null)
            {
                
                Laccess = DragNDropEvent.Invoke(gameObject, MouseState.Up, GetBehideObject(DragClone ? CloneObject : gameObject, RaycasterFillter), ref DCP);
                //eventData.pointerCurrentRaycast.gameObject
            }
        }

        if (! DCP)
        {
            if (ParentRect != null)
            {
                if (Laccess)
                {
                    ParentRect.anchoredPosition = WidgetDownPosition + (eventData.position - MouseDownPosition);
                }
                else
                {
                    ParentRect.anchoredPosition = WidgetDownPosition;
                }
            }
        }

        if (DragClone)
        {
            DestroyImmediate(CloneObject);
        }
    }

    public List<RaycastResult> WidgetLineTrace()
    {
        var ped = new PointerEventData(null)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);

        return results;
    }
    public GameObject GetBehideObject(GameObject Target, Component Fillter)
    {
        List<RaycastResult> results = WidgetLineTrace();
        for (int i = 0; i < results.Count; i++)
        {
            if(Fillter == null)
            {
                return results[0].gameObject;
            }

            if (results[i].gameObject.GetComponent(Fillter.GetType().Name) && results[i].gameObject != Target)
            {
                return results[i].gameObject;
            }//Fillter�� �ش� ������Ʈ�� �����͸� + Target�� �ƴ� ù��° ������Ʈ�� ����
        }

        return null;
    }
}
