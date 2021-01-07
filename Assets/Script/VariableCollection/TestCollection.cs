using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestCollection : MonoBehaviour
{
    public CollectionList collection = new CollectionList();
    // Start is called before the first frame update
    void Start()
    {
        collection.Add<int>(10);
        collection.Add<string>("Testing");
        collection.Add<Color>(Color.white);
        collection.Add<ViewTool>(ViewTool.FPS);


        print(collection.Get<int>(0));
        print(collection.Get<string>(1));

        print(GetTypeFromAssemblies(typeof(int).FullName));

    }

    // Update is called once per frame
    void Update()
    {
    }

    public Type GetTypeFromAssemblies(string TypeName)
    {
        // null ��ȯ ���� Type�� ������ٸ� ����� �״�� ��ȯ.
        var type = Type.GetType(TypeName);
        if (type != null)
            return type;

        // ������Ʈ�� �и��� ���Ե� Ŭ�����ӿ��� �ұ��ϰ� Type�� ã������ �ʴ´ٸ�,
        // �������� ������� ��� Ž�� �ϸ鼭 �� �ȿ� ã���� �ϴ� Type�� �ִ��� �˻�.
        var currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {
            var assembly = System.Reflection.Assembly.Load(assemblyName);
            if (assembly != null)
            {
                // ã�Ҵ� ���!!!
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }
        }

        // �� ã����;;; Ŭ���� �̸��� Ʋ�ȴ���, �ƴϸ� �� �� ���� ���� �����̰���...
        return null;
    }//TypeName = typeof(Type).FullName
}


[CustomEditor (typeof(TestCollection))]
class TestCollectionInspector : Editor
{
    SerializedProperty property;
    TestCollection collection;

    private void OnEnable()
    {
        //base.OnEnable();
        property = serializedObject.FindProperty("collection");
        collection = target as TestCollection;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        /*
        serializedObject.Update();

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(target);
        */
        EditorGUILayout.HelpBox("Test", MessageType.None);
    }
    
}