using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static VariableCollection;

[System.Serializable]
public class VariableCollection
{
    public static Type ConvertType(string TypeFullName)
    {
        if(string.IsNullOrEmpty(TypeFullName))
        {
            return null;
        }else
        {
            // null 반환 없이 Type이 얻어진다면 얻어진 그대로 반환.
            var type = Type.GetType(TypeFullName);
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
                    type = assembly.GetType(TypeFullName);
                    if (type != null)
                        return type;
                }
            }
        }

        // 못 찾았음;;; 클래스 이름이 틀렸던가, 아니면 알 수 없는 문제 때문이겠지...
        return null;
    }//if Convert Error Find Assmbly

    public static SerializedPropertyType ConvertTypeEnum(string TypeName)
    {
        SerializedPropertyType TypeEnum = SerializedPropertyType.Generic;
        if(string.IsNullOrEmpty(TypeName))
        {
            return SerializedPropertyType.Generic;
        }

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
        if(typeof(Wrap<T>) != null && !string.IsNullOrEmpty(vaule))
        {
            return JsonUtility.FromJson<Wrap<T>>(vaule).Get;
        }
        else
        {
            return default;
        }
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

}//계산만

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
}//이렇게 감싸주면 배열,리스트 등을 직렬화가능 //https://birthbefore.tistory.com/11 이걸로 커스텀 직렬화

[System.Serializable]
public class VarCollection
{
    public string Data;
    public string DataType;

    public T Get<T>()
    {
        if (typeof(T) == ConvertType(DataType))
        {
            return JsonUtility.FromJson<Wrap<T>>(Data).Get;
        }
        else
        {
            Debug.Log("Not Equal GetType , DataType");
            return default;
        }
    }
    public bool Set<T>(T vaule, bool Force = false)
    {
        Wrap<T> wrap = new Wrap<T>(vaule);
        string data = JsonUtility.ToJson(wrap);

        if(string.IsNullOrEmpty(DataType) || Force)
        {
            Data = data;
            DataType = typeof(T).FullName;
            return true;
        }
        else
        {
            if (typeof(T) == ConvertType(DataType))
            {
                Data = data;
                DataType = typeof(T).FullName;
                return true;
            }
            else
            {
                Debug.Log("Not Equal SetType , DataType");
                return false;
            }
        }
    }
    public void ForceSet(string TypeName, string VauleText)
    {
        Data = VauleText;
        DataType = TypeName;
    }
    public string[] ForceGet()
    {
        return new string[] { DataType, Data };
    }

    public Type GetDataType()
    {
        return VariableCollection.ConvertType(DataType);
    }
    public SerializedPropertyType GetDataTypeEnum()
    {
        return ConvertTypeEnum(DataType);
    }
}

[CustomPropertyDrawer(typeof(VarCollection))]
public class VarCollectionProperty : PropertyDrawer
{
    SerializedProperty DataProp;
    SerializedProperty TypeProp;

    SerializedPropertyType TypeEnum;
    string DataVaule;

    Rect rect = new Rect();
    float DrawPos = 0;
    float fontSize = 7;
    float Space = 10;
    float EnumOffset = 20;
    float Height = 20;

    public Rect AddRect(Rect Lrect, Vector2 pos ,Vector2 size)
    {
        return new Rect(Lrect.position + pos, size);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);

        DataProp = property.FindPropertyRelative("Data");
        TypeProp = property.FindPropertyRelative("DataType");

        {
            if (TypeProp != null)
            {
                Type LType = ConvertType(TypeProp.stringValue);
                if (LType != null)
                    TypeEnum = ConvertTypeEnum(LType.Name);
                else
                    TypeEnum = SerializedPropertyType.Generic;
            }
            else
            {
                TypeEnum = SerializedPropertyType.Generic;
            }

            if (DataProp != null)
            {
                DataVaule = DataProp.stringValue;
            }
            else
            {
                DataVaule = "";
            }
        }//Set TypeEnum, DataVaule

        EditorGUILayout.BeginHorizontal();

        {
            DrawPos = property.name.Length * fontSize;
            rect = AddRect(position, new Vector2(0, 0), new Vector2(DrawPos, Height));
            EditorGUI.LabelField(rect, property.name);

            rect = AddRect(position, new Vector2(DrawPos, 0), new Vector2(TypeEnum.ToString().Length * fontSize + EnumOffset, Height));
            TypeEnum = (SerializedPropertyType)EditorGUI.EnumPopup(rect, TypeEnum);
            DrawPos += TypeEnum.ToString().Length * fontSize + EnumOffset + Space;

            {
                if (ConvertType(TypeProp.stringValue) != null)
                {
                    if (TypeEnum == SerializedPropertyType.Generic || ConvertTypeEnum(ConvertType(TypeProp.stringValue).Name) == SerializedPropertyType.Generic)
                    {
                        TypeProp.stringValue = ConvertTypeName(TypeEnum);
                    }else if (TypeEnum != ConvertTypeEnum(ConvertType(TypeProp.stringValue).Name))
                    {
                        Debug.LogWarning("if Change Type , Type >> Generic >> Type");
                    }
                }
                else
                {
                    TypeProp.stringValue = ConvertTypeName(TypeEnum);
                }
            }//TypeProp Update for Generic

            rect = AddRect(position, new Vector2(DrawPos, 0), new Vector2(position.width - DrawPos, Height));
            DataVaule = DataField(TypeProp.stringValue, DataProp.stringValue, rect, "", true);
            DataProp.stringValue = DataVaule;

        }//Update TypeProp & DataProp

        EditorGUILayout.EndHorizontal();
    }
    public string DataField(string TypeFullName, string DataText, string LabelText = " ", bool ErrorField = true, params GUILayoutOption[] layoutOption)
    {
        Type LType = ConvertType(TypeFullName);
        SerializedPropertyType LTypeEnum = SerializedPropertyType.Generic;
        if (LType != null)
            LTypeEnum = ConvertTypeEnum(LType.Name);
        

        string title = "";
        if (!string.IsNullOrEmpty(LabelText))
            title = LabelText;

        //layoutOption = new GUILayoutOption[] { GUILayout.Width(200) };

        switch (LTypeEnum)
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

    public string DataField(string TypeFullName, string DataText, Rect Lrect, string LabelText = " ", bool ErrorField = true)
    {
        Type LType = ConvertType(TypeFullName);
        SerializedPropertyType LTypeEnum = SerializedPropertyType.Generic;
        if (LType != null)
            LTypeEnum = ConvertTypeEnum(LType.Name);


        string title = "";
        if (!string.IsNullOrEmpty(LabelText))
            title = LabelText;

        //layoutOption = new GUILayoutOption[] { GUILayout.Width(200) };

        switch (LTypeEnum)
        {
            case SerializedPropertyType.Generic:
                if (ErrorField)
                {
                    EditorGUI.LabelField(Lrect, "Not Support / Generic");
                }
                break;
            #region done
            case SerializedPropertyType.Integer:
                {
                    //LProp.intValue = EditorGUILayout.IntField(LProp.intValue);
                    //collectionList.Set<int>(index, LProp.intValue);

                    //LProp.stringValue = VariableCollection.Rapping(EditorGUILayout.IntField(VariableCollection.UnRapping<int>(LProp.stringValue)));
                    DataText = Rapping(EditorGUI.IntField(Lrect, title, UnRapping<int>(DataText)));
                    break;
                }
            case SerializedPropertyType.Boolean:
                {
                    DataText = Rapping(EditorGUI.Toggle(Lrect, title, UnRapping<bool>(DataText)));
                    break;
                }
            case SerializedPropertyType.Float:
                {
                    DataText = Rapping(EditorGUI.FloatField(Lrect, title, UnRapping<float>(DataText)));
                    break;
                }
            case SerializedPropertyType.String:
                {
                    DataText = Rapping(EditorGUI.TextField(Lrect, title, UnRapping<string>(DataText)));
                    break;
                }
            case SerializedPropertyType.Color:
                {
                    DataText = Rapping(EditorGUI.ColorField(Lrect, title, UnRapping<Color>(DataText)));
                    break;
                }
            case SerializedPropertyType.ObjectReference:
                {
                    DataText = Rapping(EditorGUI.ObjectField(Lrect, title, UnRapping<GameObject>(DataText), typeof(GameObject), true));
                    break;
                }//GameObject OR Object ?
            case SerializedPropertyType.LayerMask:
                {
                    DataText = Rapping(EditorGUI.LayerField(Lrect, title, UnRapping<LayerMask>(DataText)));
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
                        EditorGUI.LabelField(Lrect, "Add To Script");
                    break;
                }
            case SerializedPropertyType.Vector2:
                {
                    DataText = Rapping(EditorGUI.Vector2Field(Lrect, title, UnRapping<Vector2>(DataText)));
                    break;
                }
            case SerializedPropertyType.Vector3:
                {
                    DataText = Rapping(EditorGUI.Vector3Field(Lrect, title, UnRapping<Vector3>(DataText)));
                    break;
                }
            case SerializedPropertyType.Vector4:
                {
                    DataText = Rapping(EditorGUI.Vector4Field(Lrect, title, UnRapping<Vector4>(DataText)));
                    break;
                }
            case SerializedPropertyType.Rect:
                {
                    DataText = Rapping(EditorGUI.RectField(Lrect, title, UnRapping<Rect>(DataText)));
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
                        EditorGUI.LabelField(Lrect, "Add To Script");
                    break;
                }//-------------아직 지원X // Add To Script
            case SerializedPropertyType.Character:
                {
                    //??먼지 모르겠음
                    if (ErrorField)
                        EditorGUI.LabelField(Lrect, "Not Support");
                    break;
                }//Not Support
            case SerializedPropertyType.AnimationCurve:
                {
                    DataText = Rapping(EditorGUI.CurveField(Lrect, title, UnRapping<AnimationCurve>(DataText)));
                    break;
                }
            case SerializedPropertyType.Bounds:
                {
                    DataText = Rapping(EditorGUI.BoundsField(Lrect, title, UnRapping<Bounds>(DataText)));
                    break;
                }
            case SerializedPropertyType.Gradient:
                {
                    DataText = Rapping(EditorGUI.GradientField(Lrect, title, UnRapping<Gradient>(DataText)));
                    break;
                }
            case SerializedPropertyType.Quaternion:
                {
                    DataText = Rapping(EditorGUI.Vector4Field(Lrect, title, UnRapping<Vector4>(DataText)));
                    break;
                }
            case SerializedPropertyType.ExposedReference:
            case SerializedPropertyType.FixedBufferSize:
                {
                    if (ErrorField)
                        EditorGUI.LabelField(Lrect, "Not Support");
                    break;
                }
            case SerializedPropertyType.Vector2Int:
                {
                    DataText = Rapping(EditorGUI.Vector2IntField(Lrect, title, UnRapping<Vector2Int>(DataText)));
                    break;
                }
            case SerializedPropertyType.Vector3Int:
                {
                    DataText = Rapping(EditorGUI.Vector3IntField(Lrect, title, UnRapping<Vector3Int>(DataText)));
                    break;
                }
            case SerializedPropertyType.RectInt:
                {
                    DataText = Rapping(EditorGUI.RectIntField(Lrect, title, UnRapping<RectInt>(DataText)));
                    break;
                }
            case SerializedPropertyType.BoundsInt:
                {
                    DataText = Rapping(EditorGUI.BoundsIntField(Lrect, title, UnRapping<BoundsInt>(DataText)));
                    break;
                }
            case SerializedPropertyType.ManagedReference:
                {
                    if (ErrorField)
                        EditorGUI.LabelField(Lrect, "Not Support");
                    break;
                }//Not Support
            default:
                {
                    if (ErrorField)
                        EditorGUI.TextArea(Lrect, "Unknown Type");
                    break;
                }
        }
        return DataText;
    }
}//if Change Type , Type >> Generic >> Type