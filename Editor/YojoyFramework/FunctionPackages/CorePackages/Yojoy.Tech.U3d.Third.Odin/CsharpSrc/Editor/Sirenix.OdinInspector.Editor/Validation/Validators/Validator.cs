#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="Validator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Validation
{
    using System;
    using System.Reflection;

    public abstract class Validator
    {
        public virtual RevalidationCriteria RevalidationCriteria { get { return RevalidationCriteria.Always; } }

        public virtual bool CanValidateValues()
        {
            return true;
        }

        public virtual bool CanValidateValue(Type type)
        {
            return true;
        }

        public virtual bool CanValidateMembers()
        {
            return true;
        }

        public virtual bool CanValidateMember(MemberInfo member, Type memberValueType)
        {
            return true;
        }

        public virtual void RunValueValidation(object value, UnityEngine.Object root, ref ValidationResult result)
        {
        }

        public virtual void RunMemberValidation(object parentInstance, MemberInfo member, object memberValue, UnityEngine.Object root, ref ValidationResult result)
        {
        }

        public virtual void Initialize(Type type)
        {
        }

        public virtual void Initialize(MemberInfo member, Type memberValueType)
        {
        }
    }

}
#endif
#pragma warning enable