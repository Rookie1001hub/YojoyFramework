#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ConstantValueProvider.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    internal class ConstantValueProvider<TResult> : ValueProvider<TResult>
    {
        private readonly TResult value;

        public override bool RequiresParentInstance { get { return false; } }

        public ConstantValueProvider(ResolverContext context, TResult value) : base(context)
        {
            this.value = value;
        }

        public override TResult GetValue(object instance)
        {
            return this.value;
        }
    }
}
#endif
#pragma warning enable