using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string key;
    Text Text;
    TextMeshProUGUI TextPro;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        Text = GetComponent<Text>();
        TextPro = GetComponent<TextMeshProUGUI>();
        ReloadText();
    }
    public void ReloadText()
    {
        if (Text != null)
            Text.text = LocalizationManager.Instance.GetLocalizedText(key);

        if (TextPro != null)
            TextPro.text = LocalizationManager.Instance.GetLocalizedText(key);
    }
}
