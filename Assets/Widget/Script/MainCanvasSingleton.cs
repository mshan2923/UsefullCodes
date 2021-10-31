using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasSingleton : MonoBehaviour
{
    Canvas _mainCanvas;
    public Canvas MainCanvas
    {
        get
        {
            if (_mainCanvas == null)
                _mainCanvas = gameObject.GetComponent<Canvas>();

            return _mainCanvas;
        }

        set => _mainCanvas = value;
    }
    static MainCanvasSingleton _instance;
    public static MainCanvasSingleton Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }else
            {
                _instance = FindObjectOfType<Canvas>().gameObject.AddComponent<MainCanvasSingleton>();
                return _instance;
            }
        }
    }
    void Start()
    {
        MainCanvas = gameObject.GetComponent<Canvas>();
    }

}
