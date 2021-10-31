using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputDetect : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler// , IPointerDownHandler// - 이것도 안됨
{
    bool DoOnce = true;
    public bool AutoDisable = true;
    public bool AutoDestroy = false;
    public bool AutoFullOfScreen = false;
    [Space(10)]
    public KeyCode SelectKeycode = KeyCode.None; 

    public delegate void DelegateSelected(KeyCode key , GameObject Sender);
    public DelegateSelected SelectedEvent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //DoOnce = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DoOnce = false;

        EndDetect();
    }

    private void OnMouseEnter()
    {
        
    }//안됨!

    private void Start()
    {
        if (AutoFullOfScreen)
        {
            //MainCanvasSingleton.Instance.MainCanvas// 만약 가려져서 안눌리는 문제가 있다면 이걸로 맨앞으로 지정
            gameObject.transform.SetAsLastSibling();//Layer Draw 우선순위 변경
            WidgetExpand.SetTransform(gameObject.GetComponent<RectTransform>(), Vector2.zero, new Vector2(Screen.width, Screen.height), Vector2.one * 0.5f);
        }
    }

    private void OnGUI()
    {
        if (DoOnce)
        {
            //Event e = Event.current;

            if (Input.anyKey)
            {
                if (Event.current.keyCode != KeyCode.None)
                {
                    //키보드 이벤트
                    SelectKeycode = Event.current.keyCode;
                    Selected();
                }
                else if (Event.current.pointerType == PointerType.Mouse)
                {
                    //마우스이벤트 + 구별 X
                    for (int i = 0; i <= 6; i++)
                    {
                        if (Input.GetMouseButton(i))
                        {
                            SelectKeycode = (KeyCode) System.Enum.Parse(typeof(KeyCode), ("Mouse" + i));
                            Selected();
                            break;
                        }
                    }

                    if (SelectKeycode == KeyCode.None)
                    {
                        EndDetect();
                    }//만약 입력이 일부 공백문자인경우 마우스 + None인 입력이 추가로 들어감 이때 오브젝트 파괴 가능  ...?

                }
                else
                {
                    //기타
                    SelectKeycode = Event.current.keyCode;
                    Selected();
                }

                DoOnce = false;
            }
        }
    }
    public virtual void Selected()
    {
        if (SelectedEvent != null)
        {
            SelectedEvent.Invoke(SelectKeycode, gameObject);
        }

        EndDetect();
    }

    void EndDetect()
    {
        if (AutoDisable)
            gameObject.SetActive(false);

        if (AutoDestroy)
            Destroy(gameObject);
    }
}
