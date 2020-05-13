#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="RequiredValidator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.RequiredValidator))]

namespace Sirenix.OdinInspector.Editor.Validation
{
    using System;
    using System.Reflection;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    
    public class RequiredValidator : AttributeValidator<RequiredAttribute>
    {
        private StringMemberHelper stringHelper;

        public override void Initialize(MemberInfo member, Type memberValueType)
        {
            if (this.Attribute.ErrorMessage != null)
            {
                this.stringHelper = new StringMemberHelper(member.ReflectedType, false, this.Attribute.ErrorMessage);
            }
        }

        protected override void Validate(object parentInstance, object memberValue, MemberInfo member, ValidationResult result)
        {
            if (!this.IsValid(memberValue))
            {
                result.ResultType = this.Attribute.MessageType.ToValidationResultType();
                result.Message = this.stringHelper != null ? this.stringHelper.GetString(parentInstance) : (member.Name + " is required");
            }
        }

        private bool IsValid(object memberValue)
        {
            if (object.ReferenceEquals(memberValue, null))
                return false;

            if (memberValue is string && string.IsNullOrEmpty((string)memberValue))
                return false;

            if (memberValue is UnityEngine.Object && (memberValue as UnityEngine.Object) == null)
                return false;

            return true;
        }
    }
}
#endif
#pragma warning enable