using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjPoolSpawner : MonoBehaviour
{
    public ObjectPool objectPool;
    public int ObjIndex = 0;
    public float Delay = 0.1f;
    public int SpawnAmount = 1;
    public Text SpawnCount;

    public Vector2 RandomForce;
    void Start()
    {
        StartCoroutine(SpawnDelay());
    }
    private void OnEnable()
    {
        
    }
    IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(Delay);

        SpawnCount.text = "Active Object : " + objectPool.ArrayPool.Get(ObjIndex).ActivePoolAmount;

        for (int i = 0; i < SpawnAmount; i++)
        {
            var Lobj = objectPool.GetPool(ObjIndex);
            Lobj.transform.position = gameObject.transform.position;
            Lobj.GetComponent<Rigidbody>().AddForce(Random.insideUnitCircle * RandomForce);
        }

        StartCoroutine(SpawnDelay());
    }
}
