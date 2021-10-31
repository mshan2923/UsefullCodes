using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputWidget : MonoBehaviour
{
    public GameObject ParentObject;
    public GameObject WaitingInputWidget;

    [Space(5)]
    public KeyCode SelectedKeycode = KeyCode.None;
    public bool AutoDestroy = true;

    GameObject WaitingWidget;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public InputDetect.DelegateSelected GetSelectedEvent()
    {
        if (WaitingWidget == null)
        {
            WaitingWidget = GameObject.Instantiate<GameObject>(WaitingInputWidget);
            WaitingWidget.transform.SetParent(ParentObject == null? gameObject.transform : ParentObject.transform);

        }
        else
        {
            WaitingWidget.SetActive(true);
        }

        WidgetExpand.SetTransform(WaitingWidget.GetComponent<RectTransform>(), Vector2.zero, new Vector2(Screen.width, Screen.height), Vector2.one * 0.5f);

        InputDetect temp = WaitingWidget.GetComponent<InputDetect>();
        if (temp == null)
        {
            return null;
        }else
        {
            temp.SelectedEvent = new InputDetect.DelegateSelected(RecieveEvent);
            return temp.SelectedEvent;
        }
    }

    public void RecieveEvent(KeyCode key, GameObject sender)
    {
        SelectedKeycode = key;

        if (AutoDestroy)
            Destroy(sender);
    }
}
