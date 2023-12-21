#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="FailedValueProvider.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    using System.Collections.Generic;

    internal class FailedValueProvider<TResult> : ValueProvider<TResult>
    {
        public override bool Failed { get { return true; } }

        public override bool RequiresParentInstance { get { return false; } }

        public FailedValueProvider(ResolverContext context) : base(context)
        {
        }
        public FailedValueProvider(ResolverContext context, List<EvaluatorFailInfo> failures) : base(context, failures)
        {
        }

        public override TResult GetValue(object instance)
        {
            return default(TResult);
        }
    }
}
#endif
#pragma warning enable