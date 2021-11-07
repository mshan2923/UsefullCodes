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
    public Vector2 PanelSize = new Vector2(350,250);
    public float CloseHorizonHeight = 100;

    [Space(10)]
    public bool AnchoredPanel = false;
    public bool StartRedraw = true;
    public bool Test_Reseting = true;

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

        if(StartRedraw)
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
        Vector2 FoldSize = Vector2.zero;

        FoldContent.gameObject.SetActive(open);
        //좌하단이 시작점,  상대적거리으로 / 한점을 기준으로 접고펴기
        switch (Direction)
        {
            case FoldDirection.RightToLeft:
                {
                    SelfRect.pivot = new Vector2(1, 0.5f);
                    if (AnchoredPanel)
                    {
                        SelfRect.anchorMin = Vector2.right;
                        SelfRect.anchorMax = Vector2.one;
                    }

                    buttonRect.anchorMin = new Vector2(1, 0.5f);
                    buttonRect.anchorMax = new Vector2(1, 0.5f);
                    buttonRect.pivot = new Vector2(1, 0.5f);

                    if (open)
                    {
                        WidgetExpand.SetTransform(SelfRect, Lpos, PanelSize, new Vector2(1, 0.5f));
                        FoldSize = (PanelSize - Vector2.right * TitleHeight);
                    }
                    else
                    {
                        WidgetExpand.SetTransform(SelfRect, Lpos, new Vector2(TitleHeight, CloseHorizonHeight), new Vector2(1, 0.5f));
                        FoldSize = Vector2.up * CloseHorizonHeight;
                    }

                    {
                        //WidgetExpand.SetPadding(buttonRect, (open ? (SelfRect.rect.size.x - TitleHeight) : 0), 0, 0, 0);//(FoldPanelSize.x - TitleHeight)
                        //WidgetExpand.SetPadding(FoldContent, 0, 0, TitleHeight, 0);
                        WidgetExpand.SetTransform(buttonRect, Vector2.zero, new Vector2(TitleHeight, open ? PanelSize.y : CloseHorizonHeight), new Vector2(1, 0.5f));
                        WidgetExpand.SetTransform(FoldContent, new Vector2(-TitleHeight, 0), FoldSize, new Vector2(1, 0.5f));
                    }
                    break;
                }
            case FoldDirection.LeftToRight:
                {
                    SelfRect.pivot = new Vector2(0, 0.5f);
                    if (AnchoredPanel)
                    {
                        SelfRect.anchorMin = Vector2.zero;
                        SelfRect.anchorMax = Vector2.up;
                    }

                    buttonRect.anchorMin = Vector2.up * 0.5f;
                    buttonRect.anchorMax = Vector2.up * 0.5f;
                    buttonRect.pivot = Vector2.up * 0.5f;

                    if (open)
                    {
                        WidgetExpand.SetTransform(SelfRect, Lpos, PanelSize, new Vector2(0, 0.5f));
                        FoldSize = (PanelSize - Vector2.right * TitleHeight);
                    }
                    else
                    {
                        WidgetExpand.SetTransform(SelfRect, Lpos, new Vector2(TitleHeight, CloseHorizonHeight), new Vector2(0, 0.5f));
                        FoldSize = Vector2.up * CloseHorizonHeight;
                    }

                    {
                        WidgetExpand.SetTransform(buttonRect, Vector2.zero, new Vector2(TitleHeight, open ? PanelSize.y : CloseHorizonHeight), new Vector2(0, 0.5f));
                        WidgetExpand.SetTransform(FoldContent, new Vector2(TitleHeight, 0), FoldSize, new Vector2(0, 0.5f));
                    }
                    break;
                }
            case FoldDirection.TopToButtom:
                {
                    SelfRect.pivot = new Vector2(0.5f, 1);
                    if (AnchoredPanel)
                    {
                        SelfRect.anchorMin = Vector2.up;
                        SelfRect.anchorMax = Vector2.one;
                    }

                    buttonRect.anchorMin = Vector2.up;
                    buttonRect.anchorMax = Vector2.one;
                    buttonRect.pivot = new Vector2(0.5f, 1);

                    if (open)
                    {
                        WidgetExpand.SetTransform(SelfRect, Lpos, PanelSize, new Vector2(0.5f, 0));

                        FoldSize = (PanelSize - Vector2.up * TitleHeight);
                    }
                    else
                    {
                        WidgetExpand.SetTransform(SelfRect, Lpos, new Vector2(PanelSize.x, TitleHeight), new Vector2(0.5f, 0));

                        FoldSize = Vector2.right * PanelSize;
                    }

                    {
                        WidgetExpand.SetTransform(buttonRect, new Vector2(0, -TitleHeight), new Vector2(PanelSize.x, TitleHeight), new Vector2(0.5f, 1));
                        WidgetExpand.SetTransform(FoldContent, Vector2.zero, FoldSize, new Vector2(0.5f, 1));
                    }
                    break;
                }
            case FoldDirection.ButtomToTop:
                {
                    SelfRect.pivot = new Vector2(0.5f, 0);
                    if (AnchoredPanel)
                    {
                        SelfRect.anchorMin = Vector2.zero;
                        SelfRect.anchorMax = Vector2.right;
                    }

                    buttonRect.anchorMin = Vector2.zero;
                    buttonRect.anchorMax = Vector2.right;
                    buttonRect.pivot = new Vector2(0.5f, 0);

                    if (open)
                    {
                        WidgetExpand.SetTransform(SelfRect, Lpos, PanelSize, new Vector2(0.5f, 1));

                        FoldSize = (PanelSize - Vector2.up * TitleHeight);
                    }
                    else
                    {
                        WidgetExpand.SetTransform(SelfRect, Lpos, new Vector2(PanelSize.x, TitleHeight), new Vector2(0.5f, 1));

                        FoldSize = Vector2.right * PanelSize;
                    }

                    {
                        WidgetExpand.SetTransform(buttonRect, new Vector2(0, TitleHeight), new Vector2(PanelSize.x, TitleHeight), new Vector2(0.5f, 0));
                        WidgetExpand.SetTransform(FoldContent, Vector2.zero, FoldSize, new Vector2(0.5f, 0));
                    }
                    break;
                }
        }

        if (OpenEvnet != null && Event)
        {
            OpenEvnet.Invoke(gameObject, open);
        }
    }//폴드를 방향에 맞춰 피봇이 정해지므로 , 피봇에 맞춰 위치만 설정

    /// <summary>
    /// 폴드를 방향에 맞춰 피봇이 정해지므로 , 피봇에 맞춰 위치만 설정
    /// </summary>
    public void ReDraw()
    {
        if (Test_Reseting)
            WidgetExpand.SetTransform(SelfRect, SelfRect.anchoredPosition, Vector2.zero, SelfRect.pivot);

        SetOpen(isOpen, false);//FoldContent (비)활성화 설정
    }//폴드가 지멋대로 변경될때 있음
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
