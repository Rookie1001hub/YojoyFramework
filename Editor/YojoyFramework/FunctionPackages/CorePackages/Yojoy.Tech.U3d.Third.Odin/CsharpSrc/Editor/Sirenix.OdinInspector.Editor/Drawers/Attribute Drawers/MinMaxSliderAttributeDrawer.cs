#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="MinMaxSliderAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.OdinInspector.Editor.ValueResolver;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System.Reflection;
    using UnityEngine;

    /// <summary>
    /// Draws Vector2 properties marked with <see cref="MinMaxSliderAttribute"/>.
    /// </summary>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>
    public sealed class MinMaxSliderAttributeDrawer : OdinAttributeDrawer<MinMaxSliderAttribute, Vector2>
    {
        private static IValueResolver<float> scalarResolver = ValueResolverUtility.GetResolver<float>()
            .TryMemberReference()
            .TryExpression();
        private static IValueResolver<Vector2> vectorResolver = ValueResolverUtility.GetDrawerResolver<MinMaxSliderAttributeDrawer, Vector2>()
            .TryMemberReference()
            .TryExpression()
            .Func(context => new Vector2(context.Drawer.minGetter.GetValue(), context.Drawer.maxGetter.GetValue()));

        private string errorMessage;

        //private InspectorPropertyValueGetter<int> intMinGetter;
        //private InspectorPropertyValueGetter<float> floatMinGetter;

        //private InspectorPropertyValueGetter<int> intMaxGetter;
        //private InspectorPropertyValueGetter<float> floatMaxGetter;

        //private InspectorPropertyValueGetter<Vector2> vector2MinMaxGetter;

        private IValueProvider<float> minGetter;
        private IValueProvider<float> maxGetter;
        private IValueProvider<Vector2> rangeGetter;

        /// <summary>
        /// Initializes the drawer.
        /// </summary>
        protected override void Initialize()
        {
            var context = ValueResolverUtility.CreateContext(this);
            this.minGetter = scalarResolver.Resolve(context, this.Attribute.MinMember, this.Attribute.MinValue);
            this.maxGetter = scalarResolver.Resolve(context, this.Attribute.MaxMember, this.Attribute.MaxValue);
            this.rangeGetter = vectorResolver.Resolve(context, this.Attribute.MinMaxMember);

            this.errorMessage = ValueResolverUtility.CombineErrorMessagesWhereFailed(this.minGetter, this.maxGetter, this.rangeGetter);

            //MemberInfo member;

            //// Min member reference.
            //if (string.IsNullOrEmpty(this.Attribute.MinMember) == false)
            //{
            //    if (this.Attribute.MinMember[0] == '$')
            //    {
            //        // Look for an expression.
            //        this.floatMinGetter = new InspectorPropertyValueGetter<float>(this.Property, this.Attribute.MinMember);
            //    }
            //    else if (MemberFinder.Start(this.Property.ParentType)
            //        .IsNamed(this.Attribute.MinMember)
            //        .HasNoParameters()
            //        .TryGetMember(out member, out this.errorMessage))
            //    {
            //        var type = member.GetReturnType();
            //        if (type == typeof(int))
            //        {
            //            this.intMinGetter = new InspectorPropertyValueGetter<int>(this.Property, this.Attribute.MinMember);
            //        }
            //        else if (type == typeof(float))
            //        {
            //            this.floatMinGetter = new InspectorPropertyValueGetter<float>(this.Property, this.Attribute.MinMember);
            //        }
            //    }
            //}

            //// Max member reference.
            //if (string.IsNullOrEmpty(this.Attribute.MaxMember) == false)
            //{
            //    if (this.Attribute.MaxMember[0] == '$')
            //    {
            //        // Look for an expression.
            //        this.floatMaxGetter = new InspectorPropertyValueGetter<float>(this.Property, this.Attribute.MaxMember);
            //    }
            //    else if (MemberFinder.Start(this.Property.ParentType)
            //        .IsNamed(this.Attribute.MaxMember)
            //        .HasNoParameters()
            //        .TryGetMember(out member, out this.errorMessage))
            //    {
            //        var type = member.GetReturnType();
            //        if (type == typeof(int))
            //        {
            //            this.intMaxGetter = new InspectorPropertyValueGetter<int>(this.Property, this.Attribute.MaxMember);
            //        }
            //        else if (type == typeof(float))
            //        {
            //            this.floatMaxGetter = new InspectorPropertyValueGetter<float>(this.Property, this.Attribute.MaxMember);
            //        }
            //    }
            //}

            //// Min max member reference.
            //if (string.IsNullOrEmpty(this.Attribute.MinMaxMember) == false)
            //{
            //    this.vector2MinMaxGetter = new InspectorPropertyValueGetter<Vector2>(this.Property, this.Attribute.MinMaxMember);
            //    if (this.errorMessage != null)
            //    {
            //        this.errorMessage = this.vector2MinMaxGetter.ErrorMessage;
            //    }
            //}
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            //Vector2 range;
            //if (this.vector2MinMaxGetter != null && this.errorMessage == null)
            //{
            //    range = this.vector2MinMaxGetter.GetValue();
            //}
            //else
            //{
            //    if (this.intMinGetter != null)
            //    {
            //        range.x = this.intMinGetter.GetValue();
            //    }
            //    else if (this.floatMinGetter != null)
            //    {
            //        range.x = this.floatMinGetter.GetValue();
            //    }
            //    else
            //    {
            //        range.x = this.Attribute.MinValue;
            //    }

            //    if (this.intMaxGetter != null)
            //    {
            //        range.y = this.intMaxGetter.GetValue();
            //    }
            //    else if (this.floatMaxGetter != null)
            //    {
            //        range.y = this.floatMaxGetter.GetValue();
            //    }
            //    else
            //    {
            //        range.y = this.Attribute.MaxValue;
            //    }
            //}
            Vector2 range = this.rangeGetter.GetValue();

            if (this.errorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.errorMessage);
            }

            this.ValueEntry.SmartValue = SirenixEditorFields.MinMaxSlider(label, this.ValueEntry.SmartValue, range, this.Attribute.ShowFields);
        }
    }
}
#endif
#pragma warning enable