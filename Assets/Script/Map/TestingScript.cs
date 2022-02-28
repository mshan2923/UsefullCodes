using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    [System.Serializable]
    public class TestData
    {
        public string name;
        public float data;
    }

    //[Expand.AttributeLabel("", true, true)]//===========아니 씨벌 IntMap 구현에 영향
    public IntMap<string> IntMap = new IntMap<string>();
    IntMap<string>.Vaule vaule;

    public Map<Wrap<Rect>> map = new();//===================TestData을 쓰면 초기화가 안되나봄..?
    public Map<VarCollection, float> Map_KV = new();
    public GroupMap<string> GroupMap = new GroupMap<string>();

    public Map<LayerMask, float> Map_KV2 = new();
    public Map<string, Rect> Map_KV3 = new();

    void Start()
    {
        vaule.key = 2;
        vaule.vaule = "****";

        IntMap.Add(0,"--");
        IntMap.Add(1, "+++");
        IntMap += vaule;

        print(IntMap.GetKey(1).vaule);


        GroupMap.Add("Test", 5);
        GroupMap.Set(0, 10);
        GroupMap.Remove(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
