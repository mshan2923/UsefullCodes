using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class InputSystem : MonoBehaviour
{
    #region Singleton Prefab + Save/Load
    public const string SettingFileDirectory = "Assets/Resources";
    const string SettingFileName = "InputSystem";
    const string KeySaveFileName = "InputData";
    const string SaveExt = "setting";

    static InputSystem instance = null;

    public static InputSystem Instance
    {
        get
        {
            {
                /*
if(instance ==  null)
{
    GameObject obj = System.Activator.CreateInstance<GameObject>();
    instance = obj.AddComponent<InputSystem>();
    //DontDestroyOnLoad(obj);//�����Ϳ��� X
    obj.name = "InputSystemInstance";
}
return instance;*/
            }

            if (instance != null)
            {
                return instance;
            }
            string filePath = SettingFileDirectory + "/" + SettingFileName + ".prefab";

            //instance = Resources.Load<InputSystem>(filePath);//������Ʈ ������
            instance = GameObject.FindObjectOfType<InputSystem>();

#if UNITY_EDITOR
            if (instance == null)
            {
                GameObject obj = null;

                if (!AssetDatabase.IsValidFolder(SettingFileDirectory))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                //instance = AssetDatabase.LoadAssetAtPath<InputSystem>(filePath);

                if (AssetDatabase.LoadAssetAtPath<GameObject>(filePath) != null)
                {
                    obj = PrefabUtility.LoadPrefabContents(filePath);
                    instance = obj.GetComponent<InputSystem>();
                }

                if (instance == null)
                {
                    obj = System.Activator.CreateInstance<GameObject>();
                    instance = obj.AddComponent<InputSystem>();
                    obj.name = "InputSystemInstance";
                    //AssetDatabase.CreateAsset(obj, filePath);
                    PrefabUtility.SaveAsPrefabAsset(obj, filePath);
                }
            }
#endif
            return instance;
        }
    }//Singleton + ����ȭ �������� ���������� ����� ����

    [MenuItem("Asset/InputSystem")]
    public static void OpenInspector()
    {
        Selection.activeObject = Instance;
    }

    public void Save()
    {
        SaveLoad.Save(ButtonSingleAxis, SettingFileDirectory, "ButtonSingleAxis", SaveExt);
        SaveLoad.Save(ButtonVector2Axis, SettingFileDirectory, "ButtonVector2Axis", SaveExt);
        SaveLoad.Save(ActionEvent, SettingFileDirectory, "ActionEvent", SaveExt);
    }
    public bool Load()
    {
        int Loaded = 0;

        if (SaveLoad.Load(SettingFileDirectory, "ActionEvent", SaveExt, out ActionInput ActionData))
        {
            ActionEvent = ActionData;
            Loaded++;
        }

        if (SaveLoad.Load(SettingFileDirectory, "ButtonSingleAxis", SaveExt, out List<ButtonAxisInput> SingleData))
        {
            ButtonSingleAxis = SingleData;
            Loaded++;
        }

        if (SaveLoad.Load(SettingFileDirectory, "ButtonVector2Axis", SaveExt, out List<ButtonVector2AxisInput> VectorData))
        {
            ButtonVector2Axis = VectorData;
            Loaded++;
        }

        return Loaded == 3;
    }
    #endregion

    #region UnityEvent
    [System.Serializable]
    public class SingleEvent : UnityEvent<float>
    { 
    }
    [System.Serializable]
    public class Vector2Event : UnityEvent<Vector2>
    {
    }
    #endregion

    #region AxisInput Class


    public abstract class AxisInput<T>
    {
        public T State;
        public float PassVaule = 0.01f;
        public float Sensitivity = 1;
        public float MaxVaule = -1;

        [Header("Increase")]
        public float IncreaseRate = -1;//1�� (IncreaseRate * 100) % ��ŭ ����
        public float IncreaseLimit = -1;//1�ʿ� IncreaseLimit��ŭ ������
        public bool SlowerScaleDistance = true;

        [Header("Decrease")]
        public float DecreaseRate = -1;//���Һ��� - �̵��ѰŸ�(RelativeIncrement)�� N%��ŭ �̵� // ���Ӽӵ� => ���� ������
        public float DecreaseSpeed = -1;
        public bool BackToZero = false;
        public bool SmoothDecrease = false;

        [Space(10)]
        public bool Pressing = false;
        public T LastInputState;//���� ������ �Է�
        public T RelativeIncrement;// ��� �Է�

        public abstract T GetVaule(T vaule, bool Limit = true);
    }

    [System.Serializable]
    public class SingleAxisInput : AxisInput<float>
    {
        public SingleAxisInput()
        {
            PassVaule = 0.01f;
            Sensitivity = 1;
            MaxVaule = 1;

            IncreaseRate = -1;
            IncreaseLimit = -1;
            SlowerScaleDistance = true;

            DecreaseRate = -1;
            DecreaseSpeed = -1;
            BackToZero = false;
        }
        public override float GetVaule(float vaule, bool Limit = true)
        {
            if (Pressing == false && Mathf.Abs(vaule) >= PassVaule)
            {
                RelativeIncrement = 0;
            }//Start Pressing
            else if ((RelativeIncrement > 0 && vaule < 0) || (RelativeIncrement < 0 && vaule > 0))//RelativeIncrement �� vaule �� ��ȣ�� �ٸ��� 
            {
                RelativeIncrement = 0;
            }

            Pressing = Mathf.Abs(vaule) >= PassVaule;

            float temp = vaule * Sensitivity;

            if (Limit)
            {
                if (Pressing)
                {
                    float LInVaule = 0;
                    if (IncreaseRate > 0)
                    {
                        float Ldistance = SlowerScaleDistance ? MaxVaule - Mathf.Abs(LastInputState) : Mathf.Abs(RelativeIncrement);//SlowerScaleDistance ? �ִ� - ���� : ��밪
                        if (MaxVaule > 0)
                        {
                            Ldistance = Mathf.Min(MaxVaule, Ldistance);//��밪 �ִ�ġ
                        }
                        LInVaule = Mathf.Max(Mathf.Abs(temp), IncreaseLimit, Ldistance * IncreaseRate) * Time.deltaTime * (temp > 0 ? 1 : -1);

                    }//Percent Increase
                    else if (IncreaseLimit > 0)
                    {
                        LInVaule = Mathf.Max(Mathf.Abs(temp), IncreaseLimit * Sensitivity) * Time.deltaTime * (temp > 0 ? 1 : -1);

                    }//Linear Increase

                    if(IncreaseRate > 0 || IncreaseLimit > 0)
                    {
                        if (Mathf.Abs(State + LInVaule) <= Mathf.Abs(MaxVaule) || MaxVaule <= 0)
                        {
                            RelativeIncrement += LInVaule;
                            State += LInVaule;
                            LastInputState = State;
                        }
                        return State;
                    }//Return
                }
                else
                {
                    if (BackToZero)
                    {
                        if (DecreaseRate > 0)
                        {
                            if (Mathf.Abs(State) < PassVaule)
                            {
                                State = 0;
                                return 0;
                            }
                            else if (State > 0)
                            {
                                State -= Mathf.Max(PassVaule, DecreaseSpeed, (SmoothDecrease? State : LastInputState) * DecreaseRate) * Time.deltaTime;
                                State = Mathf.Max(State, 0);
                            }
                            else
                            {
                                State += Mathf.Max(PassVaule, DecreaseSpeed, Mathf.Abs(SmoothDecrease ? State : LastInputState) * DecreaseRate) * Time.deltaTime;
                                State = Mathf.Min(State, 0);
                            }

                            return State;
                            //Percent Decrease
                        }//Percent Decrease
                        else if (DecreaseSpeed > 0)
                        {
                            if (Mathf.Abs(State) < PassVaule)
                            {
                                State = 0;
                                return 0;
                            }
                            else if (State > 0)
                            {
                                State -= (Sensitivity * DecreaseSpeed * Time.deltaTime);
                                State = Mathf.Max(0, State);
                            }
                            else
                            {
                                State += (Sensitivity * DecreaseSpeed * Time.deltaTime);
                                State = Mathf.Min(0, State);
                            }

                            return State;
                            //Linear Decrease
                        }//Linear Decrease
                    }else
                    {
                        // �̵��ҰŸ� : RelativeIncrement * N% * (1 / ���Ҽӵ�) , �̵��ѰŸ� : State - LastInputState
                        // SmoothDecrease -> true : += (State - LastInputState) * (1 / DecreaseSpeed) / false : += LastInputState * (1 / DecreaseSpeed)

                        if (DecreaseRate > 0)
                        {
                            float MoveDistance = (RelativeIncrement * (1 / DecreaseRate));
                            float AddVaule = (SmoothDecrease ? Mathf.Abs(LastInputState + MoveDistance - State) * (1 / DecreaseRate) : Mathf.Abs(MoveDistance)) * Time.deltaTime
                                * (DecreaseSpeed > 0 ? DecreaseSpeed : 1);

                            if (Mathf.Abs(State - LastInputState) + AddVaule <= Mathf.Abs(MoveDistance))//�̵��ѰŸ� + �߰��� < �̵��Ÿ�
                            {
                                if (Mathf.Abs(State + AddVaule * (RelativeIncrement > 0 ? 1 : -1)) < MaxVaule)
                                    State += AddVaule * (RelativeIncrement > 0 ? 1 : -1);
                            }

                            return State;
                        }
                    }

                    //Decrease// -- BackToZero�� True�̸� - Percent Decrease : LastInputState * N%��ŭ�� ���� - Linear Decrease : State�� �ΰ��� * ���Ӽӵ� ��ŭ ���� 
                    //              BackToZero�� false��  -  �̵��ҰŸ� : RelativeIncrement * N% * (1 / ���Ҽӵ�) , �̵��ѰŸ� : State - LastInputState
                }
            }//������ ����


            State = temp;
            return temp;
        }
    }
    [System.Serializable]
    public class Vector2AxisInput : AxisInput<Vector2>
    {
        public Vector2AxisInput()
        {
            PassVaule = 0.01f;
            Sensitivity = 1;

            IncreaseRate = -1;
            IncreaseLimit = -1;

            DecreaseRate = -1;
            DecreaseSpeed = -1;
            BackToZero = false;
        }

        public bool RelativeInput = false;
        public bool QuickClamp = true;

        [Space(5)]
        public Vector2 InputDirection = Vector2.zero;
        public float InputDistance = 0;
        public float Distance = 0;
        float AddDistance;
        float RelativeDistance = 0;

        public override Vector2 GetVaule(Vector2 vaule, bool Limit = true)
        {
            {/*
                            Pressing = Vector2.SqrMagnitude(vaule) >= (PassVaule * PassVaule);

            Vector2 temp = vaule * Sensitivity;

            {
                if (Limit)
                {
                    if (Pressing)
                    {
                        if (IncreaseRate > 0)
                        {
                            State = temp.normalized * (Mathf.Max(PassVaule, IncreaseLimit, (IncreaseRate * temp.magnitude))) * Time.deltaTime;

                            LastInputState = State;
                            return State;
                        }
                        else if (IncreaseLimit > 0)
                        {
                            //State += Vector2.ClampMagnitude(temp, (IncreaseLimit * Time.deltaTime));

                            LastInputState = State;
                            State = temp.normalized * Sensitivity * IncreaseLimit * Time.deltaTime;

                            return State;
                        }
                        else
                        {
                            //Skip
                        }
                    }
                    else
                    {
                        {
                            Direction = State.normalized;
                            Distance = State.magnitude;//OR LastIncreasePoint.magnitude
                        }

                        if (State.sqrMagnitude < (PassVaule * PassVaule))
                        {
                            State = Vector2.zero;
                            return Vector2.zero;
                        }

                        if (BackToZero)
                        {
                            if (DecreaseRate > 0)
                            {
                                //�ּ� PassVaule OR DecreaseLimit��ŭ + Satate�� DecreaseRate��ŭ
                                State -= Direction * Mathf.Max(PassVaule, DecreaseSpeed, Distance * DecreaseRate) * Time.deltaTime;

                                return State;
                            }//Percent Decrease
                            else if (DecreaseSpeed > 0)
                            {
                                State -= Direction * (Sensitivity * DecreaseSpeed * Time.deltaTime);

                                return State;
                            }//Linear Decrease
                        }else
                        {
                            if (DecreaseRate > 0)
                            {

                            }
                            else if (DecreaseSpeed > 0)
                            {

                            }
                        }

                    }//Decrease// -- BackToZero�� True�̸� - Percent Decrease : LastIncreasePoint * N%��ŭ�� ���� - Linear Decrease : State�� N��ŭ ���� 
                     //BackToZero�� false�� - Percent Decrease : LastIncreasePoint * 1/N%��ŭ�� �߰��� ����  - Linear Decrease : ���� �ּҼӵ� (D_Rate�� ����� �ƴҶ� �̵��� �Ÿ� ��ŭ)
                     //++++++SmoothDecrease���� �����Ÿ��� N% OR �ִ��� N% ���� ����
                }
            }//������ ����

            State = temp;
            return temp;*/
            }//Disable

            if (Vector2.SqrMagnitude(vaule) >= (PassVaule * PassVaule))
            {
                if (Pressing == false)
                {
                    RelativeIncrement = Vector2.zero;
                }//�Է��� ���۵ɶ�

                Pressing = true;

                InputDirection = vaule.normalized;
                InputDistance = vaule.magnitude * Sensitivity;
                Distance = State.magnitude;
            }
            else
            {
                Pressing = false;

                InputDirection = Vector2.zero;
                InputDistance = 0;
                Distance = State.magnitude;
            }

            if (Limit)
            {
                RelativeDistance = RelativeIncrement.magnitude;

                if (Pressing)
                {
                    {
                        //Default : vaule.normalized
                        if (Distance < PassVaule || RelativeInput)
                        {
                            InputDirection = vaule.normalized;
                        }
                        else
                        {
                            if (Mathf.Approximately(Distance , MaxVaule))
                            {
                                InputDirection = vaule.normalized;
                            }
                            else
                            {

                                if (MaxVaule > 0)
                                {
                                    if (QuickClamp)
                                        InputDirection = ((vaule.normalized * Mathf.Max(Mathf.Min(MaxVaule, Distance * (1 + 1 / IncreaseRate)), InputDistance * Sensitivity)) - State).normalized;
                                    //Input.Normalized * (Distance + a) - State
                                    else
                                        InputDirection = ((vaule.normalized * MaxVaule) - State).normalized;
                                }
                                else
                                {
                                    InputDirection = ((vaule.normalized * Mathf.Max(Distance * (1 + 1 / IncreaseRate), InputDistance)) - State).normalized;
                                }
                            }
                        }
                    }//InputDirection

                    if (IncreaseRate > 0)
                    {
                        if (SlowerScaleDistance && MaxVaule > 0)
                        {
                            AddDistance = (Mathf.Max((MaxVaule - Distance), InputDistance)) * Time.deltaTime;
                        }//���°� �ݺ�� �ӵ�
                        else if (MaxVaule > 0)
                        {
                            AddDistance = Mathf.Max(Mathf.Min(MaxVaule , RelativeDistance), InputDistance) * Time.deltaTime;
                        }//����Է� ��� �ӵ� + ���밪 ����
                        else
                        {
                            AddDistance = Mathf.Max(RelativeDistance, InputDistance) * Time.deltaTime;
                        }

                        AddDistance *= IncreaseRate;
                    }
                    else if (IncreaseLimit > 0)
                    {
                        AddDistance = Mathf.Max(InputDistance, IncreaseLimit) * Time.deltaTime;
                    }

                    if (IncreaseRate > 0 || IncreaseLimit > 0)
                    {
                        if (Distance + AddDistance < MaxVaule || MaxVaule <= 0 )
                        {
                            State += AddDistance * InputDirection;
                            LastInputState = State;
                            RelativeIncrement += (AddDistance * vaule.normalized);//�Է¹��� * �߰��Ÿ� 
                        }
                        else
                        {
                            Vector2 Ltemp = Vector2.ClampMagnitude(State + AddDistance * InputDirection, MaxVaule);

                            RelativeIncrement += (Ltemp - State);

                            State = Ltemp;
                            LastInputState = State;
                        }

                        return State;
                    }
                }
                else
                {
                    if (BackToZero)
                    {
                        if (DecreaseRate > 0)
                        {
                            AddDistance = DecreaseRate * Time.deltaTime * -1 * Distance;
                        }
                        else if (DecreaseSpeed > 0)
                        {
                            AddDistance = (Sensitivity * DecreaseSpeed) * Time.deltaTime * -1;
                        }

                        if (DecreaseRate > 0 || DecreaseSpeed > 0)
                        {
                            if (Distance + AddDistance >= PassVaule * Sensitivity)
                            {
                                State += AddDistance * State.normalized;
                            }else
                            {
                                State = Vector2.zero;
                            }

                            return State;
                        }
                    }else
                    {
                        float MoveDistance = (1 / DecreaseRate) * RelativeDistance;
                        float LastInputDistance = LastInputState.magnitude;

                        if (SmoothDecrease)
                        {
                            //(�����Է� + �̵��Ÿ� - ���°�) * (1 / DecreaseRate)
                            AddDistance = (LastInputDistance + MoveDistance - Distance) * (1 / DecreaseRate) * Time.deltaTime;
                        }
                        else
                        {
                            AddDistance = MoveDistance * Time.deltaTime;
                        }
                        AddDistance *= DecreaseSpeed;

                        if (Distance > 0 && Mathf.Approximately(RelativeDistance, 0))
                        {
                            return State;
                        }//�ִ�ġ�� ���¿��� �Է½�

                        if (Distance + MoveDistance * Time.deltaTime <= LastInputDistance + MoveDistance)
                        {
                            if (MaxVaule > 0)
                            {
                                //State = Mathf.Min(MaxVaule, (Distance + MoveDistance * Time.deltaTime)) * RelativeIncrement.normalized;
                                State = Vector2.ClampMagnitude((State + AddDistance * RelativeIncrement.normalized), MaxVaule);
                            }else
                            {
                                State += AddDistance * RelativeIncrement.normalized;
                            }
                        }

                        return State;
                    }
                }//Decrease
            }

            State = vaule * Sensitivity;
            return State;
        }
    }

    [System.Serializable]
    public struct ButtonAxisInput
    {
        public string Name;
        public Map<KeyCode, float> InputVaule;
        public SingleAxisInput AxisInput;
        public SingleEvent Event;

        public float GetVaule()
        {
            return AxisInput.State;
        }
    }
    [System.Serializable]
    public struct ButtonVector2AxisInput
    {
        public string Name;
        public Map<KeyCode, Vector2> InputVaule;
        public Vector2AxisInput AxisInput;
        public Vector2Event Event;

        public Vector2 GetVaule()
        {
            return AxisInput.State;
        }
    }

    // �� �Է�      - ���콺, ���̽�ƽ, ������
    // ��ư �� �Է� - Map< �̸�, List<KeyCode , SingleAxisInput>>
    // �Ϲ� �Է�    - Map < �̸�, List<KeyCode, IsOR(��ư�Է��� OR ���� And ����)>>

    //Key�� ������ Const , �ΰ��ӿ��� Ű���� ���� , ����ȭ�ؼ� json���� ����
    #endregion

    #region ActionInput Class

    [System.Serializable]
    public struct ActionInputSlot
    {
        public string Name;
        public List<KeyCode> Buttons;
        public bool AndEvent;
        public UnityEvent Event;
        [Space(5)]
        public bool Result;

        public bool IsPress()
        {
            bool Temp = false;

            for (int i = 0; i < Buttons.Count; i++)
            {
                if (AndEvent)
                {
                    if (Input.GetKey(Buttons[i]))
                    {
                        Temp = true;
                    }else
                    {
                        Temp = false;
                        break;
                    }
                }else
                {
                    if (Input.GetKey(Buttons[i]))
                    {
                        Temp = true;
                        break;
                    }
                }
            }

            Result = Temp;
            return Temp;
        }
    }
    [System.Serializable]
    public class ActionInput
    {
        public List<ActionInputSlot> inputSlots = new List<ActionInputSlot>();

        public void Update()
        {
            for (int i = 0; i < inputSlots.Count; i++)
            {
                if (inputSlots[i].IsPress())
                {
                    if (inputSlots[i].Event != null)
                        inputSlots[i].Event.Invoke();
                }
            }
        }
    }
    #endregion


    public Vector2 MousePos;
    public Vector2AxisInput MouseOffset = new Vector2AxisInput();

    public SingleAxisInput MouseYScroll = new SingleAxisInput();

    [Header("Custom")]
    public List<ButtonAxisInput> ButtonSingleAxis = new List<ButtonAxisInput>();
    public List<ButtonVector2AxisInput> ButtonVector2Axis = new List<ButtonVector2AxisInput>();
    public ActionInput ActionEvent = new ActionInput();

    void Start()
    {
        if (Input.mousePresent)
            MousePos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        {
            /*
            if (Input.anyKey)
            {
                //Debug.Log(" Input.inputString : " + Input.inputString);//�Էµ� Ű�ν��� �Ϻθ���

                var Keylist = System.Enum.GetValues(typeof(KeyCode));

                for (int i = 0; i < Keylist.Length; i++)//Length : 326
                {

                    if (Input.GetKeyDown((KeyCode)Keylist.GetValue(i)))
                    {
                        Debug.Log(" Input.inputString : " + Input.inputString + " Press : " + Keylist.GetValue(i).ToString());//===�Ǳ��ѵ�...����ȭ!
                                                                                                                              //Ű���� �Է��� �̺�Ʈ���� ó�� , ���콺 �̳� �����е���� �̰ɷ�
                    }
                }
            }*/
        }//Test Detect Input Key

        if (Input.mousePresent)
        {

            MousePos += MouseOffset.GetVaule(new Vector2(Input.mousePosition.x, Input.mousePosition.y) - MousePos);

            MouseYScroll.GetVaule(Input.mouseScrollDelta.y);
        }
        
        for (int i = 0; i < ButtonSingleAxis.Count; i++)
        {
            float Lvaule = 0;
            for(int v = 0; v < ButtonSingleAxis[i].InputVaule.Count; v++)
            {
                if (Input.GetKey(ButtonSingleAxis[i].InputVaule.GetKey(v)))
                {
                    Lvaule += ButtonSingleAxis[i].InputVaule.GetVaule(v);
                }
            }

            if(ButtonSingleAxis[i].InputVaule.Count > 0)
            {
                ButtonSingleAxis[i].AxisInput.GetVaule(Lvaule);

                if (ButtonSingleAxis[i].Event != null)
                {
                    ButtonSingleAxis[i].Event.Invoke(ButtonSingleAxis[i].GetVaule());
                }
            }
        }//ButtonSingleAxis

        for (int i = 0; i < ButtonVector2Axis.Count; i++)
        {
            Vector2 Lvaule = Vector2.zero;
            for (int v = 0; v < ButtonVector2Axis[i].InputVaule.Count; v++)
            {
                if (Input.GetKey(ButtonVector2Axis[i].InputVaule.GetKey(v)))
                {
                    Lvaule += ButtonVector2Axis[i].InputVaule.GetVaule(v);
                }
            }

            if (ButtonVector2Axis[i].InputVaule.Count > 0)
            {
                ButtonVector2Axis[i].AxisInput.GetVaule(Lvaule);

                if (ButtonVector2Axis[i].Event != null)
                {
                    ButtonVector2Axis[i].Event.Invoke(ButtonVector2Axis[i].GetVaule());
                }
            }
        }//ButtonVector2Axis

        ActionEvent.Update();
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey)
        {
            //Debug.Log("Detected a keyboard event!" + e.keyCode);
        }
        else if (e.isMouse)
        {
            //Debug.Log("Mouse " + e.button + " Event");//Mouse 3, 4 �ν� X
        }
    }

#if UNITY_EDITOR
    public void TestEvent(Vector2 vaule)
    {
        print(vaule);
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(InputSystem))]
public class InputSystemEditor : Editor
{
    InputSystem Onwer;
    KeyCode LastKey;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Onwer = target as InputSystem;

        if (Event.current.keyCode != KeyCode.None)
        {
            LastKey = Event.current.keyCode;
        }
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Button("Update Last Input Key");
            GUILayout.Label(" : " + LastKey);
            EditorGUILayout.EndHorizontal();
        }

        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                Onwer.Save();
            }
            if (GUILayout.Button("Load"))
            {
                Onwer.Load();
            }
            EditorGUILayout.EndHorizontal();

            //List<ButtonAxisInput> ButtonSingleAxis
            //List<ButtonVector2AxisInput> ButtonVector2Axis
        }
    }
}
#endif