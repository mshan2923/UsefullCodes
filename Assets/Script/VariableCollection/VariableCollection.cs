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
        public List<System.Type> DataType;//String 으로 바꿔야 ProperyDrawer 에서 사용 가능

        public CollectionList()
        {
            Data = new List<string>();
            DataType = new List<Type>();
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
                    DataType[index] = typeof(T);
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
            DataType.Add(typeof(T));
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
            return DataType[index];
        }
        public int Find<T>(T vaule)
        {
            List<int> indexs = new List<int>();
            for(int i = 0; i < DataType.Count; i++)
            {
                if(DataType[i] == typeof(T))
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
                if (DataType[i] == typeof(T))
                {
                    indexs.Add(i);
                }
            }
            return indexs;
        }
    }
}

[CustomPropertyDrawer(typeof(VariableCollection.CollectionList))]
public class CollectionListProperty : PropertyDrawer
{
    public override bool CanCacheInspectorGUI(SerializedProperty property)
    {
        return base.CanCacheInspectorGUI(property);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float temp = base.GetPropertyHeight(property, label);
        temp += GetArrayHeight(property, "Data", 25, 0);

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

        EditorGUI.PropertyField(position, property.FindPropertyRelative("Data"), new GUIContent());
        if(property.FindPropertyRelative("DataType") != null)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("DataType"), new GUIContent());
        }
        else
        {
            Debug.LogWarning("DataType is null??");
        }
        //해당 타입으로 변환해서 보여주는거....

        EditorGUI.EndProperty();

        GetData(position, property);
    }

    public void GetData(Rect position, SerializedProperty property)
    {
        int Count = property.FindPropertyRelative("Data").arraySize;
        for(int i = 0; i < Count; i++)
        {
            string text = property.FindPropertyRelative("Data").GetArrayElementAtIndex(i).stringValue;
            //Type type = property.FindPropertyRelative("DataType").GetArrayElementAtIndex(i).name.GetType();
            Debug.Log(property.FindPropertyRelative("DataType") + " // " + text);
            
        }
        //property.FindPropertyRelative("Data").GetArrayElementAtIndex(0);
    }
}