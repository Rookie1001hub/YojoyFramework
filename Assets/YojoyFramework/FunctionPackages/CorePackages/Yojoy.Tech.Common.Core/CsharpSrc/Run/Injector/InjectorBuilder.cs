#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/9 16:47:52
//Email:         854327817@qq.com

#endregion

using TypeMap = System.Collections.Generic.Dictionary<System.Type, System.Type>;
using BuildFunc = System.Collections.Generic.Dictionary<System.Type, System.Func<object>>;
using System;
using System.Collections.Generic;

namespace Yojoy.Tech.Common.Core.Run
{
    public class InjectorBuilder
    {
        private TypeMap typeMap;
        private Action<object> debugAction;
        private BuildFunc buildFuncs;

        public void Binding(Dictionary<Type,Type> typeMap
            , Action<object> debugAction)
        {
            this.typeMap = typeMap;
            this.debugAction = debugAction;
        }
        public void Binding( Action<object> debugAction,
            Dictionary<Type,Func<object>>buildFuncs
            )
        {
            this.buildFuncs = buildFuncs;
            this.debugAction = debugAction;
        }

        public void Mapping<TTarget, TInsance>()
            where TInsance:TTarget
        {
            var targetType = typeof(TTarget);
            var instanceType = typeof(TInsance);
            if (typeMap.ContainsKey(targetType))
            {
                debugAction?.Invoke($"Target type {targetType.Name} has been " +
                    $"remapped to actual type {instanceType.Name}");
            }
            typeMap.Add(targetType, instanceType);
        }
        public void RegisterBuildFunc<TTarget>(Func<object> buildFunc)
        {
            var targetType = typeof(TTarget);
            if (buildFuncs.ContainsKey(targetType))
            {
                debugAction?.Invoke($"Target type {targetType.Name} has been " +
                    $"remapped to actual type {buildFunc.Method.Name}");
            }
            buildFuncs.Add(targetType,buildFunc);
        }
    }
}
