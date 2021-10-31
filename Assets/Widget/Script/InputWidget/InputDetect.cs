using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputDetect : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler// , IPointerDownHandler// - �̰͵� �ȵ�
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
        
    }//�ȵ�!

    private void Start()
    {
        if (AutoFullOfScreen)
        {
            //MainCanvasSingleton.Instance.MainCanvas// ���� �������� �ȴ����� ������ �ִٸ� �̰ɷ� �Ǿ����� ����
            gameObject.transform.SetAsLastSibling();//Layer Draw �켱���� ����
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
                    //Ű���� �̺�Ʈ
                    SelectKeycode = Event.current.keyCode;
                    Selected();
                }
                else if (Event.current.pointerType == PointerType.Mouse)
                {
                    //���콺�̺�Ʈ + ���� X
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
                    }//���� �Է��� �Ϻ� ���鹮���ΰ�� ���콺 + None�� �Է��� �߰��� �� �̶� ������Ʈ �ı� ����  ...?

                }
                else
                {
                    //��Ÿ
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
