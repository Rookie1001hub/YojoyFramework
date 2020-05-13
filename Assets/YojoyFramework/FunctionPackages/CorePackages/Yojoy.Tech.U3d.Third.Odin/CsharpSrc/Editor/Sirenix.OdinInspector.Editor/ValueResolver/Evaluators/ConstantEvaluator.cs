#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ConstantEvaluator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    using System;

#if SIRENIX_INTERNAL
    public class ConstantEvaluator<TContext, TResult> : ProviderEvaluator<TContext, TResult> where TContext : ResolverContext
#else
    internal class ConstantEvaluator<TContext, TResult> : ProviderEvaluator<TContext, TResult> where TContext : ResolverContext
#endif
    {
        private Func<TContext, TResult> getValueFunc;
        private TResult rawValue;

        public ConstantEvaluator(Func<TContext, TResult> getValue)
        {
            this.getValueFunc = getValue;
        }

        public ConstantEvaluator(TResult value)
        {
            this.rawValue = value;
        }

        public override ValueProvider<TResult> TryEvaluate(TContext context, string reference)
        {
            return new ConstantValueProvider<TResult>(context, this.getValueFunc != null ? this.getValueFunc(context) : this.rawValue);
        }
    }
}
#endif
#pragma warning enable