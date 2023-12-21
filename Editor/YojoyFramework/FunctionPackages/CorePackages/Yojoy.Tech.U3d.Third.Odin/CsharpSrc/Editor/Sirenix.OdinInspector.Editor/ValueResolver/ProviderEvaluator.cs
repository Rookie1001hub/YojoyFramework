#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ProviderEvaluator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    using System;
    using Sirenix.Utilities;

#if SIRENIX_INTERNAL
    public interface IProviderEvaluator
#else
    internal interface IProviderEvaluator
#endif
    {
        ValueResolverSettings Settings { get;  set; }
        IValueProvider TryEvaluateWeak(ResolverContext context, string reference);
    }

#if SIRENIX_INTERNAL
    public interface IProviderEvaluator<TResult> : IProviderEvaluator
#else
    internal interface IProviderEvaluator<TResult> : IProviderEvaluator
#endif
    {
        ValueProvider<TResult> TryEvaluate(ResolverContext context, string reference);
    }

#if SIRENIX_INTERNAL
    public interface IProviderEvaluator<TContext, TResult> : IProviderEvaluator<TResult>
#else
    internal interface IProviderEvaluator<TContext, TResult> : IProviderEvaluator<TResult>
#endif
        where TContext : ResolverContext
    {
        ValueProvider<TResult> TryEvaluate(TContext context, string reference);
    }

#if SIRENIX_INTERNAL
    public abstract class ProviderEvaluator<TContext, TResult> : IProviderEvaluator<TContext, TResult>
#else
    internal abstract class ProviderEvaluator<TContext, TResult> : IProviderEvaluator<TContext, TResult>
#endif
        where TContext : ResolverContext
    {
        public ValueResolverSettings Settings { get; set; }

        protected bool IsStatic(TContext context)
        {
            // TODO: This probably just be context.IsStatic
            //return (drawer.Property.Info.GetMemberInfo() == null || drawer.Property.Info.GetMemberInfo().IsStatic());
            return false;
        }

        public abstract ValueProvider<TResult> TryEvaluate(TContext context, string reference);

        ValueProvider<TResult> IProviderEvaluator<TResult>.TryEvaluate(ResolverContext context, string reference)
        {
            if (context is TContext == false)
            {
                throw new InvalidOperationException("Context must be of type " + typeof(TContext).GetNiceName());
            }

            return this.TryEvaluate(context as TContext, reference);
        }

        IValueProvider IProviderEvaluator.TryEvaluateWeak(ResolverContext context, string reference)
        {
            if (context is TContext == false)
            {
                throw new InvalidOperationException("Context must be of type " + typeof(TContext).GetNiceName());
            }

            return this.TryEvaluate(context as TContext, reference);
        }
    }
}
#endif
#pragma warning enable