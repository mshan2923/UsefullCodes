using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[System.Serializable]
public class Test_UIBuilder
{
    public string data;
}

public class UIBuildPropertyDrawer : MonoBehaviour
{
    public List<Test_UIBuilder> Source;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Test_UIBuilder))]
public class TestSourceEditor : PropertyDrawer_UIBuild
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {

        VisualElement container = base.CreatePropertyGUI(property);

        container.Add(new Label("---"));

        return container;
    }
}
#endif
