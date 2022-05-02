using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RuntimeUIBuilderController : MonoBehaviour
{
    public string labeltext = "Test";
    VisualTreeAsset source;
    VisualElement element;

    //https://docs.unity3d.com/kr/2021.2/Manual/UIBuilder.html

    void Start()
    {
        source = GetComponent<UIDocument>().visualTreeAsset;
        element = GetComponent<UIDocument>().rootVisualElement;
    }

    // Update is called once per frame
    void Update()
    {
        element.Q<Label>("TestLabel").text = labeltext;

        element.Q<Button>("first").clicked += first_clicked;
        element.Q<Button>("secon").clicked += secon_clicked;
    }

    private void secon_clicked()
    {
        Debug.Log("Secon");
    }

    private void first_clicked()
    {
        Debug.Log("First");
    }
}
