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
    //�μ��� �ִ°Ŵ� Ŭ���� ������ �ʿ� 

    /*
    [System.Serializable]
    public class CancleEvent : UnityEngine.Events.UnityEvent<string>
    {
    }
    public CancleEvent onCancle;
    *///--�׳� �׽�Ʈ , �ߵ�, �����Ϳ��� �����Ͽ� �����ϰ� �������� ����

    void Start()
    {
        if (ApplyButton != null)
            ApplyButton.onClick.AddListener(ApplyEvent);
        if (CencleButton != null)
            CencleButton.onClick.AddListener(CencleEvent);

        gameObject.transform.SetAsLastSibling();//Layer Draw �켱���� ����
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
