using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ObjectPoolSturt
{
    public string Title = "";
    public GameObject m_Preperb;
    public Queue<GameObject> PoolObject;

    [Tooltip("0 is No Limit")] //ToolTip Maximum
    public int MaximumActivePool;
    public int PoolAmount;
    public int ActivePoolAmount;

    public List<GameObject> ActivePoolObj = new List<GameObject>();

    public ObjectPoolSturt(int maxAP = -1, int poolAmount = -1, int activePoolObj = -1)
    {
        m_Preperb = null;
        PoolObject = new Queue<GameObject>();
        MaximumActivePool = maxAP;
        PoolAmount = poolAmount;
        ActivePoolAmount = activePoolObj;
    }
    public void CreateQueue()
    {
        PoolObject = new Queue<GameObject>();
    }
    public void Add_MaxAP(int Vaule = 1)
    {
        MaximumActivePool += Vaule;
    }
    public void Add_PoolAmount(int Vaule = 1)
    {
        PoolAmount += Vaule;
    }
    public void Add_ActivePool(int Vaule = 1)
    {
        ActivePoolAmount += Vaule;
    }
}

public class ObjectPool : MonoBehaviour
{
    //public ObjectPool objectPool;//Remove Static
    [SerializeField]
    public List<GameObject> ManagmentPool = new List<GameObject>();//Remove Static

    public bool SaveActiveObj = false;

    [SerializeField]
    public Map<ObjectPoolSturt> ArrayPool = new Map<ObjectPoolSturt>();//Map 은 IntMap에 있는 List<>를 더 편하게 쓰기위해 만듬

    private void OnEnable()
    {

    }
    void Awake()
    {
        //objectPool = this;

        for (int i = 0; i < ArrayPool.Length; i++)
        {
            ManagmentPool.Add(null);
        }
    }
    private void Start()
    {
        for (int i = 0; i < ArrayPool.Length; i++)
        {
            ArrayPool.Get(i).CreateQueue();
            //ArrayPool[i].PoolObject = new Queue<GameObject>();

            //CreatePool(i, ArrayPool.Get(i).PoolAmount);
        }
    }

    public GameObject GetManagment(int index)
    {
        if(ManagmentPool.Count > index)
        {
            return ManagmentPool[index];
        }else
        {
            return null;
        }

    }

    public void CreatePool(int index , int Amount)
    {
        if (ArrayPool.Length> index)
        {
            for (int i = 1; i <= Amount; i++)
            {
                GameObject SpawnedPool = Instantiate(ArrayPool.Get(index).m_Preperb, Vector3.zero, Quaternion.identity);
                if(ArrayPool.Get(index).PoolObject == null)
                {
                    Debug.LogWarning("PoolObj is null // "+ ArrayPool.Get(index).PoolAmount);
                }
                ArrayPool.Get(index).PoolObject.Enqueue(SpawnedPool);
                SpawnedPool.SetActive(false);
                SpawnedPool.transform.SetParent(this.transform);
            }
        }
    }
    public GameObject ForceAddPool(int index)
    {
        var results = new GameObject();
        results = Instantiate(ArrayPool.Get(index).m_Preperb, Vector3.zero, Quaternion.identity);
        results.SetActive(false);
        results.transform.SetParent(this.transform);

        ArrayPool.Get(index).Add_PoolAmount();

        return results;
    }

    public GameObject GetPool(int index)
    {
        if(ArrayPool.Length> index)
        {
            var PoolData = ArrayPool.Get(index);
            bool CanSpawn = PoolData.ActivePoolAmount < PoolData.MaximumActivePool || PoolData.MaximumActivePool <= 0;

            if (PoolData.PoolObject == null)
            {
                PoolData.PoolObject = new Queue<GameObject>();//Sometime Make Null problem
            }

            if(PoolData.PoolObject.Count == 0 && CanSpawn)//Need More Object
            {
                CreatePool(index, 1);
                ArrayPool.Get(index).Add_PoolAmount();
            }//Need More Object & if No Limit

            if (PoolData.ActivePoolAmount >= PoolData.MaximumActivePool && PoolData.MaximumActivePool > 0)
            {
                return null;
            }//Over MaxiumPool

            GameObject L_GO = ArrayPool.Get(index).PoolObject.Dequeue();
            L_GO.SetActive(true);


            ArrayPool.Get(index).Add_ActivePool();

            {
                if (SaveActiveObj)
                {
                    PoolData.ActivePoolObj.Add(L_GO);
                }
            }// ActivePoolObj추가

            if (L_GO == null)
            {
                if(ArrayPool.Get(index).PoolObject.Count == 0)
                {
                    return ForceAddPool(index);
                }
                else if(ArrayPool.Get(index).PoolAmount < ArrayPool.Get(index).MaximumActivePool)
                {
                    return ForceAddPool(index);
                }
            }//Queue 가 없거나 생성조건됐으나 Null 일때// 의도치 않은 버그처리
            return L_GO;
        }else
        {
            return null;
        }
    }

    public bool ReturnPool(int index, GameObject L_object)//인덱스를 제거후 instanceID로 찾고 반환
    {
        if (ArrayPool.Length> index)
        {
            if (ArrayPool.Get(index).m_Preperb.GetType() == L_object.GetType() && L_object.activeSelf)
            {
                ArrayPool.Get(index).PoolObject.Enqueue(L_object);
                L_object.transform.position = Vector3.zero;
                L_object.transform.rotation = Quaternion.identity;
                L_object.SetActive(false);

                //ArrayPool.Get(index).ActivePoolObj--;
                ArrayPool.Get(index).Add_ActivePool(-1);

                {
                    if (SaveActiveObj)
                    {
                        ArrayPool.Get(index).ActivePoolObj.Remove(L_object);
                    }
                }//Remove ActivePoolObject 
                return true;
            }else
            {
                return false;
            }
        }else
        {
            return false;
        }
    }

    public IEnumerator DelayReturnPool(int index,float DelayTime , GameObject L_object)
    {
        yield return new WaitForSeconds(DelayTime);

        ReturnPool(index, L_object);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ObjectPool))]
public class ObjectPool_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.HelpBox("Preperb Object Need 'PoolReceiveEvnet' for Receive DamageEvent & IDamageUnit", MessageType.Info);
    }
}
#endif