using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class Editor_UIBuilder : Editor
{
    protected VisualTreeAsset UIbuilder;//이 스크립트 이름이 이 클래스일때 커스텀 인스팩더로 설정가능
    protected VisualElement element;

    public override VisualElement CreateInspectorGUI()
    {
        //InspectorXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIBuilder/UIBuilderTemplate.uxml");

        {
            string guID = PlayerPrefs.GetString(this.ToString());

            UIbuilder = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(guID));//GUID으로 저장 / 불러오기

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

                Redraw();//여기 블럭이 함수라서 변경이 적용되지 않아 , 직접 다시 호출해서 업데이트
            }
            );
            element.Add(objField);

            if (UIbuilder != null)
                UIbuilder.CloneTree(element);//Load form default

        }//VisualTreeAsset ObjectField , Load Default 

        //Debug.Log(inspector.Children().GetEnumerator().Current);//NotWork
        //자동할당 못할꺼 같음

        //element.ElementAt // 유용하게 쓸듯?
        //element.hierarchy.

        //target.GetType() //자동 추가후 멤버수만큼 Bind 실행 가능 

        return element;
    }
    public virtual void Redraw()
    {
        CreateInspectorGUI();

        for (int i = 0; i < element.childCount; i++)
        {
            if (element.ElementAt(i).GetType() == typeof(PropertyField))
            {
                element.ElementAt(i).Bind(serializedObject);//serializedObject 와 연결
                                                            //element.Q<PropertyField>().Bind(serializedObject);//처음꺼만 되서
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
    public VisualTreeAsset UIbuilder;//이 스크립트 이름이 이 클래스일때 커스텀 인스팩더로 설정가능
    VisualElement element;

    public override VisualElement CreateInspectorGUI()
    {
        //InspectorXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIBuilder/UIBuilderTemplate.uxml");

        {
            string guID = PlayerPrefs.GetString(this.ToString());

            UIbuilder = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(guID));//GUID으로 저장 / 불러오기

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

                CreateInspectorGUI();//함수 취급이라서 변경이 적용되지 않아 , 직접 다시 호출해서 업데이트
            }
            );
            element.Add(objField);

            if (UIbuilder != null)
                UIbuilder.CloneTree(element);//Load form default

        }//VisualTreeAsset ObjectField , Load Default // ==============================================================================조금더 개선하고 상속해서 편하게쓰기

        if (element.Q<Button>("button") != null)
            element.Q<Button>("button").clicked += UIBuilderEditor_onClick;

        //Debug.Log(inspector.Children().GetEnumerator().Current);//NotWork
        //자동할당 못할꺼 같음

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