using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Expand;

[Serializable]
public class Map<T>
{
    [SerializeField]
    private List<T> Vaule;

    public Map(List<T> vaule = null)
    {
        Vaule = new List<T>();
        if (vaule != null)
        {
            Vaule = vaule;
        }
    }

    public List<T> Get()
    {
        return Vaule;
    }
    public T Get(int index)
    {
        if (Vaule.Count > index && index >= 0)
        {
            return Vaule[index];
        }
        else
        {
            return default;
        }
    }
    public bool Set(int index, T t = default)
    {
        if (Vaule.Count > index && index >= 0)
        {
            Vaule[index] = t;
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool Exist(T t)
    {
        return Vaule.Contains(t);
    }

    public void Add(T t = default)
    {
        Vaule.Add(t);
    }
    public void Remove(T t)
    {
        Vaule.Remove(t);
    }
    public void RemoveAt(int index)
    {
        Vaule.RemoveAt(index);
    }

    public int Length => Vaule.Count;

    public static Map<T> operator +(Map<T> map, T t)
    {
        map.Add(t);
        return map;
    }
    public static Map<T> operator -(Map<T> map, T t)
    {
        map.Remove(t);
        return map;
    }
    public static Map<T> operator -(Map<T> map, int index)
    {
        map.RemoveAt(index);
        return map;
    }

    public static int ArraySize(SerializedProperty property)
    {
        return property.FindPropertyRelative("Vaule").arraySize;
    }

}

[Serializable]
public class IntMap<T>
{
    [Serializable]
    public struct Vaule
    {
        public int key;
        public T vaule;
    }//int key , T vaule
    [SerializeField]
    private List<Vaule> Vaules;

    public IntMap(List<Vaule> vaules = null)
    {
        Vaules = new List<Vaule>();
        if (vaules != null)
        {
            Vaules = vaules;
        }
    }

    public List<Vaule> Get()
    {
        return Vaules;
    }
    public Vaule Get(int index)
    {
        if (Vaules.Count > index && index >= 0)
        {
            return Vaules[index];
        }
        else
        {
            return default;
        }
    }
    public Vaule GetKey(int Key)
    {
        if (Vaules.Exists(v => v.key == Key))
        {
            return Vaules.Find(v => v.key == Key);
        }
        else
        {
            return default;
        }
    }
    public T Find(int index)
    {
        if (Vaules.Count > index && index >= 0)
        {
            return Get(index).vaule;
        }
        else
        {
            return default;
        }
    }
    public T FindToKey(int Key)
    {
        return GetKey(Key).vaule;
    }
    public int FindIndex(int Key)
    {
        return Vaules.FindIndex(v => v.key == Key);
    }

    public bool Set(int index, T t = default)
    {
        if (Vaules.Count > index && index >= 0)
        {
            Vaules[index] = new Vaule { key = Vaules[index].key, vaule = t };
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool SetToKey(int Key, T t = default)
    {
        int temp = FindIndex(Key);
        if (temp >= 0)
        {
            Vaules[temp] = new Vaule { key = Key, vaule = t };
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Exist(Vaule vaule)
    {
        return Vaules.Contains(vaule);
    }
    public bool Exist(int Key)
    {
        return Vaules.Exists(v => v.key == Key);
    }

    public void Add(Vaule vaule = default)
    {
        Vaules.Add(vaule);
    }
    public void Add(int key, T t = default)
    {
        Vaule Temp = new Vaule
        {
            key = key,
            vaule = t
        };

        Vaules.Add(Temp);
    }
    public void Remove(Vaule vaule)
    {
        Vaules.Remove(vaule);
    }
    public void Remove(int Key)
    {
        RemoveAt(FindIndex(Key));
    }
    public void RemoveAt(int index)
    {
        Vaules.RemoveAt(index);
    }

    public static IntMap<T> operator +(IntMap<T> intMap, Vaule vaule)
    {
        intMap.Add(vaule);
        return intMap;
    }
    public static IntMap<T> operator +(IntMap<T> intMap, int key)
    {
        intMap.Add(key);
        return intMap;
    }
    public static IntMap<T> operator -(IntMap<T> intMap, Vaule vaule)
    {
        intMap.Remove(vaule);
        return intMap;
    }
    public static IntMap<T> operator -(IntMap<T> intMap, int key)
    {
        intMap.RemoveAt(key);
        return intMap;
    }

    public int Count
    {
        get { return Vaules.Count; }
    }
    public void Clear()
    {
        Vaules.Clear();
    }
}

[Serializable]
public class Map<T, V>
{
    [Serializable]
    public struct MapSlot
    {
        public T Key;
        public V Vaule;
    }
    [SerializeField]
    private List<MapSlot> Slots = new List<MapSlot>();
    //[SerializeField]
    //List<T> Key = new List<T>();
    //[SerializeField]
    //List<V> Vaule = new List<V>();

    public Map()
    {
        //Key = new List<T>();
        //Vaule = new List<V>();
        Slots = new List<MapSlot>();
    }
    public void Clear()
    {
        Slots.Clear();
        //Key.Clear();
        //Vaule.Clear();
    }
    public void Add(T key, V vaule)
    {
        //Key.Add(key);
        //Vaule.Add(vaule);
        Slots.Add(new MapSlot { Key = key, Vaule = vaule});
    }
    public bool Remove(int i)
    {
        /*
        if (i >= 0 && i < Key.Count)
        {
            Key.RemoveAt(i);
            Vaule.RemoveAt(i);

            return true;
        }
        return false;*/
        if (i >= 0 && i < Slots.Count)
        {
            Slots.RemoveAt(i);

            return true;
        }
        return false;
    }

    public int Count
    {
        //get { return Key.Count; }
        get { return Slots.Count; }
    }
    public List<MapSlot> Get()
    {
        return Slots;
    }
    public List<T> GetKey()
    {
        //return Key;
        List<T> result = new List<T>();

        for (int i = 0; i < Slots.Count; i++)
        {
            result.Add(Slots[i].Key);
        }
        return result;
    }
    public T GetKey(int index)
    {
        //return Key[index];
        return Slots[index].Key;
    }
    public List<V> GetVaule()
    {
        //return Vaule;

        List<V> result = new List<V>();

        for (int i = 0; i < Slots.Count; i++)
        {
            result.Add(Slots[i].Vaule);
        }
        return result;
    }
    public V GetVaule(int index)
    {
        //return Vaule[index];
        return Slots[index].Vaule;
    }

    public bool SetKey(int index, T key)
    {
        /*
        if (index >= 0 && index < Key.Count)
        {
            Key[index] = key;
            return true;
        }else
        {
            return false;
        }*/

        if (index >= 0 && index < Slots.Count)
        {
            MapSlot temp = Slots[index];
            temp.Key = key;
            Slots[index] = temp;
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool SetVaule(int index, V vaule)
    {
        /*
        if (index >= 0 && index < Vaule.Count)
        {
            Vaule[index] = vaule;
            return true;
        }else
        {
            return false;
        }*/

        if (index >= 0 && index < Slots.Count)
        {
            MapSlot temp = Slots[index];
            temp.Vaule = vaule;
            Slots[index] = temp;
            return true;
        }
        else
        {
            return false;
        }
    }

}

[Serializable]
public class GroupMap<T>
{
    [SerializeField]
    List<T> Key = new List<T>();
    [SerializeField]
    List<int> Vaule = new List<int>();

    public Map<int, Map<int>> SortList = new Map<int, Map<int>>();

    public GroupMap()
    {
        Key = new List<T>();
        Vaule = new List<int>();

        SortList = new Map<int, Map<int>>();
    }

    public List<T> GetKey()
    {
        return Key;
    }
    public T GetKey(int index)
    {
        return Key[index];
    }
    public List<int> GetVaule()
    {
        return Vaule;
    }
    public int GetVaule(int index)
    {
        return Vaule[index];
    }

    public void SortElement(int index)
    {
        int SortSlot = SortList.GetKey().FindIndex(t => t == Vaule[index]);
        if (SortSlot >= 0)
        {
            SortList.GetVaule(SortSlot).Add(index);
        }else
        {
            var temp = new Map<int>();
            temp.Add(index);

            SortList.Add(Vaule[index], temp);
        }
    }

    public void Add(T key, int vaule)
    {
        Key.Add(key);
        Vaule.Add(vaule);

        SortElement(Key.Count - 1);
    }

    public void Set(int index, T key)
    {
        Key[index] = key;
    }
    public void Set(int index, int vaule)
    {
        //이전값으로 SortList에서 저거 > 값변경 > SortElement
        RemoveSortList(index);

        Vaule[index] = vaule;

        SortElement(index);
    }
    public bool Remove(int index)
    {
        if (index >= 0 && index < Length)
        {
            RemoveSortList(index);

            Key.RemoveAt(index);
            Vaule.RemoveAt(index);

            return true;
        }else
        {
            return false;
        }
    }
    public void Clear()
    {
        Key.Clear();
        Vaule.Clear();
        SortList.Clear();
    }
    public int Length => Key.Count;

    void RemoveSortList(int index)
    {
        int SortIndex = SortList.GetKey().FindIndex(t => t == Vaule[index]);
        SortList.GetVaule(SortIndex).RemoveAt(index);
        if (SortList.GetVaule(SortIndex).Length == 0)
        {
            SortList.Remove(SortIndex);
        }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Map<>))]
public class MapEditor : PropertyDrawer
{
    Rect DrawRect;
    bool fold = false;

    SerializedProperty vaule;

    float SlotOffset = 10f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (fold)
        {
            return (property.FindPropertyRelative("Vaule").arraySize + 2) * 20;
        }
        else
        {
            return 20;
        }
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DrawRect = new Rect(position.x, position.y, position.width, 20);
        vaule = property.FindPropertyRelative("Vaule");

        //fold = EditorGUI.Foldout(DrawRect, fold, label);
        if (GUI.Button(DrawRect, (label.text + (fold ? " (open) " : " (close) ") + " ( " + vaule.arraySize + " ) ")))
        {
            fold = !fold;
        }

        if (fold)
        {
            Rect SlotSize = new Rect(position.x, position.y, position.width - 50, 20);
            for (int i = 0; i < vaule.arraySize; i++)
            {
                DrawRect = EditorExpand.NextLine(SlotSize, DrawRect);
                DrawRect = new Rect(DrawRect.x + SlotOffset, DrawRect.y, DrawRect.width - SlotOffset, DrawRect.height);

                EditorGUI.PropertyField(DrawRect, vaule.GetArrayElementAtIndex(i), new GUIContent { text = "" });
                //DrawRect = new Rect((position.width - position.x - 25), DrawRect.y, 50, DrawRect.height);
                DrawRect = new Rect(DrawRect.x + DrawRect.width, DrawRect.y, 50, DrawRect.height);
                if (GUI.Button(DrawRect, " - "))
                {
                    vaule.DeleteArrayElementAtIndex(i);
                }

            }
            {
                DrawRect = EditorExpand.NextLine(position, DrawRect);
                DrawRect = new Rect(DrawRect.x + SlotOffset, DrawRect.y, DrawRect.width - SlotOffset, DrawRect.height);
                if (GUI.Button(DrawRect, "Add"))
                {
                    vaule.arraySize++;
                }
            }

        }
    }
}
//[CustomPropertyDrawer(typeof(IntMap<>))]
public class IntMapEditor : PropertyDrawer
{
    Rect DrawRect;
    bool fold = false;

    SerializedProperty list;
    SerializedProperty key;
    SerializedProperty vaule;

    float SlotOffset = 10f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (fold)
        {
            return (property.FindPropertyRelative("Vaules").arraySize + 2) * 20;
        }
        else
        {
            return 20;
        }
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DrawRect = new Rect(position.x, position.y, position.width, 20);
        //key = property.FindPropertyRelative("Key");
        //vaule = property.FindPropertyRelative("Vaule");
        list = property.FindPropertyRelative("Vaules");

        //fold = EditorGUI.Foldout(DrawRect, fold, label);
        if (GUI.Button(DrawRect, (label.text + (fold ? " (open) " : " (close) ") + " ( " + list.arraySize + " ) ")))
        {
            fold = !fold;
        }

        if (fold)
        {
            Rect SlotSize = new Rect(position.x, position.y, position.width - 50, 20);
            for (int i = 0; i < list.arraySize; i++)
            {
                key = list.GetArrayElementAtIndex(i).FindPropertyRelative("key");
                vaule = list.GetArrayElementAtIndex(i).FindPropertyRelative("vaule");

                DrawRect = EditorExpand.NextLine(SlotSize, DrawRect);
                DrawRect = EditorExpand.RateRect(SlotSize, DrawRect, 0, 2, SlotOffset, 20);
                EditorGUI.PropertyField(DrawRect, key, new GUIContent { text = "" });
                DrawRect = EditorExpand.RateRect(SlotSize, DrawRect, 1, 2, SlotOffset, 20);
                EditorGUI.PropertyField(DrawRect, vaule, new GUIContent { text = "" });
                DrawRect = new Rect(DrawRect.x + DrawRect.width, DrawRect.y, 50, DrawRect.height);
                if (GUI.Button(DrawRect, " - "))
                {
                    list.DeleteArrayElementAtIndex(i);
                }

            }
            {
                DrawRect = EditorExpand.NextLine(position, DrawRect);
                DrawRect = new Rect(DrawRect.x + SlotOffset, DrawRect.y, DrawRect.width - SlotOffset, DrawRect.height);

                if (GUI.Button(DrawRect, "Add"))
                {
                    list.arraySize++;
                }
            }

        }
    }
}

[CustomPropertyDrawer(typeof(IntMap<>.Vaule))]
public class IntMapSlotEditor : PropertyDrawer
{
    Rect DrawRect;

    SerializedProperty key;
    SerializedProperty vaule;

    float SlotOffset = 10f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        vaule = property.FindPropertyRelative("vaule");
        if (LargeProperty(vaule.propertyType.ToString()))
        {
            return 40;
        }

        return 20;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DrawRect = new Rect(position.x, position.y, position.width, 20);
        key = property.FindPropertyRelative("key");
        vaule = property.FindPropertyRelative("vaule");

        DrawRect = EditorExpand.RateRect(position, DrawRect, 0, 2, SlotOffset, 20);
        EditorGUI.PropertyField(DrawRect, key, new GUIContent { text = "" });
        DrawRect = EditorExpand.RateRect(position, DrawRect, 1, 2, SlotOffset, 20);
        EditorGUI.PropertyField(DrawRect, vaule, new GUIContent { text = "" });
    }

    public bool LargeProperty(string name)
    {
        switch (name)
        {

            case "Rect":
            case "Bounds":
            case "Quaternion":
            case "RectInt":
            case "BoundsInt":
                return true;
            default:
                return false;
        }
    }
}

//[CustomPropertyDrawer(typeof(Map<,>))]
public class MapEditor_KV : PropertyDrawer
{
    Rect DrawRect;
    bool fold = false;

    SerializedProperty key;
    SerializedProperty vaule;

    float SlotOffset = 10f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (fold)
        {
            return (property.FindPropertyRelative("Key").arraySize + 2) * 20;
        }
        else
        {
            return 20;
        }
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DrawRect = new Rect(position.x, position.y, position.width, 20);
        key = property.FindPropertyRelative("Key");
        vaule = property.FindPropertyRelative("Vaule");

        //fold = EditorGUI.Foldout(DrawRect, fold, label);
        if (GUI.Button(DrawRect, (label.text + (fold ? " (open) " : " (close) ") + " (" + key.arraySize + ") " )))
        {
            fold = !fold;
        }

        if (fold)
        {
            Rect SlotSize = new Rect(position.x, position.y, position.width - 50, 20);
            for (int i = 0; i < key.arraySize; i++)
            {
                DrawRect = EditorExpand.NextLine(SlotSize, DrawRect);
                DrawRect = EditorExpand.RateRect(SlotSize, DrawRect, 0, 2, SlotOffset, 20);
                EditorGUI.PropertyField(DrawRect, key.GetArrayElementAtIndex(i), new GUIContent { text = ""});
                DrawRect = EditorExpand.RateRect(SlotSize, DrawRect, 1, 2, SlotOffset, 20);
                EditorGUI.PropertyField(DrawRect, vaule.GetArrayElementAtIndex(i), new GUIContent { text = "" });
                //DrawRect = new Rect((position.width - position.x - 25), DrawRect.y, 50, DrawRect.height);
                DrawRect = new Rect(DrawRect.x + DrawRect.width, DrawRect.y, 50, DrawRect.height);
                if (GUI.Button(DrawRect, " - "))
                {
                    key.DeleteArrayElementAtIndex(i);
                    vaule.DeleteArrayElementAtIndex(i);
                }

            }
            {
                DrawRect = EditorExpand.NextLine(position, DrawRect);
                DrawRect = new Rect(DrawRect.x + SlotOffset, DrawRect.y, DrawRect.width - SlotOffset, DrawRect.height);
                if (GUI.Button(DrawRect, "Add"))
                {
                    key.arraySize++;
                    vaule.arraySize++;
                    
                }
            }

        }
    }
}

[CustomPropertyDrawer(typeof(Map<,>.MapSlot))]
public class MapSlotEditor : PropertyDrawer
{
    Rect DrawRect;

    SerializedProperty key;
    SerializedProperty vaule;

    float SlotOffset = 10f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        key = property.FindPropertyRelative("Key");
        vaule = property.FindPropertyRelative("Vaule");
        if(LargeProperty(key.propertyType.ToString()) || LargeProperty(vaule.propertyType.ToString()))
        {
            return 40;
        }

        return 20;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DrawRect = new Rect(position.x, position.y, position.width, 20);
        key = property.FindPropertyRelative("Key");
        vaule = property.FindPropertyRelative("Vaule");

        DrawRect = EditorExpand.RateRect(position, DrawRect, 0, 2, SlotOffset, 20);
        EditorGUI.PropertyField(DrawRect, key, new GUIContent { text = "" });
        DrawRect = EditorExpand.RateRect(position, DrawRect, 1, 2, SlotOffset, 20);
        EditorGUI.PropertyField(DrawRect, vaule, new GUIContent { text = "" });
    }

    public bool LargeProperty(string name)
    {
        switch (name)
        {

            case "Rect":
            case "Bounds":
            case "Quaternion":
            case "RectInt":
            case "BoundsInt":
                return true;
            default:
                return false;
        }
    }
}

[CustomPropertyDrawer(typeof(GroupMap<>))]
public class GroupMapEditor : PropertyDrawer
{
    Rect DrawRect;
    bool fold = false;
    List<bool> SlotFold = new List<bool>();

    SerializedProperty key;
    SerializedProperty vaule;
    SerializedProperty Sort;

    float SlotOffset = 10f;
    float RemoveButton = 50f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int Line = 0;
        if (fold)
        {
            Line = 3;
            Line += property.FindPropertyRelative("SortList").FindPropertyRelative("Key").arraySize;

            for (int i = 0; i < SlotFold.Count; i++)
            {
                if (i < property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").arraySize && SlotFold[i])
                {
                    Line += property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").GetArrayElementAtIndex(i).FindPropertyRelative("Vaule").arraySize;
                }
                
            }

            //SlotFold 적용
            return Line * 20;
        }
        else
        {
            return 20;
        }
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        DrawRect = new Rect(position.x, position.y, position.width, 20);
        key = property.FindPropertyRelative("Key");
        vaule = property.FindPropertyRelative("Vaule");
        Sort = property.FindPropertyRelative("SortList");

        for (int i = SlotFold.Count; i < MapSize(Sort); i++)
        {
            SlotFold.Add(false);
        }//SortList의 크기와 SlotFold 크기와 같게 + 항목이 제거시 같이 제거


        if (GUI.Button(DrawRect, (label.text + (fold ? " (open) " : " (close) ") + " (" + key.arraySize + ") ")))
        {
            fold = !fold;
        }

        if (fold)
        {
            bool Changed = false;
            int RemoveGroup = -1;
            int RemoveSlot = -1;

            for (int i = 0; i < MapSize(Sort); i++)
            {
                SerializedProperty SortKey = Sort.FindPropertyRelative("Key").GetArrayElementAtIndex(i);

                var SortSlot = property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").GetArrayElementAtIndex(i).FindPropertyRelative("Vaule");//Map<int>
                //Sort.FindPropertyRelative("Vaule").arraySize

                DrawRect = EditorExpand.NextLine(position, DrawRect, SlotOffset);
                if ((i < SlotFold.Count))
                {
                    if (GUI.Button(DrawRect, ("Group " + SortKey.intValue + (SlotFold[i] ? " (open) " : " (close) ") + (" [" + SortSlot.arraySize + "] "))))
                    {
                        SlotFold[i] = !SlotFold[i];
                    }

                    if (SlotFold[i])
                    {
                        
                        for (int j = 0; j < SortSlot.arraySize; j++)
                        {
                            DrawRect = EditorExpand.NextLine(position, DrawRect, (SlotOffset * 2));
                            DrawRect = EditorExpand.RateRect(position, DrawRect, 0, 2, (SlotOffset * 2 + RemoveButton), 20);
                            DrawRect = new Rect(DrawRect.x - RemoveButton, DrawRect.y, DrawRect.width, DrawRect.height);

                            int Lindex = SortSlot.GetArrayElementAtIndex(j).intValue;

                            if (Lindex >= key.arraySize)
                            {
                                Changed = true;
                                break;
                            }

                            DrawRect = EditorExpand.PropertyField(position, DrawRect, key.GetArrayElementAtIndex(Lindex), (i + " - " + j + " "), 2);

                            int temp = vaule.GetArrayElementAtIndex(Lindex).intValue;
                            DrawRect = EditorExpand.RateRect(position, DrawRect, 1, 2, (SlotOffset * 2), 20);

                            Rect TempRect = new Rect(position.x, position.y, position.width - RemoveButton * 2, position.height);
                            DrawRect = EditorExpand.PropertyField(TempRect, DrawRect, vaule.GetArrayElementAtIndex(Lindex), "", 2);

                            DrawRect = new Rect(DrawRect.x, DrawRect.y, RemoveButton, DrawRect.height);
                            if (GUI.Button(DrawRect, " - "))
                            {
                                RemoveGroup = i;
                                RemoveSlot = j;
                            }

                            if (temp != vaule.GetArrayElementAtIndex(Lindex).intValue)
                            {
                                Changed = true;
                            }
                        }
                    }
                }
            }

            if (RemoveSlot >= 0)
            {
                var SortSlot = property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").GetArrayElementAtIndex(RemoveGroup).FindPropertyRelative("Vaule");//Map<int>

                key.DeleteArrayElementAtIndex(SortSlot.GetArrayElementAtIndex(RemoveSlot).intValue);
                vaule.DeleteArrayElementAtIndex(SortSlot.GetArrayElementAtIndex(RemoveSlot).intValue);

                SortMap(property);
            }

            if (Changed)
            {
                SortMap(property);
            }

            DrawRect = EditorExpand.NextLine(position, DrawRect, SlotOffset);
            if (GUI.Button(DrawRect, "Add"))
            {
                key.arraySize++;
                vaule.arraySize++;

                SortMap(property);
                //키, 값 추가함
            }

            DrawRect = EditorExpand.NextLine(position, DrawRect, SlotOffset);
            if (GUI.Button(DrawRect, "Clear"))
            {
                property.FindPropertyRelative("Key").arraySize = 0;
                property.FindPropertyRelative("Vaule").arraySize = 0;

                property.FindPropertyRelative("SortList").FindPropertyRelative("Key").arraySize = 0;
                property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").arraySize = 0;
            }
        }
    }
    public int MapSize(SerializedProperty property)
    {
        return property.FindPropertyRelative("Key").arraySize;
    }

    public void SortMap(SerializedProperty property, int index)
    {
        int vauleData = vaule.GetArrayElementAtIndex(index).intValue;

        //SortList내 vauleData인 키가 있는지 확인 
        //있다면 index추가 , 없으면 SortList 크기증가후 index추가 

        int FindKey = -1;
        for (int i = 0; i < MapSize(Sort); i++)
        {
            if (vauleData == Sort.FindPropertyRelative("Key").GetArrayElementAtIndex(i).intValue)
            {
                FindKey = i;
                break;
            }
        }

        if (FindKey >= 0)
        {
            //var SortVaule = Sort.FindPropertyRelative("Vaule").GetArrayElementAtIndex(FindKey).FindPropertyRelative("Vaule");//Map<int>
            //int Lsize = SortVaule.arraySize++;
            //SortVaule.GetArrayElementAtIndex(Lsize).intValue = vauleData;

            Debug.Log(property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").GetArrayElementAtIndex(FindKey).FindPropertyRelative("Vaule").propertyPath);

            int Lsize = property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").GetArrayElementAtIndex(FindKey).FindPropertyRelative("Vaule").arraySize++;
            property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").GetArrayElementAtIndex(FindKey)
                .FindPropertyRelative("Vaule").GetArrayElementAtIndex(Lsize - 1).intValue = vauleData;
        }
        else
        {
            int sortSize = property.FindPropertyRelative("SortList").FindPropertyRelative("Key").arraySize++;
            property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").arraySize++;

            property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").GetArrayElementAtIndex(sortSize);//Map<int>

            property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").GetArrayElementAtIndex(sortSize).FindPropertyRelative("Vaule").arraySize++;
            property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").GetArrayElementAtIndex(sortSize).FindPropertyRelative("Vaule").GetArrayElementAtIndex(0).intValue = vauleData;
        }
    }
    public void SortMap(SerializedProperty property)
    {
        property.FindPropertyRelative("SortList").FindPropertyRelative("Key").arraySize = 0;
        property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").arraySize = 0;

        for (int i = 0; i < property.FindPropertyRelative("Vaule").arraySize; i++)
        {
            int Lvaule = property.FindPropertyRelative("Vaule").GetArrayElementAtIndex(i).intValue;

            int Find = -1;
            for (int f = 0; f < property.FindPropertyRelative("SortList").FindPropertyRelative("Key").arraySize; f++)
            {
                if (Lvaule == property.FindPropertyRelative("SortList").FindPropertyRelative("Key").GetArrayElementAtIndex(f).intValue)
                {
                    Find = f;
                    break;
                }
            }

            if (Find >= 0)
            {
                int Lsize = property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").GetArrayElementAtIndex(Find).FindPropertyRelative("Vaule").arraySize++;
                property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").GetArrayElementAtIndex(Find).FindPropertyRelative("Vaule").GetArrayElementAtIndex(Lsize - 1).intValue = i;

                //                 SortList                      .Vaule                      Map<int>                   .Vaule                             (int)
            }
            else
            {
                int Lmap = property.FindPropertyRelative("SortList").FindPropertyRelative("Key").arraySize++;
                property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").arraySize++;
                property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").GetArrayElementAtIndex(Lmap).FindPropertyRelative("Vaule").arraySize = 0;

                property.FindPropertyRelative("SortList").FindPropertyRelative("Key").GetArrayElementAtIndex(Lmap ).intValue = Lvaule;
                int Mvaule = property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").GetArrayElementAtIndex(Lmap ).FindPropertyRelative("Vaule").arraySize++;
                property.FindPropertyRelative("SortList").FindPropertyRelative("Vaule").GetArrayElementAtIndex(Lmap ).FindPropertyRelative("Vaule").GetArrayElementAtIndex(Mvaule ).intValue = i;
            }
        }
    }
}
#endif

