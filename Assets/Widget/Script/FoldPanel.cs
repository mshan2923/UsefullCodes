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

    [Space(5) , SerializeField]
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


    void ToggleFold()
    {
        //IsOpen = ! isOpen;
        isOpen = !isOpen;
        SetOpen(isOpen);
    }
    public void SetOpen(bool open , bool Event = true)
    {
        if (SelfRect == null)
        {
            SelfRect = gameObject.GetComponent<RectTransform>();
        }

        //SetPadding(SelfRect, SelfRect.offsetMin.x, -SelfRect.offsetMax.y, -SelfRect.offsetMax.x, -SelfRect.offsetMin.y);
        FoldContent.gameObject.SetActive(open);

        var buttonRect = FoldButton.gameObject.GetComponent<RectTransform>();
        Vector2 Lpos = SelfRect.anchoredPosition;

        //좌하단이 시작점,  상대적거리으로 / 한점을 기준으로 접고펴기
        switch (Direction)
        {
            case FoldDirection.RightToLeft:
                {
                    if (open)
                    {
                        //SelfRect.sizeDelta = FoldPanelSize;
                        //SelfRect.localPosition = new Vector3(FoldPanelSize.x * 0.5f, 0, 0);

                        WidgetExpandScript.SetTransform(SelfRect, Lpos, FoldPanelSize, new Vector2(0, 0.5f));
                    }
                    else
                    {
                        //SelfRect.sizeDelta = new Vector2(TitleHeight, FoldPanelSize.y);
                        WidgetExpandScript.SetTransform(SelfRect, Lpos, new Vector2(TitleHeight, FoldPanelSize.x), new Vector2(0, 0.5f));
                    }

                    {
                        WidgetExpandScript.SetPadding(buttonRect, (open ? (SelfRect.rect.size.x - TitleHeight) : 0), 0, 0, 0);//(FoldPanelSize.x - TitleHeight)
                        WidgetExpandScript.SetPadding(FoldContent, 0, 0, TitleHeight, 0);
                    }
                    break;
                }
            case FoldDirection.LeftToRight:
                {
                    if (open)
                    {
                        //SelfRect.sizeDelta = FoldPanelSize;
                        //SelfRect.localPosition = new Vector3(FoldPanelSize.x * 0.5f, 0, 0);

                        WidgetExpandScript.SetTransform(SelfRect, Lpos, FoldPanelSize, new Vector2(0, 0.5f));
                    }
                    else
                    {
                        //SelfRect.sizeDelta = new Vector2(TitleHeight, FoldPanelSize.y);
                        WidgetExpandScript.SetTransform(SelfRect, Lpos, new Vector2(TitleHeight, FoldPanelSize.x), new Vector2(0, 0.5f));
                    }

                    {
                        WidgetExpandScript.SetPadding(buttonRect, 0, 0, (open ? (SelfRect.rect.size.x - TitleHeight) : 0), 0);
                        WidgetExpandScript.SetPadding(FoldContent, TitleHeight, 0, 0, 0);
                    }
                    break;
                }
            case FoldDirection.TopToButtom:
                {
                    if (open)
                    {
                        //SelfRect.sizeDelta = FoldPanelSize;
                        //SelfRect.localPosition = new Vector3(0, FoldPanelSize.y * -0.5f, 0);

                        WidgetExpandScript.SetTransform(SelfRect, Lpos, FoldPanelSize, new Vector2(0.5f,0));
                    }
                    else
                    {
                        //SelfRect.sizeDelta = new Vector2(FoldPanelSize.x, TitleHeight);

                        WidgetExpandScript.SetTransform(SelfRect, Lpos, new Vector2(FoldPanelSize.x, TitleHeight), new Vector2(0.5f, 0));

                    }

                    {
                        WidgetExpandScript.SetPadding(buttonRect, 0, 0, 0, (open ? (SelfRect.rect.size.y - TitleHeight) : 0));
                        WidgetExpandScript.SetPadding(FoldContent, 0, TitleHeight, 0, 0);
                    }
                    break;
                }
            case FoldDirection.ButtomToTop:
                {
                    if (open)
                    {
                        //SelfRect.sizeDelta = FoldPanelSize;
                        //SelfRect.localPosition = new Vector3(0, FoldPanelSize.y * 0.5f, 0);

                        WidgetExpandScript.SetTransform(SelfRect, Lpos, FoldPanelSize, new Vector2(0.5f, 0));
                    }
                    else
                    {
                        //SelfRect.sizeDelta = new Vector2(FoldPanelSize.x, TitleHeight);
                        WidgetExpandScript.SetTransform(SelfRect, Lpos, new Vector2(FoldPanelSize.x, TitleHeight), new Vector2(0.5f, 0));
                    }

                    {
                        WidgetExpandScript.SetPadding(buttonRect, 0, (open ? (SelfRect.rect.size.y - TitleHeight) : 0), 0, 0);
                        WidgetExpandScript.SetPadding(FoldContent, 0, 0, 0, TitleHeight);
                    }
                    break;
                }
        }

        if (OpenEvnet != null && Event)
        {
            OpenEvnet.Invoke(gameObject, open);
        }
    }

    public void ReDraw()
    {
        SetOpen(isOpen, false);//FoldContent (비)활성화 설정
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
