#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="DetailedInfoBoxValidator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.DetailedInfoBoxValidator))]

namespace Sirenix.OdinInspector.Editor.Validation
{
    using System;
    using System.Reflection;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor.ValueResolver;

    [NoValidationInInspector]
    public class DetailedInfoBoxValidator : AttributeValidator<DetailedInfoBoxAttribute>
    {
        private static readonly IValueResolver<bool> BoolValueResolver = ValueResolverUtility.GetResolver<bool>()
            .TryMemberReference()
            .TryExpression();

        private static readonly IValueResolver<string> MessageValueResolver = ValueResolverUtility.GetValidatorResolver<DetailedInfoBoxValidator, string>()
            .TryMemberReference()
            .TryExpression();

        private IValueProvider<bool> showMessageGetter;
        private IValueProvider<string> messageGetter;
        private IValueProvider<string> detailsGetter;
        
        public override void Initialize(MemberInfo member, Type memberValueType)
        {
            var contextReqSymbol = ValueResolverUtility.CreateContext(this, member);
            var contextNoReqSymbol = ValueResolverUtility.CreateContext(this, member);

            //contextReqSymbol.RequireSymbolForMemberReferences = true;

            this.messageGetter = MessageValueResolver.Resolve(contextReqSymbol, this.Attribute.Message, this.Attribute.Message);
            this.detailsGetter = MessageValueResolver.Resolve(contextReqSymbol, this.Attribute.Details, this.Attribute.Details);
            this.showMessageGetter = BoolValueResolver.Resolve(contextNoReqSymbol, this.Attribute.VisibleIf, true);
        }

        protected override void Validate(object parentInstance, object memberValue, MemberInfo member, ValidationResult result)
        {
            if (this.showMessageGetter.Failed || this.messageGetter.Failed || this.showMessageGetter.Failed)
            {
                result.ResultType = ValidationResultType.Error;
                result.Message = ValueResolverUtility.CombineErrorMessagesWhereFailed(showMessageGetter, messageGetter, detailsGetter);
                return;
            }
            
            bool hasMessage = this.showMessageGetter.GetValue(parentInstance);

            if (hasMessage)
            {
                result.ResultType = this.Attribute.InfoMessageType.ToValidationResultType();
                result.Message = this.messageGetter.GetValue(parentInstance) + "\n\nDETAILS:\n\n" + this.detailsGetter.GetValue(parentInstance);
            }
        }
    }
}
#endif
#pragma warning enable