#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ValueResolver.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    using Sirenix.Utilities;
    using System;
    using System.Collections.Generic;

#if SIRENIX_INTERNAL
    public interface IValueResolver
#else
    internal interface IValueResolver
#endif
    {
        int Count { get; }

        IValueResolver Add(IProviderEvaluator resolver);
        IValueProvider ResolveWeak(ResolverContext context, string reference);
        IValueProvider ResolveWeak(ResolverContext context, string reference, object defaultValue);
    }

#if SIRENIX_INTERNAL
    public interface IValueResolver<TResult> : IValueResolver
#else
    internal interface IValueResolver<TResult> : IValueResolver
#endif
    {
        ValueProvider<TResult> Resolve(ResolverContext context, string reference);
        ValueProvider<TResult> Resolve(ResolverContext context, string reference, TResult defaultValue);
    }

#if SIRENIX_INTERNAL
    public interface IValueResolver<TContext, TResult> : IValueResolver<TResult>
#else
    internal interface IValueResolver<TContext, TResult> : IValueResolver<TResult>
#endif
        where TContext : ResolverContext
    {
        ValueProvider<TResult> Resolve(TContext context, string reference);
        ValueProvider<TResult> Resolve(TContext context, string reference, TResult defaultValue);
    }

#if SIRENIX_INTERNAL
    public abstract class BaseValueResolver<TResolver, TContext, TResult> : IValueResolver<TContext, TResult>
#else
    internal abstract class BaseValueResolver<TResolver, TContext, TResult> : IValueResolver<TContext, TResult>
#endif
        where TResolver : BaseValueResolver<TResolver, TContext, TResult>
        where TContext : ResolverContext
    {
        private readonly List<ProviderEvaluator<TContext, TResult>> evaluators = new List<ProviderEvaluator<TContext, TResult>>();

        private readonly ValueResolverSettings settings;

        public BaseValueResolver(ValueResolverSettings settings)
        {
            this.settings = settings;
        }

        public int Count { get { return this.evaluators.Count; } }

        public Type ContextType { get { return typeof(TContext); } }

        public Type ResultType { get { return typeof(TResult); } }

        public ValueProvider<TResult> Resolve(TContext context, string reference)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Reset();

            for (int i = 0; i < this.evaluators.Count; i++)
            {
                try
                {
                    this.evaluators[i].Settings = this.settings;
                    var result = this.evaluators[i].TryEvaluate(context, reference);
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    context.LogFailInfoFormat(this.evaluators[i], "Exception happened with evaluating '" + reference + "' with evaluator: '" + evaluators[i].GetType().GetNiceName() + "'.\n" + ex.Message + "\n" + ex.StackTrace);
                    break;
                }
            }

            var failures = context.GetFailInfos();
            if (failures.Count == 0)
            {
                if (this.evaluators.Count == 0)
                {
                    failures.Add(new EvaluatorFailInfo(null, "Failed to resolver '" + reference + "' because the resolver has no evaluators."));
                }
                else
                {
                    failures.Add(new EvaluatorFailInfo(null, "Failed to resolver '" + reference + "' but no error was found."));
                }
            }

            return new FailedValueProvider<TResult>(context, failures);
        }

        public ValueProvider<TResult> Resolve(TContext context, string reference, TResult defaultValue)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Reset();

            if (string.IsNullOrEmpty(reference))
            {
                return new ConstantValueProvider<TResult>(context, defaultValue);
            }
            else
            {
                for (int i = 0; i < this.evaluators.Count; i++)
                {
                    try
                    {
                        this.evaluators[i].Settings = this.settings;
                        var result = this.evaluators[i].TryEvaluate(context, reference);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                    catch (Exception ex)
                    {
                        context.LogFailInfoFormat(this.evaluators[i], "Exception happened with evaluating '" + reference + "' with evaluator: '" + evaluators[i].GetType().GetNiceName() + "'.\n" + ex.Message + "\n" + ex.StackTrace);
                        break;
                    }
                }

                var failures = context.GetFailInfos();
                if (failures.Count == 0)
                {
                    return new ConstantValueProvider<TResult>(context, defaultValue);
                }
                else
                {
                    return new FailedValueProvider<TResult>(context, failures);
                }
            }
        }

        ValueProvider<TResult> IValueResolver<TResult>.Resolve(ResolverContext context, string reference)
        {
            if (context is TContext == false)
            {
                throw new InvalidOperationException("Context should be of type " + typeof(TContext).GetNiceName());
            }

            return this.Resolve(context as TContext, reference);
        }

        ValueProvider<TResult> IValueResolver<TResult>.Resolve(ResolverContext context, string reference, TResult defaultValue)
        {
            if (context is TContext == false)
            {
                throw new InvalidOperationException("Context should be of type " + typeof(TContext).GetNiceName());
            }

            return this.Resolve(context as TContext, reference, defaultValue);
        }

        IValueProvider IValueResolver.ResolveWeak(ResolverContext context, string reference)
        {
            if (context is TContext == false)
            {
                throw new InvalidOperationException("Context should be of type " + typeof(TContext).GetNiceName());
            }

            return this.Resolve(context as TContext, reference);
        }

        IValueProvider IValueResolver.ResolveWeak(ResolverContext context, string reference, object defaultValue)
        {
            if (context is TContext == false)
            {
                throw new InvalidOperationException("Context should be of type " + typeof(TContext).GetNiceName());
            }
            // TODO: Test if defaultValue is of type TResult?

            return this.Resolve(context as TContext, reference, (TResult)defaultValue);
        }

        public TResolver Add(ProviderEvaluator<TContext, TResult> resolver)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException("resolver");
            }

            this.evaluators.Add(resolver);
            return (TResolver)this;
        }

        IValueResolver IValueResolver.Add(IProviderEvaluator resolver)
        {
            return this.Add((ProviderEvaluator<TContext, TResult>)resolver);
        }

        public TResolver CatchFailures()
        {
            return this.Add(new CatchFailuresEvaluator<TContext, TResult>());
        }

        public TResolver ConstantValue(Func<TContext, TResult> getValue)
        {
            return this.Add(new ConstantEvaluator<TContext, TResult>(getValue));
        }

        public TResolver ConstantRaw(TResult value)
        {
            return this.Add(new ConstantEvaluator<TContext, TResult>(d => value));
        }

        public TResolver Func(Func<TContext, TResult> getValue)
        {
            return this.Add(new FuncEvaluator<TContext, TResult>(getValue));
        }

        public TResolver TryMemberReference()
        {
            return this.Add(new MemberEvaluator<TContext, TResult>());
        }

        public TResolver TryMethodReference()
        {
            return this.Add(new MethodEvaluator<TContext, TResult>());
        }

        public TResolver TryMethodReference(Type param, Func<TContext, Func<object, TResult>, TResult> callMethod)
        {
            return this.Add(new MethodEvaluator<TContext, object, TResult>(callMethod));
        }

        public TResolver TryMethodReference<TParam>(Func<TContext, Func<TParam, TResult>, TResult> callMethod)
        {
            return this.Add(new MethodEvaluator<TContext, TParam, TResult>(callMethod));
        }

        public TResolver TryMethodReference<TParam1, TParam2>(Func<TContext, Func<TParam1, TParam2, TResult>, TResult> callMethod)
        {
            return this.Add(new MethodEvaluator<TContext, TParam1, TParam2, TResult>(callMethod));
        }

        public TResolver TryExpression()
        {
            return this.Add(new ExpressionEvaluator<TContext, TResult>());
        }
    }

#if SIRENIX_INTERNAL
    public class ValueResolver<TContext, TResult> : BaseValueResolver<ValueResolver<TContext, TResult>, TContext, TResult>
#else
    internal class ValueResolver<TContext, TResult> : BaseValueResolver<ValueResolver<TContext, TResult>, TContext, TResult>
#endif
        where TContext : ResolverContext
    {
        public ValueResolver(ValueResolverSettings settings) : base(settings)
        { }
    }

#if SIRENIX_INTERNAL
    public class DrawerValueResolver<TDrawer, TResult> : BaseValueResolver<DrawerValueResolver<TDrawer, TResult>, DrawerResolverContext<TDrawer>, TResult>
#else
    internal class DrawerValueResolver<TDrawer, TResult> : BaseValueResolver<DrawerValueResolver<TDrawer, TResult>, DrawerResolverContext<TDrawer>, TResult>
#endif
        where TDrawer : OdinDrawer
    {
        public DrawerValueResolver(ValueResolverSettings settings) : base(settings)
        {
        }
    }

#if SIRENIX_INTERNAL
    public class ValidatorValueResolver<TValidator, TResult> : BaseValueResolver<ValidatorValueResolver<TValidator, TResult>, ValidatorResolverContext<TValidator>, TResult>
#else
    internal class ValidatorValueResolver<TValidator, TResult> : BaseValueResolver<ValidatorValueResolver<TValidator, TResult>, ValidatorResolverContext<TValidator>, TResult>
#endif
        where TValidator : Validation.Validator
    {
        public ValidatorValueResolver(ValueResolverSettings settings) : base(settings)
        {
        }
    }

    public struct ValueResolverSettings
    {
        // TODO: Find a better name for this.
        public bool RequireSymbolForMemberReferences;

        public static ValueResolverSettings Default<TResult>()
        {
            return new ValueResolverSettings()
            {
                RequireSymbolForMemberReferences = typeof(TResult) == typeof(string),
            };
        }
    }
}
#endif
#pragma warning enable