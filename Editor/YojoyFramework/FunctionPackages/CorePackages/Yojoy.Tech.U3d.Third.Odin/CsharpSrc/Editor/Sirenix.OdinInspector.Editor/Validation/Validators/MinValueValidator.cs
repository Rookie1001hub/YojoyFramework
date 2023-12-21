#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="MinValueValidator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.MinValueValidator<>))]

namespace Sirenix.OdinInspector.Editor.Validation
{
    using Sirenix.OdinInspector.Editor.ValueResolver;
    using System;
    using System.Reflection;

    public class MinValueValidator<T> : AttributeValidator<MinValueAttribute, T>
        where T : struct
    {
        private static IValueResolver<double> ValueResolver;

        private ValueProvider<double> minValueGetter;
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

            this.minValueGetter = ValueResolver.Resolve(context, this.Attribute.Expression, context.Validator.Attribute.MinValue);
        }

        protected override void Validate(object parentInstance, T memberValue, MemberInfo member, ValidationResult result)
        {
            if (this.minValueGetter.Failed)
            {
                result.Message = this.minValueGetter.GetNiceErrorMessage();
                result.ResultType = ValidationResultType.Error;
                return;
            }

            this.valueExpressionArgument = memberValue;
            var min = this.minValueGetter.GetValue(parentInstance);

            if (!GenericNumberUtility.NumberIsInRange(memberValue, min, double.MaxValue))
            {
                result.Message = "Number is smaller than " + min + ".";
                result.ResultType = ValidationResultType.Error;
            }
        }
    }
}
#endif
#pragma warning enable