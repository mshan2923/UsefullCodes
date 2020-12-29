using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertWindowScript : MonoBehaviour
{
    public delegate void ChooseEvent(bool b);
    ChooseEvent choose;

    public Text C_Text;
    public Button Cencle;
    public Button Apply;

    public bool AutoDeactive = false;

    void Start()
    {
        Cencle.onClick.AddListener(CencleEvent);
        Apply.onClick.AddListener(ApplyEvent);
    }
    public void Spawn(string text, ChooseEvent chooseEvent , bool Active = true)
    {
        gameObject.SetActive(Active);
        choose = chooseEvent;
        C_Text.text = text;
    }
    public void Despawn(bool autoDeactive = true)
    {
        gameObject.SetActive(!autoDeactive);
    }

    void CencleEvent()
    {
        if(choose != null)
        {
            choose.Invoke(false);
            Despawn(AutoDeactive);
        }
    }
    void ApplyEvent()
    {
        if(choose != null)
        {
            choose.Invoke(true);
            Despawn(AutoDeactive);
        }
    }
}
