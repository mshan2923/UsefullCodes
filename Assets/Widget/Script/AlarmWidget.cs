using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlarmWidget : MonoBehaviour
{
    [System.Serializable]
    public class AlarmEvent : UnityEngine.Events.UnityEvent<bool>
    {

    }
    public bool AutoDestroy = true;

    public Button ApplyButton;
    [Header("Optional")]
    public Button CencleButton;

    public AlarmEvent onApply;

    /*
    [System.Serializable]
    public class CancleEvent : UnityEngine.Events.UnityEvent<string>
    {
    }
    public CancleEvent onCancle;
    *///--그냥 테스트 , 잘됨, 에디터에서 연결하여 간단하게 변수변경 가능

    void Start()
    {
        if (ApplyButton != null)
            ApplyButton.onClick.AddListener(ApplyEvent);
        if (CencleButton != null)
            CencleButton.onClick.AddListener(CencleEvent);

        gameObject.transform.SetAsLastSibling();//Layer Draw 우선순위 변경
    }

    public void ApplyEvent()
    {
        if (onApply != null)
            onApply.Invoke(true);

        if (AutoDestroy)
            Destroy(gameObject);
    }
    public void CencleEvent()
    {
        if (onApply != null)
            onApply.Invoke(false);

        if (AutoDestroy)
            Destroy(gameObject);
    }
}
