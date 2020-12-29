using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAlertDelegate : MonoBehaviour
{
    public AlertWindowScript AlertWindow;

    // Start is called before the first frame update
    void Start()
    {
        AlertWindow.Spawn("Testing", ChooseEvent, true);
    }

    void ChooseEvent(bool b)
    {
        AlertWindow.Spawn("Result : " + b, ChooseEvent, true);//update
    }

}
