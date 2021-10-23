using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class InputSystemDTscript : ScriptableObject//�ΰ��ӿ��� ����Ұ� �ϴϱ�... ==> ����ȭ�ؼ� ����,�ҷ������ϰ� 
{
    #region SingletonScriptableObject
    const string SettingFileDirectory = "Assets/Resources";
    const string SettingFilePath = "Assets/Resources/InputSystemDataTable.asset";
    static InputSystemDTscript _instance;

    public static InputSystemDTscript Instance
    {
        get 
        {
            if (_instance != null)
            {
                return _instance;
            }
            _instance = Resources.Load<InputSystemDTscript>(SettingFilePath);

#if UNITY_EDITOR
            if (_instance == null)
            {
                if(!AssetDatabase.IsValidFolder(SettingFileDirectory))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                _instance = AssetDatabase.LoadAssetAtPath<InputSystemDTscript>(SettingFilePath);

                if (_instance == null)
                {
                    _instance = CreateInstance<InputSystemDTscript>();
                    //_instance = System.Activator.CreateInstance<InputSystemDTscript>(); //== GameObject.Instantiate
                    AssetDatabase.CreateAsset(_instance, SettingFilePath);
                }
            }
#endif
            return _instance;
        }
    }

    [MenuItem("Asset/SingletonDataTable")]
    public static void OpenInspector()
    {
        Selection.activeObject = Instance;
    }
    #endregion

    public KeyCode Temp;
    //Map<�Է� �̺�Ʈ �̸� , List<KeyCode>> �Է� �� �̺�Ʈ
    //Map<�Է� �̺�Ʈ �̸� , List<KeyCode>> �Է� �̺�Ʈ
}

[CustomEditor(typeof(InputSystemDTscript))]
public class InputSystemDTEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.TextField("");
        Event e = Event.current;
        GUILayout.Label(" Input : " + e.keyCode);
        
    }
}