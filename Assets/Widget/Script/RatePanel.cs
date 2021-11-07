using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatePanel : MonoBehaviour
{
    [Header("Rect Vaule is Clamp 0 ~ 1 / Origin : LeftButtom")]
    public Map<GameObject, Rect> ObjRect = new Map<GameObject, Rect>();
    //해상도 변경알림 -> 대신 다시 활성화 될때 변경확인

    public bool ForcePivot_LeftTop = false;//자식위젯 피벗을 모두 좌상단으로 변환
    public bool SetParentCanvas = false;

    Vector2 LastResolution = new Vector2();
    private void OnEnable()
    {
        Vector2 Temp = new Vector2(Screen.width, Screen.height);

        if (LastResolution != Temp)
        {
            LastResolution = Temp;

            UIUpdate();
        }
    }
    void Start()
    {

    }

    public void UIUpdate(bool UseCanvasSize = false)//UseCanvasSize -> Before Play
    {
        //Debug.Log(MainCanvasSingleton.Instance.MainCanvas.renderingDisplaySize);

        for (int i = 0; i < ObjRect.Count; i++)
        {
            var RectT = ObjRect.GetKey(i).GetComponent<RectTransform>();

            if (ForcePivot_LeftTop)
            {
                RectT.pivot = new Vector2(0, 1);
            }

            if (UseCanvasSize || SetParentCanvas)
            {
                //LastResolution = new Vector2(Screen.width, Screen.height);
                LastResolution = gameObject.GetComponentInParent<Canvas>().pixelRect.size;                
            }

            ObjRect.GetKey(i).transform.position = ObjRect.GetVaule(i).position * LastResolution;
            RectT.sizeDelta = ObjRect.GetVaule(i).size * LastResolution;

        }
    }

}

[UnityEditor.CustomEditor(typeof(RatePanel))]
public class RatePanelEditor : UnityEditor.Editor
{
    RatePanel owner;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        owner = target as RatePanel;

        if (GUILayout.Button("Update"))
        {
            owner.UIUpdate(true);
        }
    }
}
