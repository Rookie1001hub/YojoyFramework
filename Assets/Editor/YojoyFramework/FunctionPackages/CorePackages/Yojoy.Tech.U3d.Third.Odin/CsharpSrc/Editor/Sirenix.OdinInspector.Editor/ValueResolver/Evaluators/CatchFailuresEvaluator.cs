#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="CatchFailuresEvaluator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    internal class CatchFailuresEvaluator<TContext, TResult> : ProviderEvaluator<TContext, TResult> where TContext : ResolverContext
    {
        public override ValueProvider<TResult> TryEvaluate(TContext context, string reference)
        {
            if (context.HasAnyFails)
            {
                return new FailedValueProvider<TResult>(context);
            }
            else
            {
                return null;
            }
        }
    }
}
#endif
#pragma warning enable