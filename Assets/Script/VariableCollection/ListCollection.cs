using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static VariableCollection;

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
        else if (index == Data.Count)
        {
            ForceAdd(TypeName, VauleText);
            return true;
        }
        else
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
}

[CustomPropertyDrawer(typeof(CollectionList))]
public class CollectionListProperty : PropertyDrawer
{
    SerializedProperty DataProp;
    SerializedProperty TypeProp;

    //float DrawHeight = 0;

    string TempTypeEnumKey = "TempTypeEnumKey";
    string TempIndexKey = "TempIndexKey";
    string TempVauleKey = "TempVauleKey";
    int TempTypeEnum = -1;
    int TempIndex = -1;
    string TempVaule = "";

    bool DataInputFold = false;
    bool AddRemoveFold = false;

    UnityEditorInternal.ReorderableList list;
    int SelectIndex = 0;

    #region NotUse
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
    public float GetArrayHeight(SerializedProperty property, string name = "Data", float indexSize = 25, float Default = 50)
    {
        var Local = property.FindPropertyRelative(name);
        int size = Mathf.Clamp(Local.arraySize, 1, Local.arraySize) + 1;//최소 2칸 , 갯수가 1개이상 - 갯수 + 1
        return Local.isExpanded ? size * indexSize + Default : Default;
    }
    #endregion

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);

        DataProp = property.FindPropertyRelative("Data");
        TypeProp = property.FindPropertyRelative("DataType");

        if (list == null)
        {
            list = new UnityEditorInternal.ReorderableList(property.serializedObject, DataProp)
            {
                drawHeaderCallback = DrawHeader,
                drawElementCallback = DrawListItems
            };

            list.onSelectCallback = list =>
            {
                SelectIndex = list.index;

            };
            list.onReorderCallback = list =>
            {
                //ReOrder(SelectIndex, list.index);
                TypeProp.MoveArrayElement(SelectIndex, list.index);
            };
            list.onRemoveCallback = list =>
            {
                Debug.Log("Remove " + SelectIndex);
                DataProp.DeleteArrayElementAtIndex(SelectIndex);
                TypeProp.DeleteArrayElementAtIndex(SelectIndex);
            };
            list.onAddCallback = list =>
            {

            };

        }//Spawn ReOrderableList => 매 프래임마다 초기화 돼서 + 인덱스 교체 적용
        list.DoLayoutList();

        TempTypeEnum = PlayerPrefs.GetInt(TempTypeEnumKey);
        TempIndex = PlayerPrefs.GetInt(TempIndexKey);
        TempVaule = PlayerPrefs.GetString(TempVauleKey);

        {
            DataInputFold = EditorGUILayout.BeginFoldoutHeaderGroup(DataInputFold, ("Debug Data Field" + "  /  Length : " + DataProp.arraySize));
            if (DataInputFold)
            {
                for (int i = 0; i < DataProp.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.TextArea(DataProp.GetArrayElementAtIndex(i).stringValue, GUILayout.Width(200));
                    EditorGUILayout.TextArea(TypeProp.GetArrayElementAtIndex(i).stringValue);

                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

        }//Test Data Field--------------------------------------------

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
                        TempVaule = DataField(LTypeName, TempVaule, TempIndex + 1, "");
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

        property.serializedObject.ApplyModifiedProperties();//Apply Vaule
    }

    public void DataReOrderField(Rect rect, int index, string LabelText = "Index ")
    {
        SerializedProperty DataPropSlot = null;
        SerializedProperty TypePropSlot = null;
        if (index >= 0 && DataProp.arraySize > index)
        {
            DataPropSlot = DataProp.GetArrayElementAtIndex(index);
            TypePropSlot = TypeProp.GetArrayElementAtIndex(index);
        }
        string DataText = DataPropSlot.stringValue;

        Type LType = ConvertType(TypePropSlot.stringValue);
        var TypeEnum = ConvertTypeEnum(LType.Name);

        string title = LabelText + index;

        //DataPropSlot.stringValue = DataField(TypePropSlot.stringValue, DataPropSlot.stringValue, index, LabelText, false, layoutOption);//EditorGUI로 위치 직접 지정해야함
        switch (TypeEnum)
        {
            case SerializedPropertyType.Generic:
                //EditorGUI.LabelField(rect, "Not Support / Generic");                
                break;
            #region done
            case SerializedPropertyType.Integer:
                {
                    //LProp.intValue = EditorGUILayout.IntField(LProp.intValue);
                    //collectionList.Set<int>(index, LProp.intValue);

                    //LProp.stringValue = VariableCollection.Rapping(EditorGUILayout.IntField(VariableCollection.UnRapping<int>(LProp.stringValue)));
                    DataText = Rapping(EditorGUI.IntField(rect, title, UnRapping<int>(DataText)));
                    break;
                }
            case SerializedPropertyType.Boolean:
                {
                    DataText = Rapping(EditorGUI.Toggle(rect, title, UnRapping<bool>(DataText)));
                    break;
                }
            case SerializedPropertyType.Float:
                {
                    DataText = Rapping(EditorGUI.FloatField(rect, title, UnRapping<float>(DataText)));
                    break;
                }
            case SerializedPropertyType.String:
                {
                    DataText = Rapping(EditorGUI.TextField(rect, title, UnRapping<string>(DataText)));
                    break;
                }
            case SerializedPropertyType.Color:
                {
                    DataText = Rapping(EditorGUI.ColorField(rect, title, UnRapping<Color>(DataText)));
                    break;
                }
            case SerializedPropertyType.ObjectReference:
                {
                    DataText = Rapping(EditorGUI.ObjectField(rect, title, UnRapping<GameObject>(DataText), typeof(GameObject), true));
                    break;
                }//GameObject OR Object ?
            case SerializedPropertyType.LayerMask:
                {
                    DataText = Rapping(EditorGUI.LayerField(rect, title, UnRapping<LayerMask>(DataText)));
                    break;
                }
            case SerializedPropertyType.Enum:
                {/*
                    int LEnumIndex = UnRapping<int>(DataText);
                    var LEnum = (Enum)Enum.GetValues(LType).GetValue(LEnumIndex);//Enum

                    var Ldata = Enum.Parse(LType, EditorGUILayout.EnumPopup(title, LEnum, layoutOption).ToString());
                    LEnumIndex = (int)Convert.ChangeType(Ldata, typeof(int));//Enum - EnumType => int

                    if (LEnumIndex >= 0)
                    {
                        DataText = Rapping<int>(LEnumIndex);
                    }*/
                    //EditorGUI.LabelField(rect, "Add To Script");
                    break;
                }
            case SerializedPropertyType.Vector2:
                {
                    DataText = Rapping(EditorGUI.Vector2Field(rect, title, UnRapping<Vector2>(DataText)));
                    break;
                }
            case SerializedPropertyType.Vector3:
                {
                    DataText = Rapping(EditorGUI.Vector3Field(rect, title, UnRapping<Vector3>(DataText)));
                    break;
                }
            case SerializedPropertyType.Vector4:
                {
                    DataText = Rapping(EditorGUI.Vector4Field(rect, title, UnRapping<Vector4>(DataText)));
                    break;
                }
            case SerializedPropertyType.Rect:
                {
                    DataText = Rapping(EditorGUI.RectField(rect, title, UnRapping<Rect>(DataText)));
                    break;
                }
            #endregion
            case SerializedPropertyType.ArraySize:
                {
                    //for (int i = 0; i < DataPropSlot.arraySize; i++)
                    {
                        //DataField(DataPropSlot, TypePropSlot, collectionList, i);
                    }
                    EditorGUI.LabelField(rect, "Add To Script");
                    break;
                }//-------------아직 지원X // Add To Script
            case SerializedPropertyType.Character:
                {
                    //??먼지 모르겠음
                    EditorGUI.LabelField(rect, "Not Support");
                    break;
                }//Not Support
            case SerializedPropertyType.AnimationCurve:
                {
                    DataText = Rapping(EditorGUI.CurveField(rect, title, UnRapping<AnimationCurve>(DataText)));
                    break;
                }
            case SerializedPropertyType.Bounds:
                {
                    DataText = Rapping(EditorGUI.BoundsField(rect, UnRapping<Bounds>(DataText)));
                    break;
                }
            case SerializedPropertyType.Gradient:
                {
                    DataText = Rapping(EditorGUI.GradientField(rect, title, UnRapping<Gradient>(DataText)));
                    break;
                }
            case SerializedPropertyType.Quaternion:
                {
                    DataText = Rapping(EditorGUI.Vector4Field(rect, title, UnRapping<Vector4>(DataText)));
                    break;
                }
            case SerializedPropertyType.ExposedReference:
            case SerializedPropertyType.FixedBufferSize:
                {
                    EditorGUI.LabelField(rect, "Not Support");
                    break;
                }
            case SerializedPropertyType.Vector2Int:
                {
                    DataText = Rapping(EditorGUI.Vector2IntField(rect, title, UnRapping<Vector2Int>(DataText)));
                    break;
                }
            case SerializedPropertyType.Vector3Int:
                {
                    DataText = Rapping(EditorGUI.Vector3IntField(rect, title, UnRapping<Vector3Int>(DataText)));
                    break;
                }
            case SerializedPropertyType.RectInt:
                {
                    DataText = Rapping(EditorGUI.RectIntField(rect, title, UnRapping<RectInt>(DataText)));
                    break;
                }
            case SerializedPropertyType.BoundsInt:
                {
                    DataText = Rapping(EditorGUI.BoundsIntField(rect, title, UnRapping<BoundsInt>(DataText)));
                    break;
                }
            case SerializedPropertyType.ManagedReference:
                {
                    EditorGUI.LabelField(rect, "Not Support");
                    break;
                }//Not Support
            default:
                {
                    EditorGUI.TextArea(rect, "Unknown Type");
                    break;
                }
        }

        if (TypeEnum == SerializedPropertyType.Generic)
        {
            if (LType.IsArray)
            {
                //Debug.Log("---Array | " + ConvertTypeAssmbly(TypePropSlot.stringValue).GetElementType());
                EditorGUI.LabelField(rect, ConvertType(TypePropSlot.stringValue).GetElementType() + "[] -  Not Support");
                //DataField();//Type[] 형을 UnRapping ,,, 항상 이게 문제인데

            }//힘들어 너무 노가다야
            else
            if (LType.IsEnum)
            {
                int LEnumIndex = UnRapping<int>(DataPropSlot.stringValue);
                if (LEnumIndex >= 0 && LEnumIndex < Enum.GetValues(LType).Length)
                {
                    var LEnum = (Enum)Enum.GetValues(LType).GetValue(LEnumIndex);//Enum

                    var Ldata = Enum.Parse(LType, EditorGUI.EnumPopup(rect, title, LEnum).ToString());
                    LEnumIndex = (int)Convert.ChangeType(Ldata, typeof(int));//Enum - EnumType => int

                    if (LEnumIndex >= 0)
                    {
                        DataText = Rapping<int>(LEnumIndex);
                    }
                }
                else
                {
                    Debug.LogError(LEnumIndex);
                }
            }
            else
            if (LType.IsClass)
            {
                if (DataPropSlot.isArray)//List
                {
                    EditorGUI.LabelField(rect, "List is Not Support");
                    //EditorGUI.PropertyField(rect, DataPropSlot);//Test

                    //ConvertTypeAssmbly(TypePropSlot.stringValue).
                    Debug.Log(ConvertType(TypePropSlot.stringValue));//이름에서 [] 안 FullName 추출해서 다시 Switch로...
                }//List is Not Support
            }//리스트형은 엄청난 노가다로 할순 있긴한데 , 클래스는.....못할듯
            else
            {
                EditorGUI.LabelField(rect, "Not Support / Generic");
            }//Not Support
        }

        DataPropSlot.stringValue = DataText;

    }//배열 , 클래스, 구조체 구현

    public string DataField(string TypeFullName, string DataText, int index, string LabelText = "Index ", bool ErrorField = true, params GUILayoutOption[] layoutOption)
    {
        Type LType = ConvertType(TypeFullName);
        SerializedPropertyType TypeEnum = SerializedPropertyType.Generic;
        if (LType != null)
            TypeEnum = ConvertTypeEnum(LType.Name);

        string title = "";
        if (!string.IsNullOrEmpty(LabelText))
            title = LabelText + index;

        //layoutOption = new GUILayoutOption[] { GUILayout.Width(200) };

        switch (TypeEnum)
        {
            case SerializedPropertyType.Generic:
                if (ErrorField)
                {
                    EditorGUILayout.LabelField("Not Support / Generic");
                }
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
                {/*
                    int LEnumIndex = UnRapping<int>(DataText);
                    var LEnum = (Enum)Enum.GetValues(LType).GetValue(LEnumIndex);//Enum

                    var Ldata = Enum.Parse(LType, EditorGUILayout.EnumPopup(title, LEnum, layoutOption).ToString());
                    LEnumIndex = (int)Convert.ChangeType(Ldata, typeof(int));//Enum - EnumType => int

                    if (LEnumIndex >= 0)
                    {
                        DataText = Rapping<int>(LEnumIndex);
                    }*/
                    if (ErrorField)
                        EditorGUILayout.LabelField("Add To Script");
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
                    if (ErrorField)
                        EditorGUILayout.LabelField("Add To Script");
                    break;
                }//-------------아직 지원X // Add To Script
            case SerializedPropertyType.Character:
                {
                    //??먼지 모르겠음
                    if (ErrorField)
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
                {
                    if (ErrorField)
                        EditorGUILayout.LabelField("Not Support");
                    break;
                }
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
                    if (ErrorField)
                        EditorGUILayout.LabelField("Not Support");
                    break;
                }//Not Support
            default:
                {
                    if (ErrorField)
                        EditorGUILayout.TextArea("Unknown Type");
                    break;
                }
        }
        return DataText;
    }

    void DrawListItems(Rect rect, int index, bool isActive, bool isfocused)// CreateDataField 통합 가능
    {
        var element = DataProp.GetArrayElementAtIndex(index);
        rect.height -= 4;
        rect.y += 2;
        //EditorGUI.PropertyField(rect, element);
        DataReOrderField(rect, index);
    }
    void DrawHeader(Rect rect)
    {
        string name = "Vaule";
        EditorGUI.LabelField(rect, name);
    }
}//PropertyDrawer

//Not Recommend