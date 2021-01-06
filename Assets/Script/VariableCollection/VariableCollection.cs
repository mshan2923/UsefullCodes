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
        //if (typeof(Buffer).Name.Equals(TypeName))
        //    TypeEnum = SerializedPropertyType.FixedBufferSize;//???
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
    public static string ConvertTypeName(SerializedPropertyType TypeEnum)
    {
        switch (TypeEnum)
        {
            case SerializedPropertyType.Generic:
                return null;
            case SerializedPropertyType.Integer:
                return typeof(int).FullName;
            case SerializedPropertyType.Boolean:
                return typeof(bool).FullName;
            case SerializedPropertyType.Float:
                return typeof(float).FullName;
            case SerializedPropertyType.String:
                return typeof(string).FullName;
            case SerializedPropertyType.Color:
                return typeof(Color).FullName;
            case SerializedPropertyType.ObjectReference:
                return typeof(GameObject).FullName;
            case SerializedPropertyType.LayerMask:
                return typeof(LayerMask).FullName;
            case SerializedPropertyType.Enum:
                return typeof(Enum).FullName;
            case SerializedPropertyType.Vector2:
                return typeof(Vector2).FullName;
            case SerializedPropertyType.Vector3:
                return typeof(Vector3).FullName;
            case SerializedPropertyType.Vector4:
                return typeof(Vector4).FullName;
            case SerializedPropertyType.Rect:
                return typeof(Rect).FullName;
            case SerializedPropertyType.ArraySize:
                //return typeof(Array).FullName;
                return null;
            case SerializedPropertyType.Character:
                return null;
            case SerializedPropertyType.AnimationCurve:
                return typeof(AnimationCurve).FullName;
            case SerializedPropertyType.Bounds:
                return typeof(Bounds).FullName;
            case SerializedPropertyType.Gradient:
                return typeof(Gradient).FullName;
            case SerializedPropertyType.Quaternion:
                return typeof(Quaternion).FullName;
            case SerializedPropertyType.ExposedReference:
            case SerializedPropertyType.FixedBufferSize:
                return null;
            case SerializedPropertyType.Vector2Int:
                return typeof(Vector2Int).FullName;
            case SerializedPropertyType.Vector3Int:
                return typeof(Vector3Int).FullName;
            case SerializedPropertyType.RectInt:
                return typeof(RectInt).FullName;
            case SerializedPropertyType.BoundsInt:
                return typeof(BoundsInt).FullName;
            case SerializedPropertyType.ManagedReference:
            default:
                return null;
        }
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
    public static string Rapping(SerializedPropertyType TypeEnum, string vaule = "")
    {
        switch (TypeEnum)
        {
            case SerializedPropertyType.Generic:
                return vaule;
            case SerializedPropertyType.Integer:
                {
                    if(string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<int>(0));
                    }else
                    {
                        return JsonUtility.ToJson(new Wrap<int>(JsonUtility.FromJson<int>(vaule)));
                    }
                }
            case SerializedPropertyType.Boolean:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<bool>(false));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<bool>(JsonUtility.FromJson<bool>(vaule)));
                    }
                }
            case SerializedPropertyType.Float:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<float>(0));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<float>(JsonUtility.FromJson<float>(vaule)));
                    }
                }
            case SerializedPropertyType.String:
                return JsonUtility.ToJson(new Wrap<string>(vaule));
            case SerializedPropertyType.Color:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<Color>(Color.black));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<Color>(JsonUtility.FromJson<Color>(vaule)));
                    }
                }
            case SerializedPropertyType.ObjectReference:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<GameObject>(null));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<GameObject>(JsonUtility.FromJson<GameObject>(vaule)));
                    }
                }
            case SerializedPropertyType.LayerMask:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<LayerMask>(new LayerMask()));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<LayerMask>(JsonUtility.FromJson<LayerMask>(vaule)));
                    }
                }
            case SerializedPropertyType.Enum:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<Enum>(null));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<Enum>(JsonUtility.FromJson<Enum>(vaule)));
                    }
                }
            case SerializedPropertyType.Vector2:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<Vector2>(Vector2.zero));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<Vector2>(JsonUtility.FromJson<Vector2>(vaule)));
                    }
                }
            case SerializedPropertyType.Vector3:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<Vector3>(Vector3.zero));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<Vector3>(JsonUtility.FromJson<Vector3>(vaule)));
                    }
                }
            case SerializedPropertyType.Vector4:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<Vector4>(Vector4.zero));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<Vector4>(JsonUtility.FromJson<Vector4>(vaule)));
                    }
                }
            case SerializedPropertyType.Rect:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<Rect>(Rect.zero));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<Rect>(JsonUtility.FromJson<Rect>(vaule)));
                    }
                }
            case SerializedPropertyType.ArraySize:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<Array>(null));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<Array>(JsonUtility.FromJson<Array>(vaule)));
                    }
                }//...?
            case SerializedPropertyType.Character:
                {
                    return null;
                }//null
            case SerializedPropertyType.AnimationCurve:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<AnimationCurve>(null));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<AnimationCurve>(JsonUtility.FromJson<AnimationCurve>(vaule)));
                    }
                }
            case SerializedPropertyType.Bounds:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<Bounds>(new Bounds()));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<Bounds>(JsonUtility.FromJson<Bounds>(vaule)));
                    }
                }
            case SerializedPropertyType.Gradient:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<Gradient>(null));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<Gradient>(JsonUtility.FromJson<Gradient>(vaule)));
                    }
                }
            case SerializedPropertyType.Quaternion:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<Quaternion>(Quaternion.identity));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<Quaternion>(JsonUtility.FromJson<Quaternion>(vaule)));
                    }
                }
            case SerializedPropertyType.ExposedReference:
            case SerializedPropertyType.FixedBufferSize:
                return null;
            case SerializedPropertyType.Vector2Int:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<Vector2Int>(Vector2Int.zero));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<Vector2Int>(JsonUtility.FromJson<Vector2Int>(vaule)));
                    }
                }
            case SerializedPropertyType.Vector3Int:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<Vector3Int>(Vector3Int.zero));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<Vector3Int>(JsonUtility.FromJson<Vector3Int>(vaule)));
                    }
                }
            case SerializedPropertyType.RectInt:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<RectInt>(new RectInt()));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<RectInt>(JsonUtility.FromJson<RectInt>(vaule)));
                    }
                }
            case SerializedPropertyType.BoundsInt:
                {
                    if (string.IsNullOrEmpty(vaule))
                    {
                        return JsonUtility.ToJson(new Wrap<BoundsInt>(new BoundsInt()));
                    }
                    else
                    {
                        return JsonUtility.ToJson(new Wrap<BoundsInt>(JsonUtility.FromJson<BoundsInt>(vaule)));
                    }
                }
            case SerializedPropertyType.ManagedReference:
            default:
                return null;
        }
    }//vaule = JsonUtility.ToJson(new wrap<T>(Vaule))

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
*///Editor - 되긴 되는데 target이 CollectionList 형전환이 안되서

[CustomPropertyDrawer(typeof(CollectionList))]
public class CollectionListProperty : PropertyDrawer
{
    SerializedProperty DataProp;
    SerializedProperty TypeProp;

    float DrawHeight = 0;

    string TempTypeEnumKey = "TempTypeEnumKey";
    string TempIndexKey = "TempIndexKey";
    string TempVauleKey = "TempVauleKey";
    int TempTypeEnum = -1;
    int TempIndex = -1;
    string TempVaule = "";

    bool DataInputFold = false;
    bool AddRemoveFold = false;
    public override bool CanCacheInspectorGUI(SerializedProperty property)
    {
        return base.CanCacheInspectorGUI(property);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float temp = base.GetPropertyHeight(property, label);
        //temp += GetArrayHeight(property, "Data", 25, 25);
        //temp += GetArrayHeight(property, "DataType", 25, 25);

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
        {
            //EditorGUI.BeginProperty(position, label, property);

            //DrawHeight = position.y;
            //var DataRect = new Rect(position.x, DrawHeight, position.width, position.height);
            //DrawHeight += GetArrayHeight(property, "Data", 25, 25);
            //var TypeRect = new Rect(position.x, DrawHeight, position.width, position.height);

            //EditorGUI.PropertyField(DataRect, property.FindPropertyRelative("Data"), GUIContent.none);
            //EditorGUI.PropertyField(TypeRect, property.FindPropertyRelative("DataType"), GUIContent.none);
        }//EditorGUI
        DataProp = property.FindPropertyRelative("Data");
        TypeProp = property.FindPropertyRelative("DataType");

        TempTypeEnum = PlayerPrefs.GetInt(TempTypeEnumKey);
        TempIndex = PlayerPrefs.GetInt(TempIndexKey);
        TempVaule = PlayerPrefs.GetString(TempVauleKey);

        EditorGUILayout.PropertyField(DataProp, GUIContent.none);
        EditorGUILayout.PropertyField(TypeProp, GUIContent.none);

        //EditorGUI.EndProperty();

        //Data 인덱스 교체시 DataType 도
        //Data , DataType 으로 추가, 삭제시 문제

        {
            DataInputFold = EditorGUILayout.BeginFoldoutHeaderGroup(DataInputFold, ("Data Input Field" + "  /  Length : " + DataProp.arraySize));
            if (DataInputFold)
            {
                for (int i = 0; i < DataProp.arraySize; i++)
                {
                    CreateDataField(TypeProp, DataProp, i);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

        }//Data Input Field--------------------------------------------

        {
            AddRemoveFold = EditorGUILayout.BeginFoldoutHeaderGroup(AddRemoveFold, "Add / Remove");
            if (AddRemoveFold)
            {
                {
                    EditorGUILayout.BeginHorizontal();
                    TempTypeEnum = (int)(SerializedPropertyType)EditorGUILayout.EnumPopup((SerializedPropertyType)TempTypeEnum);
                    string LTypeName = ConvertTypeName((SerializedPropertyType)TempTypeEnum);

                    if (string.IsNullOrEmpty(TempVaule))
                    {
                        TempVaule = Rapping((SerializedPropertyType)TempTypeEnum);
                    }
                    //if(TempIndex >= 0)
                    {
                        TempVaule = DataField((SerializedPropertyType)TempTypeEnum, TempVaule, TempIndex + 1, "");
                    }
                    if (GUILayout.Button("Add") && !string.IsNullOrEmpty(LTypeName))
                    {
                        DataProp.arraySize += 1;
                        TypeProp.arraySize += 1;
                        DataProp.GetArrayElementAtIndex(DataProp.arraySize - 1).stringValue = TempVaule;//Rapping(null) 필요, TempVaule 직렬화 
                        TypeProp.GetArrayElementAtIndex(TypeProp.arraySize - 1).stringValue = LTypeName;//TypeEnum To Type
                        TempIndex = DataProp.arraySize - 1;
                    }

                    EditorGUILayout.EndHorizontal();

                    PlayerPrefs.SetInt(TempTypeEnumKey, TempTypeEnum);
                    PlayerPrefs.SetInt(TempIndexKey, TempIndex);
                    PlayerPrefs.SetString(TempVauleKey, TempVaule);
                }//Add Data
                {
                    EditorGUILayout.BeginHorizontal();

                    TempIndex = EditorGUILayout.IntField(TempIndex);
                    if (GUILayout.Button("Remove") && DataProp.arraySize > TempIndex && TempIndex >= 0)
                    {
                        DataProp.DeleteArrayElementAtIndex(TempIndex);
                        TypeProp.DeleteArrayElementAtIndex(TempIndex);
                        TempIndex--;
                    }

                    EditorGUILayout.EndHorizontal();

                    PlayerPrefs.SetInt(TempIndexKey, TempIndex);
                }//Remove Data
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }//Add / Remove
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
        SerializedProperty DataPropSlot = null;
        SerializedProperty TypePropSlot = null;
        if (index >= 0)
        {
            DataPropSlot = DataProp.GetArrayElementAtIndex(index);
            TypePropSlot = TypeProp.GetArrayElementAtIndex(index);
        }

        Type LType = ConvertType(TypePropSlot.stringValue);

        var TypeEnum = ConvertTypeEnum(LType.Name);
        //Debug.Log("Type : " + LType.Name + " | " + TypeEnum.ToString());
        string title = LabelText + index;

        GUILayoutOption[] layoutOption = null;
        //layoutOption = new GUILayoutOption[] { GUILayout.Width(200) };

        DataPropSlot.stringValue = DataField(TypeEnum, DataPropSlot.stringValue, index, LabelText, layoutOption);
    }
    public string DataField(SerializedPropertyType TypeEnum, string DataText, int index, string LabelText = "Index ", params GUILayoutOption[] layoutOption)
    {

        //Type LType = ConvertType(TypeText);
        //var TypeEnum = ConvertTypeEnum(LType.Name);
        string title = "";
        if (!string.IsNullOrEmpty(LabelText))
            title = LabelText + index;

        //layoutOption = new GUILayoutOption[] { GUILayout.Width(200) };

        switch (TypeEnum)
        {
            case SerializedPropertyType.Generic:
                EditorGUILayout.LabelField("Not Support");
                break;
            #region done
            case SerializedPropertyType.Integer:
                {
                    //LProp.intValue = EditorGUILayout.IntField(LProp.intValue);
                    //collectionList.Set<int>(index, LProp.intValue);

                    //LProp.stringValue = VariableCollection.Rapping(EditorGUILayout.IntField(VariableCollection.UnRapping<int>(LProp.stringValue)));
                    DataText = Rapping(EditorGUILayout.IntField(title, UnRapping<int>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.Boolean:
                {
                    DataText = Rapping(EditorGUILayout.Toggle(title, UnRapping<bool>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.Float:
                {
                    DataText = Rapping(EditorGUILayout.FloatField(title, UnRapping<float>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.String:
                {
                    DataText = Rapping(EditorGUILayout.TextField(title, UnRapping<string>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.Color:
                {
                    DataText = Rapping(EditorGUILayout.ColorField(title, UnRapping<Color>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.ObjectReference:
                {
                    DataText = Rapping(EditorGUILayout.ObjectField(title, UnRapping<GameObject>(DataText), typeof(GameObject), true, layoutOption));
                    break;
                }//GameObject OR Object ?
            case SerializedPropertyType.LayerMask:
                {
                    DataText = Rapping(EditorGUILayout.LayerField(title, UnRapping<LayerMask>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.Enum:
                {
                    if(UnRapping<Enum>(DataText) != null)
                    {
                        DataText = Rapping(EditorGUILayout.EnumFlagsField(title, UnRapping<Enum>(DataText), layoutOption));
                    }
                    break;
                }
            case SerializedPropertyType.Vector2:
                {
                    DataText = Rapping(EditorGUILayout.Vector2Field(title, UnRapping<Vector2>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.Vector3:
                {
                    DataText = Rapping(EditorGUILayout.Vector3Field(title, UnRapping<Vector3>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.Vector4:
                {
                    DataText = Rapping(EditorGUILayout.Vector4Field(title, UnRapping<Vector4>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.Rect:
                {
                    DataText = Rapping(EditorGUILayout.RectField(title, UnRapping<Rect>(DataText), layoutOption));
                    break;
                }
            #endregion
            case SerializedPropertyType.ArraySize:
                {
                    //for (int i = 0; i < DataPropSlot.arraySize; i++)
                    {
                        //DataField(DataPropSlot, TypePropSlot, collectionList, i);
                    }
                    EditorGUILayout.LabelField("Add To Script");
                    break;
                }//-------------아직 지원X
            case SerializedPropertyType.Character:
                {
                    //??먼지 모르겠음
                    EditorGUILayout.LabelField("Not Support");
                    break;
                }//Not Support
            case SerializedPropertyType.AnimationCurve:
                {
                    DataText = Rapping(EditorGUILayout.CurveField(title, UnRapping<AnimationCurve>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.Bounds:
                {
                    DataText = Rapping(EditorGUILayout.BoundsField(title, UnRapping<Bounds>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.Gradient:
                {
                    DataText = Rapping(EditorGUILayout.GradientField(title, UnRapping<Gradient>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.Quaternion:
                {
                    DataText = Rapping(EditorGUILayout.Vector4Field(title, UnRapping<Vector4>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.ExposedReference:
            case SerializedPropertyType.FixedBufferSize:
                EditorGUILayout.LabelField("Not Support");
                break;
            case SerializedPropertyType.Vector2Int:
                {
                    DataText = Rapping(EditorGUILayout.Vector2IntField(title, UnRapping<Vector2Int>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.Vector3Int:
                {
                    DataText = Rapping(EditorGUILayout.Vector3IntField(title, UnRapping<Vector3Int>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.RectInt:
                {
                    DataText = Rapping(EditorGUILayout.RectIntField(title, UnRapping<RectInt>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.BoundsInt:
                {
                    DataText = Rapping(EditorGUILayout.BoundsIntField(title, UnRapping<BoundsInt>(DataText), layoutOption));
                    break;
                }
            case SerializedPropertyType.ManagedReference:
                {
                    EditorGUILayout.LabelField("Not Support");
                    break;
                }//Not Support
            default:
                {
                    EditorGUILayout.TextArea("Unknown Type");
                    break;
                }
        }
        return DataText;
    }
}//PropertyDrawer