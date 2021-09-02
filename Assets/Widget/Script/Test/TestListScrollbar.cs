using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ListScroll))]
public class TestListScrollbar : MonoBehaviour
{
    ListScroll listScroll;
    public GameObject SlotObject;

    public Button AddButton;
    public Button RemoveButton;

    void Start()
    {
        listScroll = gameObject.GetComponent<ListScroll>();

        AddButton.onClick.AddListener(AddEvent);
        RemoveButton.onClick.AddListener(RemoveEvent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddEvent()
    {
        var obj = GameObject.Instantiate(SlotObject);
        obj.GetComponentInChildren<Text>().text = listScroll.ScrollList.Count + "";

        listScroll.Add(obj);
    }
    void RemoveEvent()
    {
        listScroll.Remove(listScroll.ScrollList.Count - 1);
    }
}
