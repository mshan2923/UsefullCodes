using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    public IntMap<string> IntMap = new IntMap<string>();
    IntMap<string>.Vaule vaule; 

    void Start()
    {
        vaule.key = 2;
        vaule.vaule = "****";

        IntMap.Add(0,"--");
        IntMap.Add(1, "+++");
        IntMap += vaule;

        print(IntMap.GetKey(1).vaule);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
