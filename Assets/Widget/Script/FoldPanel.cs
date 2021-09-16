using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoldPanel : MonoBehaviour
{
    public enum FoldDirection
    {
        RightToLeft, LeftToRight, TopToButtom, ButtomToTop
    }
    public delegate void OpenDelegate(GameObject sender, bool IsOpen);
    public OpenDelegate OpenEvnet;

    public RectTransform SelfRect;
    public Button FoldButton;
    public RectTransform FoldContent;

    public FoldDirection Direction = FoldDirection.TopToButtom;
    public float TitleHeight = 30;
    public Vector2 FoldPanelSize = new Vector2(350,250);

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
    }//자동 동기화

    void Start()
    {
        SelfRect = gameObject.GetComponent<RectTransform>();

        FoldButton.onClick.AddListener(ToggleFold);

        ReDraw();
    }

    public void SetPadding(RectTransform rect, float left, float top, float right, float bottom)//Setting Rect Position & Size
    {
        rect.offsetMax = new Vector2(-right, -top);
        rect.offsetMin = new Vector2(left, bottom);
        //앵커 -> 좌하단이 시작점,  상대적거리으로
        //일반 -> 중앙기준
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

        //SetPadding(SelfRect, SelfRect.offsetMin.x, -SelfRect.offsetMax.y, -SelfRect.offsetMax.x, -SelfRect.offsetMin.y);
        var buttonRect = FoldButton.gameObject.GetComponent<RectTransform>();

        //좌하단이 시작점,  상대적거리으로 / 한점을 기준으로 접고펴기
        switch (Direction)
        {
            case FoldDirection.RightToLeft:
                {
                    if (open)
                    {
                        //SetPadding(SelfRect, -FoldPanelSize.x, FoldPanelSize.y * 0.5f, 0, FoldPanelSize.y * -0.5f);
                        SelfRect.sizeDelta = FoldPanelSize;
                        SelfRect.localPosition = new Vector3(FoldPanelSize.x * -0.5f, 0, 0);
                    }
                    else
                    {
                        //SetPadding(SelfRect, -TitleHeight, FoldPanelSize.y * 0.5f, 0, FoldPanelSize.y * -0.5f);
                        SelfRect.sizeDelta = new Vector2(TitleHeight, FoldPanelSize.y);
                        SelfRect.localPosition = new Vector3(TitleHeight * -0.5f, 0, 0);
                    }

                    {
                        buttonRect.anchorMax = new Vector2(1, 0);
                        buttonRect.anchorMin = Vector2.one;
                        buttonRect.pivot = new Vector2(1, 0.5f);
                        break;
                    }
                }
            case FoldDirection.LeftToRight:
                {
                    if (open)
                    {
                        //SetPadding(SelfRect, -FoldPanelSize.x, FoldPanelSize.y * 0.5f, 0, FoldPanelSize.y * -0.5f);
                        SelfRect.sizeDelta = FoldPanelSize;
                        SelfRect.localPosition = new Vector3(FoldPanelSize.x * 0.5f, 0, 0);
                    }
                    else
                    {
                        //SetPadding(SelfRect, -TitleHeight, FoldPanelSize.y * 0.5f, 0, FoldPanelSize.y * -0.5f);
                        SelfRect.sizeDelta = new Vector2(TitleHeight, FoldPanelSize.y);
                        SelfRect.localPosition = new Vector3(TitleHeight * 0.5f, 0, 0);
                    }

                    {
                        buttonRect.anchorMax = new Vector2(0, 0);
                        buttonRect.anchorMin = new Vector2(0, 1);
                        buttonRect.pivot = new Vector2(0, 0.5f);
                        break;
                    }
                }
            case FoldDirection.TopToButtom:
                {
                    if (open)
                    {
                        //SetPadding(SelfRect, -FoldPanelSize.x, FoldPanelSize.y * 0.5f, 0, FoldPanelSize.y * -0.5f);
                        SelfRect.sizeDelta = FoldPanelSize;
                        SelfRect.localPosition = new Vector3(0, 0, 0);
                    }
                    else
                    {
                        //SetPadding(SelfRect, -TitleHeight, FoldPanelSize.y * 0.5f, 0, FoldPanelSize.y * -0.5f);
                        SelfRect.sizeDelta = new Vector2(FoldPanelSize.x, TitleHeight);
                        SelfRect.localPosition = new Vector3(0, TitleHeight * -0.5f, 0);
                    }

                    {
                        buttonRect.anchorMax = new Vector2(0, 1);
                        buttonRect.anchorMin = Vector2.one;
                        buttonRect.pivot = new Vector2(0.5f, 1);
                    }//Anchor
                    break;
                }
            case FoldDirection.ButtomToTop:
                {
                    if (open)
                    {
                        //SetPadding(SelfRect, -FoldPanelSize.x, FoldPanelSize.y * 0.5f, 0, FoldPanelSize.y * -0.5f);
                        SelfRect.sizeDelta = FoldPanelSize;
                        SelfRect.localPosition = new Vector3(0, FoldPanelSize.y * 0.5f, 0);
                    }
                    else
                    {
                        //SetPadding(SelfRect, -TitleHeight, FoldPanelSize.y * 0.5f, 0, FoldPanelSize.y * -0.5f);
                        SelfRect.sizeDelta = new Vector2(FoldPanelSize.x, TitleHeight);
                        SelfRect.localPosition = new Vector3(0, TitleHeight * 0.5f, 0);
                    }

                    {
                        buttonRect.anchorMax = new Vector2(0, 0);
                        buttonRect.anchorMin = new Vector2(1, 0);
                        buttonRect.pivot = new Vector2(0.5f, 0);
                    }//Anchor
                    break;
                }
        }

        FoldContent.gameObject.SetActive(open);

        if (OpenEvnet != null)
        {
            OpenEvnet.Invoke(gameObject, open);
        }
    }

    public void ReDraw()
    {
        SetOpen(isOpen);//FoldContent (비)활성화 설정

        var buttonRect = FoldButton.gameObject.GetComponent<RectTransform>();

        switch (Direction)
        {
            case FoldDirection.RightToLeft:
                {
                    buttonRect.localPosition = Vector2.zero;
                    SetPadding(buttonRect, -TitleHeight, -FoldPanelSize.y, 0, -FoldPanelSize.y);

                    FoldContent.localPosition = Vector2.zero;
                    SetPadding(FoldContent, 0, 0, TitleHeight, 0);

                    break;
                }
            case FoldDirection.LeftToRight:
                {
                    buttonRect.localPosition = Vector2.zero;
                    SetPadding(buttonRect, 0, -FoldPanelSize.y, -TitleHeight, -FoldPanelSize.y);

                    FoldContent.localPosition = Vector2.zero;
                    SetPadding(FoldContent, TitleHeight, 0, 0, 0);

                    break;
                }
            case FoldDirection.TopToButtom:
                {
                    //SelfRect는 SetOpen에 포함
                    buttonRect.localPosition = Vector2.zero;
                    SetPadding(buttonRect, -FoldPanelSize.x, 0, -FoldPanelSize.x, -TitleHeight);

                    FoldContent.localPosition = Vector2.zero;
                    //SetPadding(FoldContent, FoldPanelSize.x * -0.5f, TitleHeight, FoldPanelSize.x * -0.5f, -FoldPanelSize.y);
                    SetPadding(FoldContent, 0, TitleHeight, 0, 0);

                    break;
                }
            case FoldDirection.ButtomToTop:
                {
                    buttonRect.localPosition = Vector2.zero;
                    SetPadding(buttonRect, -FoldPanelSize.x, -TitleHeight, -FoldPanelSize.x, 0);

                    FoldContent.localPosition = Vector2.zero;
                    //SetPadding(FoldContent, FoldPanelSize.x * -0.5f, TitleHeight, FoldPanelSize.x * -0.5f, -FoldPanelSize.y);
                    SetPadding(FoldContent, 0, 0, 0, TitleHeight);
                    break;
                }
        }//정렬 , Title + FoldContent 크기
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(FoldPanel))]
public class FoldPanelEditor : UnityEditor.Editor
{
    FoldPanel onwer;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        onwer = target as FoldPanel;

        if (GUILayout.Button("Redraw"))
        {
            onwer.ReDraw();
        }
    }
}
#endif
