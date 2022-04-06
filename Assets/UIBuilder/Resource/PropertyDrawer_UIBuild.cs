using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class PropertyDrawer_UIBuild : PropertyDrawer
{
    protected VisualElement container;
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        //var RootAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIBuilder/UIBuilderTemplate.uxml");

        //var root = RootAsset.CloneTree();
        //root.BindProperty(property);

        if (PlayerPrefs.HasKey(property.type))
        {            
             var RootAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
                (AssetDatabase.GUIDToAssetPath(PlayerPrefs.GetString(property.type)));

            container = RootAsset.CloneTree();
            container.Bind(property.serializedObject);
        }else
        {
            container = new VisualElement();
        }

        return container;
    }

}
[CustomEditor(typeof(MonoBehaviour), true)]
public class ImguiToToolkitWrapper : UnityEditor.Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        var prop = serializedObject.GetIterator();
        if (prop.NextVisible(true))
        {
            do
            {
                var field = new PropertyField(prop);

                if (prop.name == "m_Script")
                {
                    field.SetEnabled(false);
                }

                root.Add(field);
            }
            while (prop.NextVisible(false));
        }

        return root;
    }
    //Source : https://forum.unity.com/threads/property-drawers.595369/page-2#post-7644859
}//Change To Enable UIbuilder in PropertyDrawer
