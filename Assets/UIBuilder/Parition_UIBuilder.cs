using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Parition_UIBuilder : BaseUIBuilderControl
{
    public float Rate { get; set; }
    public new class UxmlFactory : UxmlFactory<Parition_UIBuilder, UxmlTraits> { }//UIBuilder에 등록
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlFloatAttributeDescription m_Rate = new() { name = "rate", defaultValue = -1 };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            var onwer = ve as Parition_UIBuilder;

            onwer.Rate = m_Rate.GetValueFromBag(bag, cc);
        }

    }//UIBuilder의 Property 등록

    public override void Init(bool _focusable = true, PickingMode _pickingMode = PickingMode.Position)
    {
        base.Init(_focusable, _pickingMode);
    }
    public override void Update()
    {
        base.Update();

        bool IsVectical = resolvedStyle.flexDirection == FlexDirection.Column
                            || resolvedStyle.flexDirection == FlexDirection.ColumnReverse;

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
                //em.style.width = GetStyle(this).width;//paddingRect.width;

                if (Rate > 0)
                {
                    if (i == 0)
                    {
                        em.style.height = GetStyle(this).height * Rate;
                    }
                    else
                    {
                        em.style.height = GetStyle(this).height * (1 - Rate) / (childCount - 1);
                    }
                }
                else
                {
                    em.style.height = GetStyle(this).height / childCount;
                }
            }
            else
            {
                //em.style.height = GetStyle(this).height;

                if (Rate > 0)
                {
                    if (i == 0)
                    {
                        em.style.width = GetStyle(this).width * Rate;
                    }
                    else
                    {
                        em.style.width = GetStyle(this).width * (1 - Rate) / (childCount - 1);
                    }
                }
                else
                {
                    em.style.width = GetStyle(this).width / childCount;
                }
            }
        }

    }
    public override void OnClickedButton(VisualElement element)
    {
        base.OnClickedButton(element);
    }
}
