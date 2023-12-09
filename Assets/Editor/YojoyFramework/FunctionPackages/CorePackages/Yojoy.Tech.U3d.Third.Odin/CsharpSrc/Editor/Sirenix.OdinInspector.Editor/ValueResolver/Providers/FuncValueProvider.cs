#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="FuncValueProvider.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    using System;

    internal class FuncValueProvider<TResult> : ValueProvider<TResult>
    {
        private readonly bool requiresParentInstance;
        private readonly Func<object, TResult> func;

        public override bool RequiresParentInstance
        {
            get { return this.requiresParentInstance; }
        }

        public FuncValueProvider(ResolverContext context, bool requiresParentInstance, Func<object, TResult> func) : base(context)
        {
            this.requiresParentInstance = requiresParentInstance;
            this.func = func;
        }

        public override TResult GetValue(object instance)
        {
            return this.func(instance);
        }
    }
}
#endif
#pragma warning enable