#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ResolverContext.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    using System;
    using System.Collections.Generic;

#if SIRENIX_INTERNAL
    public abstract class ResolverContext
#else
    internal abstract class ResolverContext
#endif
    {
        private readonly List<EvaluatorFailInfo> failures = new List<EvaluatorFailInfo>();
        private readonly HashSet<ExpressionParam> expressionParameters = new HashSet<ExpressionParam>();

        //public bool RequireSymbolForMemberReferences;

        public bool HasAnyFails { get { return this.failures.Count > 0; } }

        public void LogFailInfo(EvaluatorFailInfo info)
        {
            this.failures.Add(info);
        }

        public void LogFailInfo(IProviderEvaluator resolver, string message)
        {
            this.failures.Add(new EvaluatorFailInfo(resolver, message));
        }

        public void LogFailInfoFormat(IProviderEvaluator resolver, string format, params object[] args)
        {
            this.failures.Add(new EvaluatorFailInfo(resolver, string.Format(format, args)));
        }

        public void Reset()
        {
            this.failures.Clear();
        }

        public abstract object GetParentInstance();

        public abstract Type GetParentType();

        public void AddExpressionParameter(string name, Type type, Func<object> getValue)
        {
            this.expressionParameters.Add(new ExpressionParam(name, type, getValue));
        }

        public void AddExpressionParameter<T>(string name, Func<object> getValue)
        {
            this.expressionParameters.Add(new ExpressionParam(name, typeof(T), getValue));
        }

        public ExpressionParam[] GetExpressionParameters()
        {
            var p = new ExpressionParam[this.expressionParameters.Count];
            this.expressionParameters.CopyTo(p);

            return p;
        }

        public List<EvaluatorFailInfo> GetFailInfos()
        {
            return new List<EvaluatorFailInfo>(this.failures);
        }
    }

#if SIRENIX_INTERNAL
    public struct EvaluatorFailInfo
#else
    internal struct EvaluatorFailInfo
#endif
    {
        public IProviderEvaluator Source;
        public string Message;

        public EvaluatorFailInfo(IProviderEvaluator source, string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.Source = source;
            this.Message = message;
        }
    }

#if SIRENIX_INTERNAL
    public class ExpressionParam
#else
    internal class ExpressionParam
#endif
    {
        public readonly string Name;
        public readonly Type Type;
        public readonly Func<object> GetValue;

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public ExpressionParam(string name, Type type, Func<object> getValue)
        {
            this.Name = name;
            this.Type = type;
            this.GetValue = getValue;
        }
    }
}
#endif
#pragma warning enable