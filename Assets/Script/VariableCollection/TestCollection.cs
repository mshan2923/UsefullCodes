using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollection : MonoBehaviour
{
    VariableCollection.CollectionList collection = new VariableCollection.CollectionList();
    // Start is called before the first frame update
    void Start()
    {
        collection.Add<int>(10);

        print(collection.Get<int>(0));
    }

    // Update is called once per frame
    void Update()
    {
        print(collection.Get<int>(0));
    }
}
