#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ValueResolverUtility.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    using System.Reflection;
    using System.Text;

#if SIRENIX_INTERNAL
    public static class ValueResolverUtility
#else
    internal static class ValueResolverUtility
#endif
    {
        public static ValueResolver<ResolverContext, TResult> GetResolver<TResult>()
        {
            return new ValueResolver<ResolverContext, TResult>(ValueResolverSettings.Default<TResult>());
        }

        public static ValueResolver<ResolverContext, TResult> GetResolver<TResult>(ValueResolverSettings settings)
        {
            return new ValueResolver<ResolverContext, TResult>(settings);
        }

        public static ValueResolver<TContext, TResult> GetResolver<TContext, TResult>() where TContext : ResolverContext
        {
            return new ValueResolver<TContext, TResult>(ValueResolverSettings.Default<TResult>());
        }

        public static ValueResolver<TContext, TResult> GetResolver<TContext, TResult>(ValueResolverSettings settings) where TContext : ResolverContext
        {
            return new ValueResolver<TContext, TResult>(settings);
        }

        public static DrawerValueResolver<TDrawer, TResult> GetDrawerResolver<TDrawer, TResult>() where TDrawer: OdinDrawer
        {
            return new DrawerValueResolver<TDrawer, TResult>(ValueResolverSettings.Default<TResult>());
        }

        public static DrawerValueResolver<TDrawer, TResult> GetDrawerResolver<TDrawer, TResult>(ValueResolverSettings settings) where TDrawer : OdinDrawer
        {
            return new DrawerValueResolver<TDrawer, TResult>(settings);
        }

        public static DrawerValueResolver<OdinDrawer, TResult> GetDrawerResolver<TResult>()
        {
            return new DrawerValueResolver<OdinDrawer, TResult>(ValueResolverSettings.Default<TResult>());
        }

        public static DrawerValueResolver<OdinDrawer, TResult> GetDrawerResolver<TResult>(ValueResolverSettings settings)
        {
            return new DrawerValueResolver<OdinDrawer, TResult>(settings);
        }

        public static ValidatorValueResolver<TValidator, TResult> GetValidatorResolver<TValidator, TResult>() where TValidator: Validation.Validator
        {
            return new ValidatorValueResolver<TValidator, TResult>(ValueResolverSettings.Default<TResult>());
        }

        public static ValidatorValueResolver<TValidator, TResult> GetValidatorResolver<TValidator, TResult>(ValueResolverSettings settings) where TValidator : Validation.Validator
        {
            return new ValidatorValueResolver<TValidator, TResult>(settings);
        }

        public static ValidatorResolverContext<TValidator> CreateContext<TValidator>(TValidator validator, MemberInfo member)
            where TValidator : Validation.Validator
        {
            return new ValidatorResolverContext<TValidator>(validator, member);
        }

        public static DrawerResolverContext<TDrawer> CreateContext<TDrawer>(TDrawer drawer)
            where TDrawer : OdinDrawer
        {
            return new DrawerResolverContext<TDrawer>(drawer);
        }

        public static string CombineErrorMessagesWhereFailed(params IValueProvider[] providers)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < providers.Length; i++)
            {
                if (providers[i].Failed)
                {
                    builder.AppendLine(providers[i].GetNiceErrorMessage().Trim());
                }
            }

            return builder.Length > 0
                ? builder.ToString().Trim()
                : null;
        }
    }
}
#endif
#pragma warning enable