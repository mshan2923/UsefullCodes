using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TestInputWidget : MonoBehaviour
{
    public InputDetect InputDetectWidget;


    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(PressButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PressButton()
    {
        var Temp = GameObject.Instantiate(InputDetectWidget);
        Temp.transform.SetParent(gameObject.transform);
        Temp.GetComponent<InputDetect>().SelectedEvent += new InputDetect.DelegateSelected(SelectKey);
    }

    void SelectKey(KeyCode key, GameObject sender)
    {
        Debug.Log(key);
    }
}
