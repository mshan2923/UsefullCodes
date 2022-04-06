using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class Editor_UIBuilder : Editor
{
    protected VisualTreeAsset UIbuilder;//�� ��ũ��Ʈ �̸��� �� Ŭ�����϶� Ŀ���� �ν��Ѵ��� ��������
    protected VisualElement element;

    public override VisualElement CreateInspectorGUI()
    {
        //InspectorXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIBuilder/UIBuilderTemplate.uxml");

        {
            string guID = PlayerPrefs.GetString(this.ToString());

            UIbuilder = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(guID));//GUID���� ���� / �ҷ�����

            if (element == null)
                element = new VisualElement();


            var objField = new ObjectField();
            objField.SetValueWithoutNotify(UIbuilder);
            objField.allowSceneObjects = false;
            objField.objectType = typeof(VisualTreeAsset);
            objField.RegisterValueChangedCallback(evt =>
            {
                element.Clear();
                UIbuilder = evt.newValue as VisualTreeAsset;

                PlayerPrefs.SetString(this.ToString(), AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(UIbuilder)));

                Redraw();//���� ���� �Լ��� ������ ������� �ʾ� , ���� �ٽ� ȣ���ؼ� ������Ʈ
            }
            );
            element.Add(objField);

            if (UIbuilder != null)
                UIbuilder.CloneTree(element);//Load form default

        }//VisualTreeAsset ObjectField , Load Default 

        //Debug.Log(inspector.Children().GetEnumerator().Current);//NotWork
        //�ڵ��Ҵ� ���Ҳ� ����

        //element.ElementAt // �����ϰ� ����?
        //element.hierarchy.

        //target.GetType() //�ڵ� �߰��� �������ŭ Bind ���� ���� 

        return element;
    }
    public virtual void Redraw()
    {
        CreateInspectorGUI();

        for (int i = 0; i < element.childCount; i++)
        {
            if (element.ElementAt(i).GetType() == typeof(PropertyField))
            {
                element.ElementAt(i).Bind(serializedObject);//serializedObject �� ����
                                                            //element.Q<PropertyField>().Bind(serializedObject);//ó������ �Ǽ�
            }
        }
    }
}
#endif
/*
#if UNITY_EDITOR
[CustomEditor(typeof(UIBuilderTemplate))]
public class UIBuilderEditor : Editor
{
    public VisualTreeAsset UIbuilder;//�� ��ũ��Ʈ �̸��� �� Ŭ�����϶� Ŀ���� �ν��Ѵ��� ��������
    VisualElement element;

    public override VisualElement CreateInspectorGUI()
    {
        //InspectorXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIBuilder/UIBuilderTemplate.uxml");

        {
            string guID = PlayerPrefs.GetString(this.ToString());

            UIbuilder = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(guID));//GUID���� ���� / �ҷ�����

            if (element == null)
                element = new VisualElement();


            var objField = new ObjectField();
            objField.SetValueWithoutNotify(UIbuilder);
            objField.allowSceneObjects = false;
            objField.objectType = typeof(VisualTreeAsset);
            objField.RegisterValueChangedCallback(evt =>
            {
                element.Clear();
                UIbuilder = evt.newValue as VisualTreeAsset;

                PlayerPrefs.SetString(this.ToString(), AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(UIbuilder)));

                CreateInspectorGUI();//�Լ� ����̶� ������ ������� �ʾ� , ���� �ٽ� ȣ���ؼ� ������Ʈ
            }
            );
            element.Add(objField);

            if (UIbuilder != null)
                UIbuilder.CloneTree(element);//Load form default

        }//VisualTreeAsset ObjectField , Load Default // ==============================================================================���ݴ� �����ϰ� ����ؼ� ���ϰԾ���

        if (element.Q<Button>("button") != null)
            element.Q<Button>("button").clicked += UIBuilderEditor_onClick;

        //Debug.Log(inspector.Children().GetEnumerator().Current);//NotWork
        //�ڵ��Ҵ� ���Ҳ� ����

        {
            element.Add(new Label("CreateInspectorGUI"));//Add a Simple Label

            //VisualElement inspectorFold = inspector.Q("Default_Inspector");//Get a reference to the default inpector foldout control

            //InspectorElement.FillDefaultInspector(inspectorFold, serializedObject, this);//Attach a default inspector to thr foldout
        }

        return element;
    }

    private void UIBuilderEditor_onClick()
    {
        Debug.Log("Clicked");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("InspectorGUI");
    }
}
#endif
 * */