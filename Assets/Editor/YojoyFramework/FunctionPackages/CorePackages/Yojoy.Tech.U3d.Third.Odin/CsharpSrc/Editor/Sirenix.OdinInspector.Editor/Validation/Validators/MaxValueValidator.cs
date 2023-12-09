#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="MaxValueValidator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.MaxValueValidator<>))]

namespace Sirenix.OdinInspector.Editor.Validation
{
    using Sirenix.OdinInspector.Editor.ValueResolver;
    using System;
    using System.Reflection;

    public class MaxValueValidator<T> : AttributeValidator<MaxValueAttribute, T>
        where T : struct
    {
        private static IValueResolver<double> ValueResolver;

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

            this.maxValueGetter = ValueResolver.Resolve(context, this.Attribute.Expression, context.Validator.Attribute.MaxValue);
        }

        protected override void Validate(object parentInstance, T memberValue, MemberInfo member, ValidationResult result)
        {
            if (this.maxValueGetter.Failed)
            {
                result.Message = this.maxValueGetter.GetNiceErrorMessage();
                result.ResultType = ValidationResultType.Error;
                return;
            }

            this.valueExpressionArgument = memberValue;
            var max = this.maxValueGetter.GetValue(parentInstance);

            if (!GenericNumberUtility.NumberIsInRange(memberValue, double.MinValue, max))
            {
                result.Message = "Number is larger than " + max + ".";
                result.ResultType = ValidationResultType.Error;
            }
        }
    }
}
#endif
#pragma warning enable