using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoldPanel : MonoBehaviour
{
    public delegate void OpenDelegate(GameObject sender, bool IsOpen);
    public OpenDelegate OpenEvnet;

    public RectTransform SelfRect;
    public Button FoldButton;
    public GameObject FoldContent;

    //=============4방향 전부 + 들여쓰기 + 자식추가
    //============Title , Content 크기변경 적용

    [Space(5)]
    private bool isOpen = false;
    public bool IsOpen 
    { 
      get => isOpen; 
      set 
        {
            SetOpen(value);
            isOpen = value;
        } 
    }

    public float InnerPadding = 10f;

    public float TitleSize = 20f;
    public float ContentSize = 80f;

    void Start()
    {
        SelfRect = gameObject.GetComponent<RectTransform>();

        FoldButton.onClick.AddListener(ToggleFold);

        SetOpen(false);
    }

    public void SetPadding(RectTransform rect, float left, float top, float right, float bottom)//Setting Rect Position & Size
    {
        rect.offsetMax = new Vector2(-right, -top);
        rect.offsetMin = new Vector2(left, bottom);
    }
    //Template --> SetPadding(Temp, Temp.offsetMin.x, -Temp.offsetMax.y, -Temp.offsetMax.x, -Temp.offsetMin.y);

    void ToggleFold()
    {
        IsOpen = ! isOpen;
    }
    public void SetOpen(bool open)
    {
        if (SelfRect == null)
        {
            SelfRect = gameObject.GetComponent<RectTransform>();
        }

        if (open)
        {
            //SelfRect.sizeDelta = new Vector2(SelfRect.sizeDelta.x, TitleSize + ContentSize);
            SetPadding(SelfRect, SelfRect.offsetMin.x, -SelfRect.offsetMax.y, -SelfRect.offsetMax.x, (SelfRect.offsetMax.y - TitleSize - ContentSize));
        }
        else
        {
            //SelfRect.sizeDelta = new Vector2(SelfRect.sizeDelta.x, TitleSize);
            SetPadding(SelfRect, SelfRect.offsetMin.x, -SelfRect.offsetMax.y, -SelfRect.offsetMax.x, (SelfRect.offsetMax.y - TitleSize));
        }

        FoldContent.SetActive(open);

        if (OpenEvnet != null)
        {
            OpenEvnet.Invoke(gameObject, open);
        }
    }

}
