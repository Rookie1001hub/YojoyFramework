#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="MinMaxSliderValidator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.MinMaxSliderValidator<>))]

namespace Sirenix.OdinInspector.Editor.Validation
{
    using Sirenix.OdinInspector.Editor.ValueResolver;
    using System;
    using System.Reflection;
    using UnityEngine;

    public class MinMaxSliderValidator<T> : AttributeValidator<MinMaxSliderAttribute, T>
        where T : struct
    {
        private static IValueResolver<double> ValueResolver;
        private static IValueResolver<Vector2> RangeResolver;

        private ValueProvider<double> minValueGetter;
        private ValueProvider<double> maxValueGetter;
        private ValueProvider<Vector2> rangeGetter;

        private T valueExpressionArgument;

        public override bool CanValidateMember(MemberInfo member, Type memberValueType)
        {
            return GenericNumberUtility.IsNumber(memberValueType);
        }

        public override void Initialize(MemberInfo member, Type memberValueType)
        {
            if (ValueResolver == null)
            {
                ValueResolver = ValueResolverUtility.GetResolver<double>()
                    .TryMemberReference()
                    .TryExpression();

                RangeResolver = ValueResolverUtility.GetValidatorResolver<MinMaxSliderValidator<T>, Vector2>()
                    .TryMemberReference()
                    .TryExpression()
                    .Func(c => new Vector2((float)c.Validator.minValueGetter.GetValue(), (float)c.Validator.maxValueGetter.GetValue()));
            }

            var context = ValueResolverUtility.CreateContext(this, member);

            context.AddExpressionParameter<T>("value", () => this.valueExpressionArgument);

            this.minValueGetter = ValueResolver.Resolve(context, this.Attribute.MinMember, context.Validator.Attribute.MinValue);
            this.maxValueGetter = ValueResolver.Resolve(context, this.Attribute.MaxMember, context.Validator.Attribute.MaxValue);
            this.rangeGetter = RangeResolver.Resolve(context, this.Attribute.MinMaxMember);
        }

        protected override void Validate(object parentInstance, T memberValue, MemberInfo member, ValidationResult result)
        {
            if (this.minValueGetter.Failed || this.maxValueGetter.Failed || this.rangeGetter.Failed)
            {
                result.Message = ValueResolverUtility.CombineErrorMessagesWhereFailed(this.minValueGetter, this.maxValueGetter, this.rangeGetter);
                result.ResultType = ValidationResultType.Error;
                return;
            }

            this.valueExpressionArgument = memberValue;
            this.minValueGetter.DefaultParentInstance = parentInstance;
            this.maxValueGetter.DefaultParentInstance = parentInstance;
            var range = this.rangeGetter.GetValue(parentInstance);

            if (!GenericNumberUtility.NumberIsInRange(memberValue, range.x, range.y))
            {
                result.Message = "Number is not in range.";
                result.ResultType = ValidationResultType.Error;
            }
        }
    }
}
#endif
#pragma warning enable