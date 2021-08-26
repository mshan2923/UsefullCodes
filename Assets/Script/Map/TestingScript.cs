using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    public IntMap<string> IntMap = new IntMap<string>();
    IntMap<string>.Vaule vaule;

    public Map<string> Map = new Map<string>();
    public Map<VarCollection, float> Map_KV = new Map<VarCollection, float>();
    public GroupMap<string> GroupMap = new GroupMap<string>();

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
