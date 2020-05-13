#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="RangeValidator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.RangeValidator<>))]
[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.PropertyRangeValidator<>))]

namespace Sirenix.OdinInspector.Editor.Validation
{
    using Sirenix.OdinInspector.Editor.ValueResolver;
    using System;
    using System.Reflection;
    using UnityEngine;

    public class RangeValidator<T> : AttributeValidator<RangeAttribute, T> where T : struct
    {
        public override bool CanValidateMember(MemberInfo member, Type memberValueType)
        {
            return GenericNumberUtility.IsNumber(memberValueType);
        }

        protected override void Validate(object parentInstance, T memberValue, MemberInfo member, ValidationResult result)
        {
            if (!GenericNumberUtility.NumberIsInRange(memberValue, this.Attribute.min, this.Attribute.max))
            {
                result.Message = "Number is not in range.";
                result.ResultType = ValidationResultType.Error;
            }
        }
    }


    public class PropertyRangeValidator<T> : AttributeValidator<PropertyRangeAttribute, T>
        where T : struct
    {
        private static IValueResolver<double> ValueResolver;
        
        private ValueProvider<double> minValueGetter;
        private ValueProvider<double> maxValueGetter;
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
            }

            var context = ValueResolverUtility.CreateContext(this, member);

            context.AddExpressionParameter<T>("value", () => this.valueExpressionArgument);

            this.minValueGetter = ValueResolver.Resolve(context, this.Attribute.MinMember, context.Validator.Attribute.Min);
            this.maxValueGetter = ValueResolver.Resolve(context, this.Attribute.MaxMember, context.Validator.Attribute.Max);
        }

        protected override void Validate(object parentInstance, T memberValue, MemberInfo member, ValidationResult result)
        {
            if (this.minValueGetter.Failed || this.maxValueGetter.Failed)
            {
                result.Message = ValueResolverUtility.CombineErrorMessagesWhereFailed(this.minValueGetter, this.maxValueGetter);
                result.ResultType = ValidationResultType.Error;
                return;
            }

            this.valueExpressionArgument = memberValue;
            var min = this.minValueGetter.GetValue(parentInstance);
            var max = this.maxValueGetter.GetValue(parentInstance);

            if (!GenericNumberUtility.NumberIsInRange(memberValue, min, max))
            {
                result.Message = "Number is not in range.";
                result.ResultType = ValidationResultType.Error;
            }
        }
    }
}
#endif
#pragma warning enable