using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestCollection : MonoBehaviour
{
    // CollectionList MutiCollection = new CollectionList();
    [Space(50)]
    public VarCollection Collection = new VarCollection();

    [Space(50)]
    public VarCollection[] collections = new VarCollection[3];

    // Start is called before the first frame update
    void Start()
    {
        /*
        MutiCollection.Add<int>(10);
        MutiCollection.Add<string>("Testing");
        MutiCollection.Add<ViewTool>(ViewTool.FPS);
        MutiCollection.Add<List<int>>(new List<int>() { 0, 1});
        MutiCollection.Add<int[]>(new int[] { 2, 3 });
        */

        int[] Test = new int[] { 2, 3 };
        Debug.Log("///" + JsonUtility.ToJson(Test));

        Collection.Set<int>(100, true);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Type GetTypeFromAssemblies(string TypeName)
    {
        // null 반환 없이 Type이 얻어진다면 얻어진 그대로 반환.
        var type = Type.GetType(TypeName);
        if (type != null)
            return type;

        // 프로젝트에 분명히 포함된 클래스임에도 불구하고 Type이 찾아지지 않는다면,
        // 실행중인 어셈블리를 모두 탐색 하면서 그 안에 찾고자 하는 Type이 있는지 검사.
        var currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {
            var assembly = System.Reflection.Assembly.Load(assemblyName);
            if (assembly != null)
            {
                // 찾았다 요놈!!!
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }
        }

        // 못 찾았음;;; 클래스 이름이 틀렸던가, 아니면 알 수 없는 문제 때문이겠지...
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