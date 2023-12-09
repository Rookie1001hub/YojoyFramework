#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="MaxValueAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor.ValueResolver;
    using Sirenix.Utilities.Editor;
    using UnityEngine;

    [DrawerPriority(0.3)]
    public sealed class MaxValueAttributeDrawer<T> : OdinAttributeDrawer<MaxValueAttribute, T>
        where T : struct
    {
        private static IValueResolver<double> ValueResolver;

        private ValueProvider<double> maxValueGetter;

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
            this.maxValueGetter = ValueResolver.Resolve(context, this.Attribute.Expression, this.Attribute.MaxValue);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (this.maxValueGetter.Failed)
            {
                SirenixEditorGUI.ErrorMessageBox(this.maxValueGetter.GetNiceErrorMessage());
            }

            this.CallNextDrawer(label);

            if (this.maxValueGetter.Failed)
            {
                return;
            }

            T value = this.ValueEntry.SmartValue;
            var max = this.maxValueGetter.GetValue();

            if (!GenericNumberUtility.NumberIsInRange(value, double.MinValue, max))
            {
                this.ValueEntry.SmartValue = GenericNumberUtility.ConvertNumber<T>(max);
            }
        }
    }
}
#endif
#pragma warning enable