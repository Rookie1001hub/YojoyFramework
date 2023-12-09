#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="MinValueAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor.ValueResolver;
    using Sirenix.Utilities.Editor;
    using System;
    using UnityEngine;
    
    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeDrawer<T> : OdinAttributeDrawer<MinValueAttribute, T>
        where T : struct
    {
        private static IValueResolver<double> ValueResolver;

        private ValueProvider<double> minValueGetter;

        public override bool CanDrawTypeFilter(Type type)
        {
            return GenericNumberUtility.IsNumber(type);
        }

        protected override void Initialize()
        {
            if (ValueResolver == null)
            {
                ValueResolver = ValueResolverUtility.GetResolver<double>()
                    .TryMemberReference()
                    .TryExpression();
            }

            var context = ValueResolverUtility.CreateContext(this);
            this.minValueGetter = ValueResolver.Resolve(context, this.Attribute.Expression, this.Attribute.MinValue);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (this.minValueGetter.Failed)
            {
                SirenixEditorGUI.ErrorMessageBox(this.minValueGetter.GetNiceErrorMessage());
            }

            this.CallNextDrawer(label);

            if (this.minValueGetter.Failed)
            {
                return;
            }

            T value = this.ValueEntry.SmartValue;
            var min = this.minValueGetter.GetValue();

            if (!GenericNumberUtility.NumberIsInRange(value, min, double.MaxValue))
            {
                this.ValueEntry.SmartValue = GenericNumberUtility.ConvertNumber<T>(min);
            }
        }
    }
}
#endif
#pragma warning enable