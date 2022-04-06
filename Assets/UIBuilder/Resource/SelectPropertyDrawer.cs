using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class SelectPropertyDrawer : EditorWindow
{
    static EditorWindow wnd;

    //List<Map<string, VisualTreeAsset>.MapSlot> data = new();
    List<string> Data_Key = new();

    int Select = -1;

    bool inputing = false;
    string inputData = "";

    [MenuItem("Window/UI Toolkit/Select PropertyDrawer", priority = 10)]
    public static void ShowEditor()
    {
        wnd = GetWindow<SelectPropertyDrawer>();
        wnd.titleContent = new GUIContent("Select PropertyDrawer");

        wnd.minSize = new Vector2(450, 200);
    }
    public void CreateGUI()
    {
        rootVisualElement.Clear();

        {
            Data_Key = SaveLoad.Deserialized<List<string>>(PlayerPrefs.GetString(this.GetType().Name));

            if (Data_Key == null)
            {
                Data_Key = new();
            }
        }//Load Data

        {
            /*
List<string> Temp = new List<string> { "1", "2" };
var Lscroll = new ListView();
Lscroll.makeItem = () => new Label();
Lscroll.bindItem = (item, index) => 
{
    (item as Label).text = Temp[index];
};
Lscroll.itemsSource = Temp;

rootVisualElement.Add(Lscroll);
*/
        }//리스트 예제

        rootVisualElement.Add(new Label("Key : PropertyDrawe Type().Name \n Get Vaule : AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(PlayerPrefs.GetString(Type().Name)))"));

        var Lscroll = new ListView();
        Lscroll.makeItem = () => SlotElement(wnd);
        Lscroll.bindItem = (item, index) =>
        {
            item.Q<TextField>().value = Data_Key[index];
            item.Q<ObjectField>().value = PlayerPrefs.GetString(Data_Key[index]) == null ?
                null : AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(PlayerPrefs.GetString(Data_Key[index])));
        };
        Lscroll.itemsSource = Data_Key;
        Lscroll.onSelectedIndicesChange += ListSelectionIndexChange;//using System.Linq;써야 의미있게 사용가능
        //Lscroll.onSelectionChange += Lscroll_onSelectionChange;//using System.Linq;써야 의미있게 사용가능

        rootVisualElement.Add(Lscroll);

        {
            var slot = new GroupBox();
            var Add = new Button();
            var Remove = new Button();

            slot.style.height = 30;

            var splite = new TwoPaneSplitView(0, wnd == null ? 250 : wnd.position.width * 0.5f, TwoPaneSplitViewOrientation.Horizontal);
            slot.Add(splite);

            splite.Add(Add);
            splite.Add(Remove);
            splite.style.height = 30;
            //splite.style.justifyContent = new StyleEnum<Justify>(Justify.SpaceAround);

            Add.clicked += Add_clicked;
            Add.text = "+";
            Remove.clicked += Remove_clicked;
            Remove.text = "-";

            rootVisualElement.Add(slot);
        }//Draw Add , Remove Button
    }

    private void ListSelectionIndexChange(IEnumerable<int> index)
    {
        Select = index.First();
        //Debug.Log("Select : " + index.First() + "\n Count :  " + index.ToList().Count);
    }

    private void Remove_clicked()
    {
        if (Select >= 0)
        {
            if (Data_Key.FindAll(t => string.Equals(t, Data_Key[Select])).Count == 1)
            {
                PlayerPrefs.DeleteKey(Data_Key[Select]);
            }

            Data_Key.RemoveAt(Select);
            PlayerPrefs.SetString(this.GetType().Name, SaveLoad.Serialized(Data_Key));

            CreateGUI();
        }
    }

    private void Add_clicked()
    {
        Data_Key.Add("");
        PlayerPrefs.SetString(this.GetType().Name, SaveLoad.Serialized(Data_Key));

        CreateGUI();
    }

    public VisualElement SlotElement(EditorWindow window)
    {
        var slot = new GroupBox();
        var slot_key = new TextField();
        var slot_vaule = new ObjectField();

        var splite = new TwoPaneSplitView(0, window == null ? 250 : window.position.width * 0.5f, TwoPaneSplitViewOrientation.Horizontal);
        slot.Add(splite);

        splite.Add(slot_key);
        splite.Add(slot_vaule);
        splite.style.height = 30;

        slot_vaule.objectType = typeof(VisualTreeAsset);
        slot_vaule.allowSceneObjects = false;

        slot_key.RegisterValueChangedCallback (EditedTextFeid);
        slot_vaule.RegisterValueChangedCallback (EditedObjField);

        slot_key.RegisterCallback<FocusOutEvent>(ListKeyCallback);//포커스를 잃을때 값변경 적용
        //입력후 포커스 잃을때도 작동해서....

        return slot;
    }
    private void EditedTextFeid(ChangeEvent<string> evt)
    {
        inputData = evt.newValue;
        inputing = true;
    }//Input TextField
    void ListKeyCallback(FocusOutEvent evt)
    {
        //Debug.Log("Try Edit Data[" + Select + "] / => " + inputData + "\n Inputing : " + inputing);
        if (inputing)
        {
            Data_Key[Select] = inputData;

            PlayerPrefs.SetString(this.GetType().Name, SaveLoad.Serialized(Data_Key));

            if (PlayerPrefs.HasKey(Data_Key[Select]) == false)
            {
                PlayerPrefs.SetString(Data_Key[Select], "");
            }
        }
        inputData = "";
        inputing = false;
    }//End Input
    void EditedObjField(ChangeEvent<Object> evt)
    {
        //Key : Data_Key[Select] , Vaule : GUID

        if (Select >= 0 && Select < Data_Key.Count)
        {
            if (evt.newValue == null)
            {
                PlayerPrefs.SetString(Data_Key[Select], "");
            }
            else
            {
                PlayerPrefs.SetString(Data_Key[Select], AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(evt.newValue)));
            }
        }
    }//Changed 

}
