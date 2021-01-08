using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ReOrderableTemplate : MonoBehaviour
{
    [NonReorderable]
    public List<string> Vaule = new List<string>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[CustomEditor(typeof(ReOrderableTemplate))]
public class ReOrderableEditor : Editor
{
    UnityEditorInternal.ReorderableList list;
    int SelectIndex = 0;

    private void OnEnable()
    {
        var prop = serializedObject.FindProperty("Vaule");

        list = new UnityEditorInternal.ReorderableList(serializedObject, prop)
        {
            drawHeaderCallback = DrawHeader,
            drawElementCallback = DrawListItems
        };

        list.onSelectCallback = list =>
            {
                SelectIndex = list.index;
            };
        list.onReorderCallback = list =>
            {
                Debug.Log("Select " + SelectIndex + " / ReOrder " + list.index);
            };
        list.onRemoveCallback = list =>
            {
                Debug.Log("Remove " + SelectIndex);
                prop.DeleteArrayElementAtIndex(SelectIndex);
            };
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
    void DrawListItems(Rect rect, int index, bool isActive, bool isfocused)
    {
        var element = serializedObject.FindProperty("Vaule").GetArrayElementAtIndex(index);
        rect.height -= 4;
        rect.y += 2;
        EditorGUI.PropertyField(rect, element);
        //EditorGUILayout.PropertyField(element);
    }
    void DrawHeader(Rect rect)
    {
        string name = "Vaule";
        EditorGUI.LabelField(rect, name);
    }
}
