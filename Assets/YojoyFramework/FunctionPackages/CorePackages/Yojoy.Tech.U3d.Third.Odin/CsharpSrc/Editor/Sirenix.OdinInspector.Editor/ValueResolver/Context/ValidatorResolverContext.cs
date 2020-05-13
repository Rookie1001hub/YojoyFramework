#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ValidatorResolverContext.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    using System;
    using System.Reflection;
    using Sirenix.OdinInspector.Editor.Validation;

#if SIRENIX_INTERNAL
    public class ValidatorResolverContext<TValidator> : ResolverContext
#else
    internal class ValidatorResolverContext<TValidator> : ResolverContext
#endif
        where TValidator : Validator
    {
        private readonly MemberInfo member;
        private readonly object parentInstance;

        public readonly TValidator Validator;

        public ValidatorResolverContext(TValidator validator, MemberInfo member) : this(validator, member, null)
        {
        }

        public ValidatorResolverContext(TValidator validator, MemberInfo member, object parentInstance)
        {
            if (validator == null)
            {
                throw new ArgumentNullException("validator");
            }
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            this.Validator = validator;
            this.member = member;
            this.parentInstance = parentInstance;
        }

        public override object GetParentInstance()
        {
            return this.parentInstance;
        }
        public override Type GetParentType()
        {
            return this.member.DeclaringType; // TODO: Is this good enough?
        }
    }
}
#endif
#pragma warning enable