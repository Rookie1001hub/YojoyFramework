#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="FuncEvaluator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    using System;

    internal class FuncEvaluator<TContext, TResult> : ProviderEvaluator<TContext, TResult> where TContext : ResolverContext
    {
        private Func<TContext, TResult> getValue;

        public FuncEvaluator(Func<TContext, TResult> getValue)
        {
            this.getValue = getValue;
        }

        public override ValueProvider<TResult> TryEvaluate(TContext context, string reference)
        {
            return new FuncValueProvider<TResult>(context, false, o => this.getValue(context));
        }
    }
}
#endif
#pragma warning enable