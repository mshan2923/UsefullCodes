using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using System;
using UnityEngine.UIElements;

[System.Serializable]
public class VariableCollection
{
    [System.Serializable]
    class Wrap<T>
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

            if(!string.IsNullOrEmpty(data))
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
            }else
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
        public T Get<T>(int index)
        {
            if(index < Data.Count && index >= 0 && Data.Count != 0)
            {
                T vaule = JsonUtility.FromJson<Wrap<T>>(Data[index]).Get;
                return vaule;
            }else
            {
                return default;
            }
        }
        public List<T> Gets<T>()
        {
            var indexs = GetEqualTypes<T>();
            List<T> Vaules = new List<T>();
            for(int i = 0; i < indexs.Count; i++)
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
            for(int i = 0; i < DataType.Count; i++)
            {
                if(ConvertType(DataType[i]) == typeof(T))
                {
                    //indexs.Add(i);
                    T temp = Get<T>(i); 
                    if(vaule.Equals(temp))
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
    }
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
}

[CustomPropertyDrawer(typeof(VariableCollection.CollectionList))]
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

        GetData(position, property);
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
    public void CreateDataField(string type, string Data , Rect rect)
    {
        Type LType = VariableCollection.ConvertType(type);

        switch (Type.GetTypeCode(LType))
        {
            case TypeCode.Boolean:
                //return EditorGUI.PropertyField(rect,);
                break;
            case TypeCode.Byte:
                break;
            case TypeCode.Char:
                break;
            case TypeCode.DateTime:
                break;
            case TypeCode.DBNull:
                break;
            case TypeCode.Decimal:
                break;
            case TypeCode.Double:
                break;
            case TypeCode.Empty:
                break;
            case TypeCode.Int16:
                break;
            case TypeCode.Int32:
                break;
            case TypeCode.Int64:
                break;
            case TypeCode.Object:
                break;
            case TypeCode.SByte:
                break;
            case TypeCode.Single:
                break;
            case TypeCode.String:
                break;
            case TypeCode.UInt16:
                break;
            case TypeCode.UInt32:
                break;
            case TypeCode.UInt64:
                break;
            default:
                break;
        }
    }//어디에 타입별로 프로퍼티필드 만든는거 있었는데??
}