using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static VariableCollection;

[System.Serializable]
public class VariableCollection
{
    public static Type ConvertType(string TypeName)
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
    public static SerializedPropertyType ConvertTypeEnum(string TypeName)
    {
        SerializedPropertyType TypeEnum = SerializedPropertyType.Generic;

        if(typeof(int).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.Integer;
        if (typeof(bool).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.Boolean;
        if (typeof(float).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.Float;
        if (typeof(string).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.String;
        if (typeof(Color).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.Color;
        if (typeof(GameObject).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.ObjectReference;//아닐지도?
        if (typeof(LayerMask).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.LayerMask;
        if (typeof(Enum).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.Enum;
        if (typeof(Vector2).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.Vector2;
        if (typeof(Vector3).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.Vector3;
        if (typeof(Vector4).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.Vector4;//10
        if (typeof(Rect).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.Rect;
        if (typeof(Array).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.ArraySize;//될려나??
        //if (typeof(Character).Name.Equals(TypeName))
        //    TypeEnum = SerializedPropertyType.Character;//???
        if (typeof(AnimationCurve).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.AnimationCurve;
        if (typeof(Bounds).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.Bounds;
        if (typeof(Gradient).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.Gradient;
        if (typeof(Quaternion).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.Quaternion;
        //if (typeof(ExposedReference).Name.Equals(TypeName))
        //    TypeEnum = SerializedPropertyType.ExposedReference;
        if (typeof(Buffer).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.FixedBufferSize;//???
        if (typeof(Vector2Int).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.Vector2Int;//20
        if (typeof(Vector3Int).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.Vector3Int;
        if (typeof(RectInt).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.RectInt;
        if (typeof(BoundsInt).Name.Equals(TypeName))
            TypeEnum = SerializedPropertyType.BoundsInt;
        //if (typeof().Name.Equals(TypeName))
        //    TypeEnum = SerializedPropertyType.ManagedReference;

        return TypeEnum;
    }

    public static T UnRapping<T>(string vaule)
    {
        return JsonUtility.FromJson<Wrap<T>>(vaule).Get;
    }
    public static string Rapping<T>(T vaule)
    {
        Wrap<T> wrap = new Wrap<T>(vaule);
        return JsonUtility.ToJson(wrap);
    }

}

[System.Serializable]
public class Wrap<T>
{
    [SerializeField]
    T data;

    public Wrap(T data)
    {
        this.data = data;
    }

    public T Get { get => data; }
}//이렇게 감싸주면 배열,리스트 등을 직렬화가능 
[System.Serializable]
public class CollectionList
{
    [SerializeField]
    public List<string> Data;
    [SerializeField]
    public List<string> DataType;//String 으로 바꿔야 ProperyDrawer 에서 사용 가능

    public CollectionList()
    {
        Data = new List<string>();
        DataType = new List<string>();
    }
    public bool Set<T>(int index, T vaule)
    {
        Wrap<T> wrap = new Wrap<T>(vaule);
        string data = JsonUtility.ToJson(wrap);

        if (!string.IsNullOrEmpty(data))
        {
            if (index < Data.Count && index >= 0 && Data.Count != 0)
            {
                Data[index] = data;
                DataType[index] = typeof(T).FullName;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }
    public void Add<T>(T vaule)
    {
        Wrap<T> wrap = new Wrap<T>(vaule);
        string data = JsonUtility.ToJson(wrap);
        Data.Add(data);
        DataType.Add(typeof(T).FullName);
    }
    public void ForceAdd(string TypeName, string Vauletext)
    {
        Data.Add(Vauletext);
        DataType.Add(TypeName);
    }
    public bool ForceSet(string TypeName, int index, string VauleText)
    {
        if (index < Data.Count && index >= 0 && Data.Count != 0)
        {
            Data[index] = VauleText;
            DataType[index] = TypeName;
            return true;
        }
        else if(index == Data.Count)
        {
            ForceAdd(TypeName, VauleText);
            return true;
        }else
        {
            return false;
        }
    }
    public T Get<T>(int index)
    {
        if (index < Data.Count && index >= 0 && Data.Count != 0)
        {
            T vaule = JsonUtility.FromJson<Wrap<T>>(Data[index]).Get;
            return vaule;
        }
        else
        {
            return default;
        }
    }
    public List<T> Gets<T>()
    {
        var indexs = GetEqualTypes<T>();
        List<T> Vaules = new List<T>();
        for (int i = 0; i < indexs.Count; i++)
        {
            Vaules.Add(Get<T>(i));
        }

        return Vaules;
    }
    public Type GetType(int index)
    {
        return ConvertType(DataType[index]);
    }
    public int Find<T>(T vaule)
    {
        List<int> indexs = new List<int>();
        for (int i = 0; i < DataType.Count; i++)
        {
            if (ConvertType(DataType[i]) == typeof(T))
            {
                //indexs.Add(i);
                T temp = Get<T>(i);
                if (vaule.Equals(temp))
                {
                    return i;
                }
            }
        }

        return -1;
    }
    public List<int> GetEqualTypes<T>()
    {
        List<int> indexs = new List<int>();
        for (int i = 0; i < DataType.Count; i++)
        {
            if (ConvertType(DataType[i]) == typeof(T))
            {
                indexs.Add(i);
            }
        }
        return indexs;
    }

    public Type ConvertType(string TypeName)
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

/*
[CustomEditor(typeof(CollectionList))]
public class CollectionListEditor : Editor
{
    SerializedProperty DataProperty;
    SerializedProperty TypeProperty;

    CollectionList collectionList;

    public void OnEnable()
    {
        collectionList = new CollectionList();
        GetCollectionList();
    }
    public void GetCollectionList()
    {
        //Debug.Log(serializedObject.FindProperty("collection").FindPropertyRelative("Data").GetArrayElementAtIndex(0).stringValue);
        
        DataProperty = serializedObject.FindProperty("collection").FindPropertyRelative("Data");
        TypeProperty = serializedObject.FindProperty("collection").FindPropertyRelative("DataType");

        //collectionList = (CollectionList)target; // Not Work Fuuuuuuuuuuuck
        if(DataProperty != null)
        {
            for(int i = 0; i < DataProperty.arraySize; i++)
            {
                collectionList.ForceSet(TypeProperty.GetArrayElementAtIndex(i).stringValue, i, DataProperty.GetArrayElementAtIndex(i).stringValue);
            }
        }//Manual Set collectionList

    }
    public override void OnInspectorGUI()
    {
        GetCollectionList();

        if (collectionList != null)
            Debug.Log("collectionList : " + collectionList.Data.Count);
        else
            Debug.Log("collectionList : Null");

        //base.OnInspectorGUI();
        EditorGUILayout.PropertyField(DataProperty, GUIContent.none);
        EditorGUILayout.PropertyField(TypeProperty, GUIContent.none);

        for(int i = 0; i < collectionList.Data.Count; i++)
        {
            DataField(DataProperty, TypeProperty, collectionList, i);
        }
        //collectionList 변경사항 적용

    }
    public void DataField(SerializedProperty DataProp , SerializedProperty TypeProp, CollectionList collectionList, int index)
    {
        SerializedProperty DataPropSlot = DataProp.GetArrayElementAtIndex(index);
        SerializedProperty TypePropSlot = TypeProp.GetArrayElementAtIndex(index);
        string TypeName = TypePropSlot.stringValue;

        //ConvertType(TypePropSlot.stringValue)//Type형 인데...
        //SerializedProperty temp = serializedObject.FindProperty(ConvertType(TypePropSlot.stringValue).FullName);
        //SerializedProperty temp = 

        //Debug.Log(TypePropSlot.propertyType + " + " + temp.propertyType);
        switch(ConvertType(TypePropSlot.stringValue).GetProperties()[0].PropertyType)
        {

        }


        Debug.Log(TypePropSlot.propertyType);
        switch (TypePropSlot.propertyType)
        {
            case SerializedPropertyType.Generic:
                EditorGUILayout.TextArea(DataPropSlot.stringValue + " + Genernic");
                break;
            case SerializedPropertyType.Integer:
                {
                    //LProp.intValue = EditorGUILayout.IntField(LProp.intValue);
                    //collectionList.Set<int>(index, LProp.intValue);

                    //LProp.stringValue = VariableCollection.Rapping(EditorGUILayout.IntField(VariableCollection.UnRapping<int>(LProp.stringValue)));
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.IntField(UnRapping<int>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Boolean:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.Toggle(UnRapping<bool>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Float:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.FloatField(UnRapping<float>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.String:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.TextField(UnRapping<string>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Color:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.ColorField(UnRapping<Color>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.ObjectReference:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.ObjectField(UnRapping<UnityEngine.Object>(DataPropSlot.stringValue), typeof(GameObject), true));
                    break;
                }
            case SerializedPropertyType.LayerMask:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.LayerField(UnRapping<LayerMask>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Enum:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.EnumFlagsField(UnRapping<Enum>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Vector2:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.Vector2Field("", UnRapping<Vector2>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Vector3:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.Vector3Field("", UnRapping<Vector3>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Vector4:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.Vector4Field("", UnRapping<Vector4>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Rect:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.RectField(UnRapping<Rect>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.ArraySize:
                {
                    for(int i = 0; i < DataPropSlot.arraySize; i++)
                    {
                        DataField(DataPropSlot, TypePropSlot, collectionList, i);
                    }
                    break;
                }//될지 모르겠음 ---------
            case SerializedPropertyType.Character:
                {
                    //??먼지 모르겠음
                    EditorGUILayout.TextArea(DataPropSlot.stringValue + " + Need Fix");
                    break;
                }//Not Support
            case SerializedPropertyType.AnimationCurve:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.CurveField(UnRapping<AnimationCurve>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Bounds:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.BoundsField(UnRapping<Bounds>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Gradient:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.GradientField(UnRapping<Gradient>(DataPropSlot.stringValue)));
                    break;
                }

            #region Not Support
            case SerializedPropertyType.Quaternion:                
            case SerializedPropertyType.ExposedReference:                
            case SerializedPropertyType.FixedBufferSize:               
            case SerializedPropertyType.Vector2Int:                
            case SerializedPropertyType.Vector3Int:                
            case SerializedPropertyType.RectInt:                
            case SerializedPropertyType.BoundsInt:                
            case SerializedPropertyType.ManagedReference:
                {
                    EditorGUILayout.TextArea(DataPropSlot.stringValue + " + Not Support Type");
                    break;
                }
            #endregion

            default:
                {
                    EditorGUILayout.TextArea(DataPropSlot.stringValue + " + Unknown Type");
                    break;
                }
        }//https://dev-youngil.tistory.com/1 참고

        collectionList.ForceSet(TypeName, index, DataPropSlot.stringValue);
    }
}
*/

[CustomPropertyDrawer(typeof(CollectionList))]
public class CollectionListProperty : PropertyDrawer
{
    float DrawHeight = 0;
    public override bool CanCacheInspectorGUI(SerializedProperty property)
    {
        return base.CanCacheInspectorGUI(property);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float temp = base.GetPropertyHeight(property, label);
        temp += GetArrayHeight(property, "Data", 25, 25);
        temp += GetArrayHeight(property, "DataType", 25, 25);

        return temp;
    }
    public float GetArrayHeight(SerializedProperty property , string name = "Data", float indexSize = 25 , float Default = 50)
    {
        var Local = property.FindPropertyRelative(name);
        int size = Mathf.Clamp(Local.arraySize, 1, Local.arraySize) + 1;//최소 2칸 , 갯수가 1개이상 - 갯수 + 1
        return Local.isExpanded ? size * indexSize + Default : Default;
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        EditorGUI.BeginProperty(position, label, property);

        DrawHeight = position.y;
        var DataRect = new Rect(position.x, DrawHeight, position.width, position.height);
        DrawHeight += GetArrayHeight(property, "Data", 25, 25);
        var TypeRect = new Rect(position.x, DrawHeight, position.width, position.height);

        EditorGUI.PropertyField(DataRect, property.FindPropertyRelative("Data"), GUIContent.none);
        EditorGUI.PropertyField(TypeRect, property.FindPropertyRelative("DataType"), GUIContent.none);

        Debug.Log("Data : " + " / " + property.FindPropertyRelative("Data").displayName);
        Debug.Log("Type : " + " / " + property.FindPropertyRelative("DataType").displayName);

        EditorGUI.EndProperty();

        for(int i = 0; i < property.FindPropertyRelative("Data").arraySize; i++)
        {
            CreateDataField(property.FindPropertyRelative("DataType"), property.FindPropertyRelative("Data"), i);
        }
        //GetData(position, property);//useLess
    }

    public void GetData(Rect position, SerializedProperty property)
    {
        int Count = property.FindPropertyRelative("Data").arraySize;
        for(int i = 0; i < Count; i++)
        {
            string text = property.FindPropertyRelative("Data").GetArrayElementAtIndex(i).stringValue;
            string type = property.FindPropertyRelative("DataType").GetArrayElementAtIndex(i).stringValue;
            
            Debug.Log(VariableCollection.ConvertType(type) + " : " + text);
        }
        //property.FindPropertyRelative("Data").GetArrayElementAtIndex(0);
    }
    public void CreateDataField(SerializedProperty TypeProp, SerializedProperty DataProp, int index, string LabelText = "Index ")
    {
        SerializedProperty DataPropSlot = DataProp.GetArrayElementAtIndex(index);
        SerializedProperty TypePropSlot = TypeProp.GetArrayElementAtIndex(index);
        Type LType = ConvertType(TypePropSlot.stringValue);

        var TypeEnum = ConvertTypeEnum(LType.Name);
        Debug.Log("Type : " + LType.Name + " | " + TypeEnum.ToString());
        string title = LabelText + index;

        //TypePropSlot.propertyType
        switch (TypeEnum)
        {
            case SerializedPropertyType.Generic:
                EditorGUILayout.TextArea(DataPropSlot.stringValue + " + Genernic");
                break;
            case SerializedPropertyType.Integer:
                {
                    //LProp.intValue = EditorGUILayout.IntField(LProp.intValue);
                    //collectionList.Set<int>(index, LProp.intValue);

                    //LProp.stringValue = VariableCollection.Rapping(EditorGUILayout.IntField(VariableCollection.UnRapping<int>(LProp.stringValue)));
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.IntField(title, UnRapping<int>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Boolean:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.Toggle(title, UnRapping<bool>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Float:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.FloatField(title, UnRapping<float>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.String:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.TextField(title, UnRapping<string>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Color:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.ColorField(title, UnRapping<Color>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.ObjectReference:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.ObjectField(title, UnRapping<UnityEngine.Object>(DataPropSlot.stringValue), typeof(GameObject), true));
                    break;
                }
            case SerializedPropertyType.LayerMask:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.LayerField(title, UnRapping<LayerMask>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Enum:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.EnumFlagsField(title, UnRapping<Enum>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Vector2:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.Vector2Field(title, UnRapping<Vector2>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Vector3:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.Vector3Field(title, UnRapping<Vector3>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Vector4:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.Vector4Field(title, UnRapping<Vector4>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Rect:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.RectField(title, UnRapping<Rect>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.ArraySize:
                {
                    for (int i = 0; i < DataPropSlot.arraySize; i++)
                    {
                        //DataField(DataPropSlot, TypePropSlot, collectionList, i);
                    }
                    break;
                }//될지 모르겠음 ---------
            case SerializedPropertyType.Character:
                {
                    //??먼지 모르겠음
                    EditorGUILayout.TextArea(DataPropSlot.stringValue + " + Need Fix");
                    break;
                }//Not Support
            case SerializedPropertyType.AnimationCurve:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.CurveField(title, UnRapping<AnimationCurve>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Bounds:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.BoundsField(title, UnRapping<Bounds>(DataPropSlot.stringValue)));
                    break;
                }
            case SerializedPropertyType.Gradient:
                {
                    DataPropSlot.stringValue = Rapping(EditorGUILayout.GradientField(title, UnRapping<Gradient>(DataPropSlot.stringValue)));
                    break;
                }

            #region Not Support
            case SerializedPropertyType.Quaternion:
            case SerializedPropertyType.ExposedReference:
            case SerializedPropertyType.FixedBufferSize:
            case SerializedPropertyType.Vector2Int:
            case SerializedPropertyType.Vector3Int:
            case SerializedPropertyType.RectInt:
            case SerializedPropertyType.BoundsInt:
            case SerializedPropertyType.ManagedReference:
                {
                    EditorGUILayout.TextArea(DataPropSlot.stringValue + " + Not Support Type");
                    break;
                }
            #endregion

            default:
                {
                    EditorGUILayout.TextArea(DataPropSlot.stringValue + " + Unknown Type");
                    break;
                }
        }
    }
}//PropertyDrawer