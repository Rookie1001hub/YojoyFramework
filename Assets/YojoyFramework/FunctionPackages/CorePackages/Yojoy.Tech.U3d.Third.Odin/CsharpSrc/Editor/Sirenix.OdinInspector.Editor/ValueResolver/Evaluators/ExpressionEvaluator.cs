#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ExpressionEvaluator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor.Expressions;
    using System;

    internal class ExpressionEvaluator<TContext, TResult> : ProviderEvaluator<TContext, TResult> where TContext : ResolverContext
    {
        public override ValueProvider<TResult> TryEvaluate(TContext context, string reference)
        {
            if (string.IsNullOrEmpty(reference) == false && reference[0] == '@')
            {
                string expression = reference.Substring(1);
                bool isStatic = this.IsStatic(context);

                var parameters = context.GetExpressionParameters();

                if (parameters != null && parameters.Length > 0)
                {
                    string[] pNames = new string[parameters.Length];
                    Type[] pTypes = new Type[parameters.Length];
                    Func<object>[] pGetters = new Func<object>[parameters.Length];
                    
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        pNames[i] = parameters[i].Name;
                        pTypes[i] = parameters[i].Type;
                        pGetters[i] = parameters[i].GetValue;
                    }

                    string error;
                    var method = ExpressionUtility.ParseExpression(expression, isStatic, context.GetParentType(), pTypes, pNames, out error);

                    if (!method.Method.ReturnType.IsCastableTo(typeof(TResult)))
                    {
                        error = "The expression '" + expression + "' evaluates to type '" + method.Method.ReturnType.GetNiceName() + "', which cannot be cast to expected type '" + typeof(TResult).GetNiceName() + "'.";
                    }

                    if (error != null)
                    {
                        context.LogFailInfo(this, error);
                        return null;
                    }
                    else if (isStatic)
                    {
                        object[] valueBuffer = new object[parameters.Length];
                        return new FuncValueProvider<TResult>(context, false, instance => ConvertUtility.Convert<TResult>(method.DynamicInvoke(GetValues(valueBuffer, pGetters))));
                    }
                    else
                    {
                        object[] valueBuffer = new object[parameters.Length + 1];
                        return new FuncValueProvider<TResult>(context, true, instance => ConvertUtility.Convert<TResult>(method.DynamicInvoke(GetValues(valueBuffer, instance, pGetters))));
                    }
                }
                else
                {
                    string error;
                    var method = ExpressionUtility.ParseExpression(expression, isStatic, context.GetParentType(), out error);

                    if (!method.Method.ReturnType.IsCastableTo(typeof(TResult)))
                    {
                        error = "The expression '" + expression + "' evaluates to type '" + method.Method.ReturnType.GetNiceName() + "', which cannot be cast to expected type '" + typeof(TResult).GetNiceName() + "'.";
                    }

                    if (error != null)
                    {
                        context.LogFailInfo(this, error);
                        return null;
                    }
                    else if (isStatic)
                    {
                        return new FuncValueProvider<TResult>(context, false, instance => ConvertUtility.Convert<TResult>(method.DynamicInvoke()));
                    }
                    else
                    {
                        return new FuncValueProvider<TResult>(context, true, instance => ConvertUtility.Convert<TResult>(method.DynamicInvoke(instance)));
                    }
                }
            }
            else
            {
                return null;
            }
        }

        private static object[] GetValues(object[] buffer, Func<object>[] getters)
        {
            for (int i = 0; i < getters.Length; i++)
            {
                buffer[i] = getters[i]();
            }

            return buffer;
        }
        private static object[] GetValues(object[] buffer, object instance, Func<object>[] getters)
        {
            buffer[0] = instance;
            for (int i = 0; i < getters.Length; i++)
            {
                buffer[i + 1] = getters[i]();
            }

            return buffer;
        }
    }
}
#endif
#pragma warning enable