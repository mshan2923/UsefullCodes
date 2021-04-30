using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPoolKillVoulme : MonoBehaviour
{
    public ObjectPool objectPool;
    public int ObjectIndex = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Trigger : " + other.gameObject);
        objectPool.ReturnPool(ObjectIndex, other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject);
        objectPool.ReturnPool(ObjectIndex, collision.gameObject);
    }
}
