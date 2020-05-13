#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="MethodEvaluator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    using System;
    using System.Reflection;
    using Sirenix.Utilities;

    internal class MethodEvaluator<TContext, TResult> : ProviderEvaluator<TContext, TResult> where TContext : ResolverContext
    {
        public override ValueProvider<TResult> TryEvaluate(TContext context, string reference)
        {
            if (string.IsNullOrEmpty(reference) == false && (this.Settings.RequireSymbolForMemberReferences == false || reference[0] == '$'))
            {
                string error;
                MethodInfo method;
                var name = reference.TrimStart('$');
                if (MemberFinder.Start(context.GetParentType())
                    .IsMethod()
                    .HasNoParameters()
                    .HasConvertableReturnType<TResult>()
                    .IsNamed(name)
                    .TryGetMember<MethodInfo>(out method, out error))
                {
                    bool isStatic = method.IsStatic();
                    Func<object, TResult> methodCaller;

                    if (isStatic)
                    {
                        // TODO: Emit
                        methodCaller = instance =>
                        {
                            return ConvertUtility.Convert<TResult>(method.Invoke(null, null));
                        };
                    }
                    else
                    {
                        // TODO: Emit
                        methodCaller = instance =>
                        {
                            if (instance == null)
                            {
                                throw new NullReferenceException();
                            }

                            return ConvertUtility.Convert<TResult>(method.Invoke(instance, null));
                        };
                    }

                    return new FuncValueProvider<TResult>(context, isStatic == false, methodCaller);
                }
                else
                {
                    context.LogFailInfoFormat(this, " - {0} {1}", typeof(TResult).GetNiceName(), name);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }

    internal class MethodEvaluator<TContext, TParam, TResult> : ProviderEvaluator<TContext, TResult> where TContext : ResolverContext
    {
        private readonly Func<TContext, Func<TParam, TResult>, TResult> callMethod;

        public MethodEvaluator(Func<TContext, Func<TParam, TResult>, TResult> callMethod)
        {
            this.callMethod = callMethod;
        }

        public override ValueProvider<TResult> TryEvaluate(TContext context, string reference)
        {
            if (string.IsNullOrEmpty(reference) == false && (this.Settings.RequireSymbolForMemberReferences == false || reference[0] == '$'))
            {
                string error;
                MethodInfo method;
                var name = reference.TrimStart('$');
                if (MemberFinder.Start(context.GetParentType())
                    .IsMethod()
                    .HasParameters<TParam>()
                    .HasConvertableReturnType<TResult>()
                    .IsNamed(name)
                    .TryGetMember<MethodInfo>(out method, out error))
                {
                    return new FuncValueProvider<TResult>(
                        context,
                        method.IsStatic() == false,
                        instance =>
                        {
                            return this.callMethod(context, (p) => ConvertUtility.Convert<TResult>(method.Invoke(instance, new object[] { p })));
                        });
                }
                else
                {
                    context.LogFailInfoFormat(this, "- {0} {1}({2})", typeof(TResult).GetNiceName(), name, typeof(TParam).GetNiceName());
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }

    internal class MethodEvaluator<TContext, TParam1, TParam2, TResult> : ProviderEvaluator<TContext, TResult> where TContext : ResolverContext
    {
        private readonly Func<TContext, Func<TParam1, TParam2, TResult>, TResult> callMethod;

        public MethodEvaluator(Func<TContext, Func<TParam1, TParam2, TResult>, TResult> callMethod)
        {
            this.callMethod = callMethod;
        }

        public override ValueProvider<TResult> TryEvaluate(TContext context, string reference)
        {
            if (string.IsNullOrEmpty(reference) == false && (this.Settings.RequireSymbolForMemberReferences == false || reference[0] == '$'))
            {
                string error;
                MethodInfo method;
                var name = reference.TrimStart('$');
                if (MemberFinder.Start(context.GetParentType())
                    .IsMethod()
                    .HasParameters<TParam1, TParam2>()
                    .HasConvertableReturnType<TResult>()
                    .IsNamed(name)
                    .TryGetMember<MethodInfo>(out method, out error))
                {
                    return new FuncValueProvider<TResult>(
                        context,
                        method.IsStatic() == false,
                        instance =>
                        {
                            return this.callMethod(context, (p1, p2) => ConvertUtility.Convert<TResult>(method.Invoke(instance, new object[] { p1, p2 })));
                        });
                }
                else
                {
                    context.LogFailInfoFormat(this, " - {0} {1}({2}, {3})", typeof(TResult).GetNiceName(), name, typeof(TParam1).GetNiceName(), typeof(TParam2).GetNiceName());
                    return null;
                }
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