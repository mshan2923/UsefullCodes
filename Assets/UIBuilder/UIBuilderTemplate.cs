using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class UIBuilderTemplate : MonoBehaviour
{
    public string Test;
    public string Test2;
    //Viewport�Ʒ� File���� uxml ���� >> Hierarchy ���� ~~.uxml Ŭ���� �����ʿ� Editor Extension Authoring���� �������������� ��ȯ
    //  >> PropertyField ��ġ�� PropertyField ���� BindingPath�� ���������� �ϸ� �ڵ�����
    //  ++ BindingPath ���ϸ� ������� ����
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(UIBuilderTemplate))]
public class UIBuilderEditor : Editor_UIBuilder
{
    public override VisualElement CreateInspectorGUI()
    {
        base.CreateInspectorGUI();

        if (element.Q<Button>("button") != null)
            element.Q<Button>("button").clicked += UIBuilderEditor_onClick;

        {
            element.Add(new Label("CreateInspectorGUI"));//Add a Simple Label

            //VisualElement inspectorFold = inspector.Q("Default_Inspector");//Get a reference to the default inpector foldout control

            //InspectorElement.FillDefaultInspector(inspectorFold, serializedObject, this);//Attach a default inspector to thr foldout
        }

        return element;
    }
    public override void Redraw()
    {
        base.Redraw();

        Debug.Log("Redraw");
    }
    private void UIBuilderEditor_onClick()
    {
        Debug.Log("Clicked");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("InspectorGUI");
    }//NotWork
}
#endif