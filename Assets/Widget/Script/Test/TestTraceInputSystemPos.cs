using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTraceInputSystemPos : MonoBehaviour
{
    public InputSystem inputSystem;
    public RectTransform ParentRect;

    public abstract class TestBase<T>
    {
        public T Lvar;
    }

    public class TestChild : TestBase<float>
    {
        void Test()
        {

        }
    }

    void Start()
    {
        inputSystem = InputSystem.Instance;//프로젝트에있는 프리팹을 가르킴
        ParentRect = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 Temp = new Vector2(InputSystem.Instance.ButtonSingleAxis[0].GetVaule(), 0);
        Vector2 Temp = InputSystem.Instance.ButtonVector2Axis[0].GetVaule();

        //gameObject.transform.position = Temp;
        ParentRect.anchoredPosition = Temp;
    }
}
