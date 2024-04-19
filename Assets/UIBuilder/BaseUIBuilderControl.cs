using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Custom;

public class BaseUIBuilderControl : VisualElement
{
    #region UXML
    public new class UxmlFactory : UxmlFactory<BaseUIBuilderControl, UxmlTraits> { }//UIBuilder에 등록
    /*
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        //public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
        //{ get { yield break; } }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
        }
    }*/


    static readonly string s_UssClaseName = "BaseUIBuilderControl";
    public BaseUIBuilderControl()
    {
        AddToClassList(s_UssClaseName);

        Init();

    }
    public BaseUIBuilderControl(string text, VisualElement target)
    {
        Init();
    }
    /// <summary>
    /// Need => public new class UxmlFactory : UxmlFactory<BaseUIBuilderControl, UxmlTraits> { }//UIBuilder에 등록
    /// </summary>
    /// <param name="_focusable"></param>
    /// <param name="_pickingMode"></param>
    public virtual void Init(bool _focusable = true, PickingMode _pickingMode = PickingMode.Position)
    {
        focusable = _focusable;
        pickingMode = _pickingMode;
    }

    public override void HandleEvent(EventBase evt)
    {
        base.HandleEvent(evt);

        if (evt.eventTypeId == AttachToPanelEvent.TypeId() || evt.eventTypeId == GeometryChangedEvent.TypeId())
        {
            Update();
        }
    }
    public virtual void Update()
    {
        for (int i = 0; i < childCount; i++)
        {
            if (ElementAt(i).GetType() == typeof(Button))
            {
                ElementAt(i).RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
            }
        }
    }
    private void OnMouseDownEvent(MouseDownEvent e)
    {
        for (int i = 0; i < childCount; i++)
        {
            if (ContainPoint(ElementAt(i), e.mousePosition))
            {
                OnClickedButton(ElementAt(i));
            }
        }
    }
    public bool ContainPoint(VisualElement element, Vector2 MousePosition)
    {
        //InRange => Vaule >= Min && Vaule <= Max

        return Math.InRange(MousePosition.x, element.worldBound.xMin, element.worldBound.xMax)
            && Math.InRange(MousePosition.y, element.worldBound.yMin, element.worldBound.yMax);
    }
    public virtual void OnClickedButton(VisualElement element)
    {

    }

    /// <summary>
    /// SetStyle : VisualElement.style
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public IResolvedStyle GetStyle(VisualElement element)
    {
        return element.resolvedStyle;
    }

    public Vector2 GetElementPosition(VisualElement element)
    {
        return new Vector2(element.resolvedStyle.left, element.resolvedStyle.top);
    }
    public void SetElementPosition(VisualElement element, Vector2 pos)
    {
        element.style.position = Position.Absolute;
        element.style.left = pos.x;
        element.style.top = pos.y;
    }
    public Vector2 GetElementSize(VisualElement element)
    {
        return new Vector2(element.resolvedStyle.width, element.resolvedStyle.height);
    }
    /// <summary>
    /// -1 is Auto
    /// </summary>
    /// <param name="element"></param>
    /// <param name="Size"></param>
    public void SetElementSize(VisualElement element, Vector2 Size)
    {
        if (Size.x > 0)
        {
            element.style.width = Size.x;
        }
        if (Size.y > 0)
        {
            element.style.height = Size.y;
        }
    }
    /// <summary>
    /// -1 is Auto
    /// </summary>
    public void SetElementSize(VisualElement element, float width, float height)
    {
        SetElementSize(element, new Vector2(width, height));
    }
    #endregion
}
