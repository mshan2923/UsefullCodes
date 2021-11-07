using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragNDrop : MonoBehaviour , IPointerDownHandler, IPointerUpHandler //IPointerMoveHandler -> 드래그하다 영역 넘어가면 중단
{
    public enum MouseState
    {
        Down, Press , Up
    }

    //드래그드랍 될 위젯에 이스크립트 추가
    RectTransform ObjRect;
    RectTransform CloneRect;

    public bool ShowDrag = true;
    public bool OnlyMoveObject = false;
    //드래그 , 드랍 따로 체크 - 드래그는 여기서 설정 , 드랍은 델리게이트 리턴으로 OR 강제 드랍
    //public GameObject DragArea;//++ 드래그 대상 제한

    //private bool ConsolVersionDND = false;//DragClone - true  , ShowDrag - false , OnlyMoveObject - false
    //콘솔 드래그드랍은 싱글톤으로 만들어야함, 이것도 드래그드랍은 가능하긴한데
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
    int InputButton;

    void Start()
    {
        ObjRect = gameObject.GetComponent<RectTransform>();
        gr = FindObjectOfType<GraphicRaycaster>();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        MouseDownPosition = eventData.position;
        WidgetDownPosition = ObjRect.anchoredPosition;
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
            CloneObject.transform.SetParent(MainCanvasSingleton.Instance.MainCanvas.transform);            

            CloneRect = CloneObject.GetComponent<RectTransform>();
            CloneObject.transform.position = Input.mousePosition;
            //CloneRect.anchoredPosition = WidgetDownPosition;
            CloneRect.pivot = Vector2.up;

            CloneObject.transform.SetAsLastSibling();//Layer Draw 우선순위 변경

        }
        else
        {
            gameObject.transform.SetAsLastSibling();////Layer Draw 우선순위 변경
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
                        CloneRect.transform.position = Input.mousePosition;//WidgetDownPosition + (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - MouseDownPosition);    
                }
                else
                {
                    ObjRect.anchoredPosition = WidgetDownPosition + (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - MouseDownPosition);
                }

                
            }
        }

        CountFrame++;
        yield return new WaitForEndOfFrame();
        DragLoop = StartCoroutine(Dragging());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Input.GetMouseButton((int)eventData.button))
        {
            InputButton = (int)eventData.button;
            StartCoroutine(WaitForUp());

            return;
        }

        Pressing = false;
        StopCoroutine(DragLoop);

        bool Laccess = false;
        bool DCP = false;//DontChangePosition - Move Object

        if (OnlyMoveObject)
        {
            Laccess = true;

            if (DragNDropEvent != null)
                DragNDropEvent.Invoke(gameObject, MouseState.Up, null, ref DCP);
        }
        else
        {
            GameObject Pointing = GetBehideObject(DragClone ? CloneObject : gameObject, RaycasterFillter);
            if (DragNDropEvent != null)
            {
                
                Laccess = DragNDropEvent.Invoke(gameObject, MouseState.Up, Pointing, ref DCP);
                //eventData.pointerCurrentRaycast.gameObject
            }
        }

        if (DCP)
        {
            if (ObjRect != null)
                ObjRect.anchoredPosition = WidgetDownPosition;

            if (OnlyMoveObject)
                ObjRect.anchoredPosition = WidgetDownPosition + (eventData.position - MouseDownPosition);
        }
        else
        {
            if (ObjRect != null)
            {
                if (Laccess)
                {
                    ObjRect.anchoredPosition = WidgetDownPosition + (eventData.position - MouseDownPosition);
                }
                else
                {
                    ObjRect.anchoredPosition = WidgetDownPosition;
                }
            }
        }
        if (DragClone)
        {
            DestroyImmediate(CloneObject);
        }
    }

    IEnumerator WaitForUp()
    {

        if (Input.GetMouseButton(InputButton))
        {
            yield return new WaitForEndOfFrame();
            StartCoroutine(WaitForUp());
        }else
        {
            Pressing = false;
            StopCoroutine(DragLoop);

            bool Laccess = false;
            bool DCP = false;//DontChangePosition - Move Object

            if (OnlyMoveObject)
            {
                Laccess = true;

                if (DragNDropEvent != null)
                    DragNDropEvent.Invoke(gameObject, MouseState.Up, null, ref DCP);
            }
            else
            {
                GameObject Pointing = GetBehideObject(DragClone ? CloneObject : gameObject, RaycasterFillter);
                if (DragNDropEvent != null)
                {

                    Laccess = DragNDropEvent.Invoke(gameObject, MouseState.Up, Pointing, ref DCP);
                    //eventData.pointerCurrentRaycast.gameObject
                }
            }

            /*
            if (DCP)
            {
                if (ObjRect != null)
                    ObjRect.anchoredPosition = WidgetDownPosition;
            }
            else
            {
                if (ObjRect != null)
                {
                    if (Laccess)
                    {
                        ObjRect.anchoredPosition = WidgetDownPosition + (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - MouseDownPosition);
                    }
                    else
                    {
                        ObjRect.anchoredPosition = WidgetDownPosition;
                    }
                }
            }*/

            if (DragClone)
            {
                DestroyImmediate(CloneObject);
            }
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
    public GameObject GetBehideObject(GameObject ExceptionTarget, Component Fillter)
    {
        List<RaycastResult> results = WidgetLineTrace();
        for (int i = 0; i < results.Count; i++)
        {
            if(Fillter == null)
            {
                return results[0].gameObject;
            }

            if (results[i].gameObject.GetComponent(Fillter.GetType().Name) && results[i].gameObject != ExceptionTarget)
            {
                return results[i].gameObject;
            }//Fillter의 해당 컴포넌트를 가진것만 + Target이 아닌 첫번째 오브젝트를 리턴
        }

        return null;
    }
}
