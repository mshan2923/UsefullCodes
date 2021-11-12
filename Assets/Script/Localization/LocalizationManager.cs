using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    #region Sington
    public const string ManagerFileDirectory = "Assets/Resources";
    const string ManagerFileName = "LocalizationManager";

    static LocalizationManager instance = null;
    public static LocalizationManager Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            string filePath = ManagerFileDirectory + "/" + ManagerFileName + ".prefab";
            instance = GameObject.FindObjectOfType<LocalizationManager>();

#if UNITY_EDITOR
            if (instance == null)
            {
                GameObject obj = null;

                if (!AssetDatabase.IsValidFolder(ManagerFileDirectory))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }

                if (AssetDatabase.LoadAssetAtPath<GameObject>(filePath) != null)
                {
                    obj = PrefabUtility.LoadPrefabContents(filePath);
                    instance = obj.GetComponent<LocalizationManager>();
                }

                if (instance == null)
                {
                    obj = System.Activator.CreateInstance<GameObject>();
                    instance = obj.AddComponent<LocalizationManager>();
                    obj.name = "LocalizationManager";
                    PrefabUtility.SaveAsPrefabAsset(obj, filePath);
                }
            }
#endif
            return instance;
        }
    }
    [MenuItem("Asset/LocalizationManager")]
    public static void OpenInspector()
    {
        Selection.activeObject = Instance;
    }

    public void SavePrefab()
    {
        PrefabUtility.SaveAsPrefabAsset(gameObject, ManagerFileDirectory + "/" + ManagerFileName + ".prefab");
        instance = GetComponent<LocalizationManager>();
    }
    public void LoadPrefab()
    {
        instance = PrefabUtility.LoadPrefabContents(ManagerFileDirectory + "/" + ManagerFileName + ".prefab").GetComponent<LocalizationManager>();
    }

    #endregion

    public Map<string, string> LocalizationData = new Map<string, string>();

    //[AttributeLocalizedText]
    //public string TestAttribute = "";//Attribute  유효한 키값인지 확인

    public void SaveCSVFile()
    {
        if (!AssetDatabase.IsValidFolder(ManagerFileDirectory + "/Localization"))
        {
            AssetDatabase.CreateFolder(ManagerFileDirectory, "Localization");
        }

        string Lpath = EditorUtility.SaveFilePanel("Select Save Localization data Path", Application.dataPath + "/Resources/Localization", "", "csv");

        if (string.IsNullOrWhiteSpace(Lpath))
        {

        }else
        {
            string columns = "index,key,vaule";//FirstLine

            Stream fileStream = new FileStream(Lpath, FileMode.Create, FileAccess.Write);
            StreamWriter outStream = new StreamWriter(fileStream, System.Text.Encoding.UTF8);

            outStream.WriteLine(columns);

            for (int i = 0; i < LocalizationData.Count; i++)
            {
                columns = i + "," + LocalizationData.GetKey(i) + "," + LocalizationData.GetVaule(i);
                outStream.WriteLine(columns);
            }
            outStream.Close();
        }
    }
    public void LoadCSVFile()
    {
        if (!AssetDatabase.IsValidFolder(ManagerFileDirectory + "/Localization"))
        {
            AssetDatabase.CreateFolder(ManagerFileDirectory, "Localization");
        }

        string filePath = EditorUtility.OpenFilePanel("Select Localization data file", Application.dataPath + "/Resources/Localization", "csv");

        if (! string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            string[] stringBigList = dataAsJson.Split('\n');

            for (int i = 1; i < stringBigList.Length; i++)
            {
                string[] slotList = stringBigList[i].Split(',');

                if (!string.IsNullOrWhiteSpace(slotList[0]))
                    LocalizationData.Add(slotList[1], slotList[2]);
            }
        }
    }
    public void LoadFile(string FileName)
    {
        string filePath = Application.dataPath + "/Resources/Localization/" + FileName + ".csv";

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            string[] stringBigList = dataAsJson.Split('\n');

            for (int i = 1; i < stringBigList.Length; i++)
            {
                string[] slotList = stringBigList[i].Split(',');

                if (!string.IsNullOrWhiteSpace(slotList[0]))
                    LocalizationData.Add(slotList[1], slotList[2]);
            }
        }
    }
    public string GetLocalizedText(string key)
    {
        return LocalizationData.Get().Find(t => t.Key == key).Vaule;
    }
    public bool ExistLocalizedText(string key)
    {
        return LocalizationData.Get().Exists(t => t.Key == key);
    }
    public void ReloadTexts(string FileName)
    {
        LoadFile(FileName);
        var textList = FindObjectsOfType<LocalizedText>();

        for(int i = 0; i < textList.Length; i++)
        {
            textList[i].ReloadText();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LocalizationManager))]
public class LocalizationManagerEditor : Editor
{
    LocalizationManager onwer;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        {
            onwer = target as LocalizationManager;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save CSV"))
            {
                onwer.SaveCSVFile();
            }
            if (GUILayout.Button("Load CSV"))
            {
                onwer.LoadCSVFile();
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("LoadFile - test"))
            {
                onwer.LoadFile("TestCSV");
            }
        }

        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save Prefab"))
            {
                onwer.SavePrefab();
            }

            if (GUILayout.Button("Load Prefab"))
            {
                onwer.LoadPrefab();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}

public class AttributeLocalizedText : PropertyAttribute
{

}
[CustomPropertyDrawer(typeof(AttributeLocalizedText))]
public class AttributeLocalizedTextEditor : PropertyDrawer
{
    Rect DrawrRect;
    float CheckSize = 20;
    string Localizaed = "";

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DrawrRect = new Rect(position.x, position.y, position.width - CheckSize, position.height);

        {
            Localizaed = LocalizationManager.Instance.GetLocalizedText(property.stringValue);
        }

        if (string.IsNullOrEmpty(Localizaed))
        {
            if (LocalizationManager.Instance.ExistLocalizedText(property.stringValue))
            {
                label.tooltip = "Localized Text is Empty";
            }
            else
            {
                label.tooltip = "Localized Text is No Exisit";
            }
        }
        else
        {
            label.tooltip = "Localized Text : " + Localizaed;
        }

        EditorGUI.PropertyField(DrawrRect, property, label);

        DrawrRect = new Rect(position.x + position.width - CheckSize * 0.75f, position.y, CheckSize, position.height);
        EditorGUI.Toggle(DrawrRect, !string.IsNullOrEmpty(Localizaed));
    }
}
#endif
