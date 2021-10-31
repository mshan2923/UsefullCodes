using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlarmWidget : MonoBehaviour
{
    public bool AutoDestroy = true;

    public Button ApplyButton;
    [Header("Optional")]
    public Button CencleButton;

    public UnityEngine.Events.UnityEvent onApply;
    //인수가 있는거는 클래스 재정의 필요 

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
            onApply.Invoke();

        if (AutoDestroy)
            Destroy(gameObject);
    }
    public void CencleEvent()
    {
        //if (onCancle != null)
        //    onCancle.Invoke("Test");

        if (AutoDestroy)
            Destroy(gameObject);
    }
}
