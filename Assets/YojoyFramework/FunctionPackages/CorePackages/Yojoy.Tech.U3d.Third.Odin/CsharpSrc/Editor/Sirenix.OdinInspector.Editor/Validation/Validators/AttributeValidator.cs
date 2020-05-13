#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="AttributeValidator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Validation
{
    using System;
    using System.Reflection;
    using UnityEngine;

    internal interface IAttributeValidator
    {
        void SetAttributeInstance(Attribute attribute);
    }

    public abstract class AttributeValidator<TAttribute> : Validator, IAttributeValidator
        where TAttribute : Attribute
    {
        public TAttribute Attribute { get; private set; }

        public sealed override bool CanValidateMembers()
        {
            return true;
        }

        public sealed override bool CanValidateValues()
        {
            return false;
        }

        public sealed override bool CanValidateValue(Type type)
        {
            return false;
        }

        public sealed override void Initialize(Type type)
        {
            throw new NotSupportedException("Attribute validators cannot validate values without members");
        }

        public sealed override void RunValueValidation(object value, UnityEngine.Object root, ref ValidationResult result)
        {
            throw new NotSupportedException("Attribute validators cannot validate values without members");
        }

        public sealed override void RunMemberValidation(object parentInstance, MemberInfo member, object memberValue, UnityEngine.Object root, ref ValidationResult result)
        {
            if (result == null)
                result = new ValidationResult();

            result.Setup = new ValidationSetup()
            {
                Kind = ValidationKind.Member,
                Validator = this,
                Member = member,
                ParentInstance = parentInstance,
                Value = memberValue,
                Root = root,
            };

            result.ResultValue = null;
            result.ResultType = ValidationResultType.Valid;
            result.Message = "";

            try
            {
                this.Validate(parentInstance, memberValue, member, result);
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

        protected abstract void Validate(object parentInstance, object memberValue, MemberInfo member, ValidationResult result);

        void IAttributeValidator.SetAttributeInstance(Attribute attribute)
        {
            this.Attribute = (TAttribute)attribute;
        }
    }

    public abstract class AttributeValidator<TAttribute, TValue> : AttributeValidator<TAttribute>
        where TAttribute : Attribute
    {
        protected sealed override void Validate(object parentInstance, object memberValue, MemberInfo member, ValidationResult result)
        {
            this.Validate(parentInstance, (TValue)memberValue, member, result);
        }

        protected abstract void Validate(object parentInstance, TValue memberValue, MemberInfo member, ValidationResult result);
    }
}
#endif
#pragma warning enable