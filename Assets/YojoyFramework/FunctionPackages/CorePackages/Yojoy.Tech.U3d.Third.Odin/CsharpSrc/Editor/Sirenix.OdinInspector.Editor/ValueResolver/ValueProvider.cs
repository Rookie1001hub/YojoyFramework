#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ValueProvider.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    using System;
    using System.Collections.Generic;
    using System.Text;

#if SIRENIX_INTERNAL
    public interface IValueProvider
#else
    internal interface IValueProvider
#endif
    {
        bool Failed { get; }
        bool RequiresParentInstance { get; }
        object DefaultParentInstance { get;  set; }
        object GetValueWeak();
        object GetValueWeak(object parentInstance);
        string GetNiceErrorMessage();
    }

#if SIRENIX_INTERNAL
    public interface IValueProvider<TResult> : IValueProvider
#else
    internal interface IValueProvider<TResult> : IValueProvider
#endif
    {
        TResult GetValue();
        TResult GetValue(object parentInstance);
    }

#if SIRENIX_INTERNAL
    public abstract class ValueProvider<TResult> : IValueProvider<TResult>
#else
    internal abstract class ValueProvider<TResult> : IValueProvider<TResult>
#endif
    {
        private List<EvaluatorFailInfo> failures;
        private string niceErrorMessage;
        protected readonly ResolverContext Context;

        public virtual bool Failed { get { return false; } }

        public abstract bool RequiresParentInstance { get; }

        public object DefaultParentInstance { get; set; }

        public ValueProvider(ResolverContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.Context = context;
            this.failures = this.Context.GetFailInfos();
        }
        internal ValueProvider(ResolverContext context, List<EvaluatorFailInfo> failures)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.Context = context;
            this.failures = failures;
        }

        public TResult GetValue()
        {
            object instance = null;
            if (this.RequiresParentInstance)
            {
                instance = this.DefaultParentInstance ?? this.Context.GetParentInstance();

                if (instance == null)
                {
                    throw new ArgumentNullException("Parent instance cannot be null.");
                }
            }

            return this.GetValue(instance);
        }

        public abstract TResult GetValue(object instance);

        object IValueProvider.GetValueWeak()
        {
            return this.GetValue();
        }
        object IValueProvider.GetValueWeak(object parentInstance)
        {
            return this.GetValue(parentInstance);
        }

        public string GetNiceErrorMessage()
        {
            if (this.niceErrorMessage == null)
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < this.failures.Count; i++)
                {
                    builder.AppendLine(this.failures[i].Message.Trim());
                }

                this.niceErrorMessage = builder.ToString();
            }

            return this.niceErrorMessage;
        }
    }
}
#endif
#pragma warning enable