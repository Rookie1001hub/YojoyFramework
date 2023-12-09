#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ValueValidator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Validation
{
    using System;
    using System.Reflection;
    using UnityEngine;

    public abstract class ValueValidator<TValue> : Validator
    {
        public sealed override bool CanValidateValues()
        {
            return true;
        }

        public sealed override bool CanValidateMembers()
        {
            return false;
        }

        public sealed override bool CanValidateMember(MemberInfo member, Type memberValueType)
        {
            return false;
        }

        public sealed override void Initialize(MemberInfo member, Type memberValueType)
        {
            throw new NotSupportedException("Value validators cannot validate members");
        }

        public sealed override void RunMemberValidation(object parentInstance, MemberInfo member, object memberValue, UnityEngine.Object root, ref ValidationResult result)
        {
            throw new NotSupportedException("Value validators cannot validate members");
        }

        public sealed override void RunValueValidation(object value, UnityEngine.Object root, ref ValidationResult result)
        {
            if (result == null)
                result = new ValidationResult();

            result.Setup = new ValidationSetup()
            {
                Kind = ValidationKind.Value,
                Validator = this,
                Value = value,
                Root = root,
            };

            result.ResultValue = null;
            result.ResultType = ValidationResultType.Valid;
            result.Message = "";

            try
            {
                this.Validate((TValue)value, result);
            }
            catch (Exception ex)
            {
                while (ex is TargetInvocationException)
                {
                    ex = ex.InnerException;
                }

                result.ResultType = ValidationResultType.Error;
                result.Message = "An exception was thrown during validation: " + ex.ToString();
            }
}

        protected abstract void Validate(TValue value, ValidationResult result);
    }
}
#endif
#pragma warning enable