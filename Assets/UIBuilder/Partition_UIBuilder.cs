using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.Scripting;

public class Partition_UIBuilder : VisualElement
{
    public float Rate { get; set; }
    #region UXML
    //[Preserve]
    public new class UxmlFactory : UxmlFactory<Partition_UIBuilder, UxmlTraits>  { }
    //[Preserve]
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlFloatAttributeDescription m_Rate = new() { name = "rate", defaultValue = -1};

        //public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        //{
        //    get { yield break; }
        //}

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            var onwer = ve as Partition_UIBuilder;

            onwer.Rate = m_Rate.GetValueFromBag(bag, cc);
        }
        
    }

    static readonly string s_UssClaseName = "partition";
    static readonly string styleName = "PartitionStyles";
    static readonly string UxmlName = "Partition";

    Label m_Label;
    public VisualElement Target { get; set; }

    public Partition_UIBuilder()
    {
        Init();
    }
    public Partition_UIBuilder(string text, VisualElement target)
    {
        Init();
        m_Label.text = text;
        Target = target;
    }
    void Init()
    {
        AddToClassList(s_UssClaseName);
        //styleSheets.Add(Resources.Load<StyleSheet>($"Styles/{styleName}"));

        //VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>($"UXML/{UxmlName}");
        //visualTree.CloneTree(this);

        CreateContexMenu(this);
    }
    private void PopulateContextMenu(ContextualMenuPopulateEvent populateEvent)
    {
        DropdownMenu dropdownMenu = populateEvent.menu;

        //if (IsCloseable)
        {
        //    dropdownMenu.AppendAction("Close Tab", e => OnClose(this));
        }
    }
    void CreateContexMenu(VisualElement visualElement)
    {
        ContextualMenuManipulator menuManipulator = new ContextualMenuManipulator(PopulateContextMenu);

        visualElement.focusable = true;
        visualElement.pickingMode = PickingMode.Position;

        visualElement.AddManipulator(menuManipulator);

        //Debug.Log("Init");//처음 스폰시에만 작동
    }

    protected override void ExecuteDefaultAction(EventBase evt)
    {
        base.ExecuteDefaultAction(evt);

        //Debug.Log(evt);

        if (evt.eventTypeId == GeometryChangedEvent.TypeId())
        {
            //style 갱신X
        }
        if (evt.eventTypeId == PointerEnterEvent.TypeId())
        {
            for (int i = 0; i < childCount; i++)
            {
                var em = this.ElementAt(i);

                //Debug.Log("Onwer : " + style.width + " , " + style.height + "\n Slot : " + em.style.width + " , " + em.style.height);
                
            }
        }
    }
    public override bool Overlaps(Rect rectangle)
    {
        //NotWork?
        return base.Overlaps(rectangle);
    }//NotWork?
    public override void HandleEvent(EventBase evt)
    {
        base.HandleEvent(evt);

        //visualTreeAssetSource.CloneTree(this);

        //Debug.Log("Handle : " + evt);
        //Debug.Log("Rate : " + Rate);//============되긴되는데  변경사항이 실시간으로 안됨

        //UnityEngine.UIElements.GeometryChangedEvent.TypeId()
        if (evt.eventTypeId == AttachToPanelEvent.TypeId() || evt.eventTypeId == GeometryChangedEvent.TypeId())
        {
            //Debug.Log("GeometryChangedEvent \n Children : " + this.childCount);
            //===============================================style 값 가져오는게 전혀 안되는데
            //==============에디터 변경시 다른창으로 바꾸었다 다시 켜야 적용되는데...

            //=================== Magin&Padding 을 0 으로 하고 , Rate로 첫번째 자식의 비율

            {
                /*
                bool IsVectical = false;
                if (childCount >= 2)
                {
                    IsVectical = Mathf.Approximately(ElementAt(0).localBound.x, ElementAt(1).localBound.x);
                }
                //this.style.flexDirection 은 항상 FlexDirection.Column

                for (int i = 0; i < childCount; i++)
                {
                    var em = this.ElementAt(i);

                    //Debug.Log("Onwer : " + this.paddingRect + "\n" + (IsVectical ? "Vectical" : "Horizon"));//style.flexDirection.value

                    {
                        em.style.marginBottom = 0;
                        em.style.marginLeft = 0;
                        em.style.marginRight = 0;
                        em.style.marginTop = 0;

                        em.style.paddingBottom = 0;
                        em.style.paddingLeft = 0;
                        em.style.paddingRight = 0;
                        em.style.paddingTop = 0;
                    }//Margin , Padding 0
                    
                    if (IsVectical)
                    {
                        em.style.width = paddingRect.width;
                        em.style.height = paddingRect.height / childCount;
                    }
                    else
                    {
                        em.style.width = paddingRect.width / childCount;
                        em.style.height = paddingRect.height;
                    }
                }
                */
            }//Legacy - Same Rate

            {
                bool IsVectical = false;
                if (childCount >= 2)
                {
                    IsVectical = Mathf.Approximately(ElementAt(0).localBound.x, ElementAt(1).localBound.x);
                }
                //this.style.flexDirection 은 항상 FlexDirection.Column

                for (int i = 0; i < childCount; i++)
                {
                    var em = this.ElementAt(i);

                    {
                        em.style.marginBottom = 0;
                        em.style.marginLeft = 0;
                        em.style.marginRight = 0;
                        em.style.marginTop = 0;

                        em.style.paddingBottom = 0;
                        em.style.paddingLeft = 0;
                        em.style.paddingRight = 0;
                        em.style.paddingTop = 0;
                    }//Margin , Padding 0

                    if (IsVectical)
                    {
                        em.style.width = paddingRect.width;

                        if (Rate > 0)
                        {
                            if (i == 0)
                            {
                                em.style.height = paddingRect.height * Rate;
                            }
                            else
                            {
                                em.style.height = paddingRect.height * (1 - Rate) / (childCount - 1);
                            }
                        }else
                        {
                            em.style.height = paddingRect.height / childCount;
                        }
                    }else
                    {
                        if (Rate > 0)
                        {
                            if (i == 0)
                            {
                                em.style.width = paddingRect.width * Rate;
                            }else
                            {
                                em.style.width = paddingRect.width * (1 - Rate) / (childCount - 1);
                            }
                        }else
                        {
                            em.style.width = paddingRect.width / childCount;
                        }
                        em.style.height = paddingRect.height;
                    }
                }
            }
        }

        if (evt.eventTypeId == PointerEnterEvent.TypeId())
        {
            for (int i = 0; i < childCount; i++)
            {
                var em = this.ElementAt(i);

                //Debug.Log("Onwer : " + style.width.value.value + " , " + style.height.value.value + "\n" + this.paddingRect);
                //paddingRect 으로 크기 가져올수 있음 , style.width는 null로 뜸


                //visualTreeAssetSource.CloneTree(this);//....무한 루프?
            }
        }

    }
    
    #endregion
}
