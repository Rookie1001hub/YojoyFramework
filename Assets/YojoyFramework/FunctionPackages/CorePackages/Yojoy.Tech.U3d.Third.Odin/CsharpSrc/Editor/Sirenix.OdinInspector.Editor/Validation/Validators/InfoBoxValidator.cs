#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="InfoBoxValidator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.InfoBoxValidator))]

namespace Sirenix.OdinInspector.Editor.Validation
{
    using System;
    using System.Reflection;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor.ValueResolver;

    [NoValidationInInspector]
    public class InfoBoxValidator : AttributeValidator<InfoBoxAttribute>
    {
        private static readonly IValueResolver<bool> BoolValueResolver = ValueResolverUtility.GetResolver<bool>()
            .TryMemberReference()
            .TryExpression();

        private static readonly IValueResolver<string> MessageValueResolver = ValueResolverUtility.GetValidatorResolver<InfoBoxValidator, string>()
            .TryMemberReference()
            .TryExpression()
            .CatchFailures()
            .ConstantValue(context => context.Validator.Attribute.Message);

        private IValueProvider<bool> showMessageGetter;
        private IValueProvider<string> messageGetter;

        public override void Initialize(MemberInfo member, Type memberValueType)
        {
            var contextReqSymbol = ValueResolverUtility.CreateContext(this, member);
            var contextNoReqSymbol = ValueResolverUtility.CreateContext(this, member);

            this.messageGetter = MessageValueResolver.Resolve(contextReqSymbol, this.Attribute.Message);
            this.showMessageGetter = BoolValueResolver.Resolve(contextNoReqSymbol, this.Attribute.VisibleIf, true);
        }

        protected override void Validate(object parentInstance, object memberValue, MemberInfo member, ValidationResult result)
        {
            if (this.showMessageGetter.Failed || this.messageGetter.Failed)
            {
                result.ResultType = ValidationResultType.Error;
                result.Message = ValueResolverUtility.CombineErrorMessagesWhereFailed(this.showMessageGetter, this.messageGetter);
                return;
            }
            
            bool showMessage = this.showMessageGetter.GetValue(parentInstance);

            if (showMessage)
            {
                result.ResultType = this.Attribute.InfoMessageType.ToValidationResultType();
                result.Message = this.messageGetter.GetValue(parentInstance);
            }
        }
    }
}
#endif
#pragma warning enable