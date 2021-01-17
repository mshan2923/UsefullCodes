using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestCollection : MonoBehaviour
{
    public CollectionList MutiCollection = new CollectionList();
    [Space(50)]
    public VarCollection Collection = new VarCollection();

    // Start is called before the first frame update
    void Start()
    {
        MutiCollection.Add<int>(10);
        MutiCollection.Add<string>("Testing");
        MutiCollection.Add<ViewTool>(ViewTool.FPS);
        MutiCollection.Add<List<int>>(new List<int>() { 0, 1});
        MutiCollection.Add<int[]>(new int[] { 2, 3 });


        int[] Test = new int[] { 2, 3 };
        Debug.Log("///" + JsonUtility.ToJson(Test));

        Collection.Set<int>(100, true);
    }

    // Update is called once per frame
    void Update()
    {
        print(Collection.ForceGet()[1]);
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
        //property = serializedObject.FindProperty("collection");
        //collection = target as TestCollection;
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
        EditorGUILayout.HelpBox("Editor class Test", MessageType.None);
    }
    
}