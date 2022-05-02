using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TabBox_UIBuilder : VisualElement
{
    public int DetectedTab { get; private set; }
    public int DetectedContainer { get; private set; }

    public int DefaultTab { get; set; }
    public int SelectTab = -1;

    List<int> TabList = new();
    List<int> ContainerList = new();

    float ElementMaxHeight = 0;//Element flex Direction Distance

    #region UXML
    public new class UxmlFactory : UxmlFactory<TabBox_UIBuilder, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlIntAttributeDescription activeTab = new() { name = "Detected Tab", defaultValue = 0 };
        UxmlIntAttributeDescription activeContainer = new() { name = "Detected Container", defaultValue = 0 };
        UxmlIntAttributeDescription defaultTab = new() { name = "Default Tab index", defaultValue = 0 };

        //public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
        //{ get { yield break; } }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            var onwer = ve as TabBox_UIBuilder;

            //onwer.ActiveTab = activeTab.GetValueFromBag(bag, cc);
            onwer.DefaultTab = defaultTab.GetValueFromBag(bag, cc);
        }
    }

    static readonly string s_UssClaseName = "tabBox";

    public TabBox_UIBuilder()
    {
        AddToClassList(s_UssClaseName);

        focusable = true;
        pickingMode = PickingMode.Position;

        Debug.Log("Init TabBox");
    }
    public TabBox_UIBuilder(string text, VisualElement target)
    {

    }

    public override void HandleEvent(EventBase evt)
    {
        base.HandleEvent(evt);

        if (evt.eventTypeId == AttachToPanelEvent.TypeId() || evt.eventTypeId == GeometryChangedEvent.TypeId())
        {
            //Debug.Log("Child : " + childCount);// �����̵� childCount�� ���� �ڼո� ī��Ʈ
            //this.hierarchy.ElementAt

            //�����ڼ��� GropBox�� ContainBox , Button�� Tab ���� / ��������... �ǾƷ� ���?

            DetectedTab = 0;
            DetectedContainer = 0;

            TabList.Clear();
            ContainerList.Clear();

            bool IsVectical = resolvedStyle.flexDirection == FlexDirection.Column
                    || resolvedStyle.flexDirection == FlexDirection.ColumnReverse;

            //Debug.Log((IsVectical ? "Vectical" : "Horizon") + " / reverse : " + IsReverse);

            for (int i = 0; i < childCount; i++)
            {
                if (ElementAt(i).GetType() == typeof(Button))
                {
                    DetectedTab++;
                    TabList.Add(i);

                    ElementAt(i).RegisterCallback<MouseDownEvent>(OnMouseDownEvent, TrickleDown.TrickleDown);
                }
                else if (ElementAt(i).GetType() == typeof(GroupBox))
                {
                    DetectedContainer++;
                    ContainerList.Add(i);
                }
            }//Setup list , RegisterCallback


            ElementMaxHeight = 0;//Element flex Direction Distance
            for (int i = 0; i < TabList.Count; i++)
            {
                //===================================================Mathf.Min(TabList.Count, ContainerList.Count) �ʰ��ϸ� �����
                if (IsVectical)
                {
                    float e_hight = ElementAt(TabList[i]).resolvedStyle.height;
                    if (e_hight > ElementMaxHeight)
                    {
                        ElementMaxHeight = e_hight;
                    }

                    ElementAt(TabList[i]).style.position = Position.Absolute;
                    ElementAt(TabList[i]).style.left = (paddingRect.width / TabList.Count) * i;
                    ElementAt(TabList[i]).style.top = (resolvedStyle.flexDirection == FlexDirection.Column) ?
                        0 : (paddingRect.height - e_hight);


                    ElementAt(TabList[i]).style.width = (paddingRect.width / TabList.Count);

                }//��ư ���ι������� ��ġ
                else
                {
                    float e_width = ElementAt(TabList[i]).resolvedStyle.width;
                    if (e_width > ElementMaxHeight)
                    {
                        ElementMaxHeight = e_width;
                    }

                    ElementAt(TabList[i]).style.position = Position.Absolute;
                    ElementAt(TabList[i]).style.left = (resolvedStyle.flexDirection == FlexDirection.Row) ?
                        0 : (paddingRect.width - e_width);
                    ElementAt(TabList[i]).style.top = (paddingRect.height / TabList.Count) * i;


                    ElementAt(TabList[i]).style.height = (paddingRect.height / TabList.Count);

                }//��ư ���� �������� ��ġ
            }//Tab RePosition

            ContainerReposition(IsVectical, ElementMaxHeight);
        }
    }
    private void OnMouseDownEvent(MouseDownEvent e)
    {
        for (int i = 0; i < childCount; i++)
        {
            //(this.ElementAt(i).ContainsPoint(e.localMousePosition))//�� �ѹ��� ���� �������� ���߃𤻤���
            if (ContainPoint(ElementAt(i), e.mousePosition))
            {
                SelectTab = i;

                bool IsVectical = resolvedStyle.flexDirection == FlexDirection.Column
                                    || resolvedStyle.flexDirection == FlexDirection.ColumnReverse;

                ContainerReposition(IsVectical, ElementMaxHeight);
            }
        }
    }
    public bool ContainPoint(VisualElement element, Vector2 MousePosition)
    {
        //InRange => Vaule >= Min && Vaule <= Max

        return Math.InRange(MousePosition.x, element.worldBound.xMin, element.worldBound.xMax)
            && Math.InRange(MousePosition.y, element.worldBound.yMin, element.worldBound.yMax);
    }

    void ContainerReposition(bool IsVectical, float ElementHeight)
    {
        if (ContainerList.Count > 0)
        {
            int selct = (SelectTab >= 0) ? SelectTab : Mathf.Max(DefaultTab, 0);
            if (selct >= ContainerList.Count)
            {
                selct = 0;
#if UNITY_EDITOR
                Debug.LogWarning("Need More GroupBox");
#endif
            }

            for (int i = 0; i < ContainerList.Count; i++)
            {
                ElementAt(ContainerList[i]).visible = (selct == i);

                ElementAt(ContainerList[i]).style.position = Position.Absolute;

                if (IsVectical)
                {
                    ElementAt(ContainerList[i]).style.left = 0;
                    ElementAt(ContainerList[i]).style.top = (resolvedStyle.flexDirection == FlexDirection.Column) ?
                        ElementHeight : 0;

                    ElementAt(ContainerList[i]).style.width = paddingRect.width;
                    ElementAt(ContainerList[i]).style.height = paddingRect.height - ElementHeight;
                }
                else
                {
                    ElementAt(ContainerList[i]).style.left = (resolvedStyle.flexDirection == FlexDirection.Row) ?
                        ElementHeight : 0;
                    ElementAt(ContainerList[i]).style.top = 0;

                    ElementAt(ContainerList[i]).style.width = paddingRect.width - ElementHeight;
                    ElementAt(ContainerList[i]).style.height = paddingRect.height;
                }
            }
        }//Container RePosition
    }
    #endregion
}
