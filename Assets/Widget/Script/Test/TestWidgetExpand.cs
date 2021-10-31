using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestWidgetExpand : MonoBehaviour
{
    public Button ButtonObject;

    public Vector2 DrawPos = new Vector2();
    public Vector2 DrawSize = new Vector2();
    public Vector2 DrawPivot = Vector2.one;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Test()
    {
        var buttonRect = ButtonObject.gameObject.GetComponent<RectTransform>();
        WidgetExpand.SetTransform(buttonRect, DrawPos, DrawSize, DrawPivot);
    }

    public void TestWorld()
    {
        var buttonRect = ButtonObject.gameObject.GetComponent<RectTransform>();
        WidgetExpand.SetWorldTransform(buttonRect, DrawPos, DrawSize, DrawPivot);
    }

}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(TestWidgetExpand))]
public class TestWidgetExpandEditor : UnityEditor.Editor
{
    TestWidgetExpand owner;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        owner = target as TestWidgetExpand;

        if (GUILayout.Button("Testing"))
        {
            owner.Test();
        }

        if (GUILayout.Button("TestingToWorld"))
        {
            owner.TestWorld();
        }
    }
}
#endif