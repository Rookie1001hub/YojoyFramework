#region Comment Head


#endregion

using System;
using System.Collections.Generic;
using TypeMap = System.Collections.Generic.Dictionary<System.Type,System.Type>;
using BuildFuncStorage = System.Collections.Generic.Dictionary<System.Type,
System.Func<object>>;

namespace Yojoy.Tech.Common.Core.Run
{
    public class InjectorBuilder
    {
        private TypeMap typeMap;
        private Action<object> debugAction;
        private BuildFuncStorage buildFuns;

        public void Binding(Dictionary<Type, Type> typeMap,
            Action<object> debugAction)
        {
            this.typeMap = typeMap;
            this.debugAction = debugAction;
        }

        public void Binding(Action<object> debugAction,
            Dictionary<Type, Func<object>> buildFuncs)
        {
            this.debugAction = debugAction;
            this.buildFuns = buildFuncs;
        }

        public void Mapping<TTarget, TInstance>()
            where TInstance : TTarget
        {
            var targetType = typeof(TTarget);
            var instanceType = typeof(TInstance);

            if (typeMap.ContainsKey(targetType))
            {
                debugAction?.Invoke(
                    $"Target type {targetType.Name} has been" +
                    $" remapped to actual type {instanceType.Name}!");
            }
            
            typeMap.Add(targetType,instanceType);
        }


        public void RegisterBuildFfunc<TTarget>(
            Func<object> buildFunc)
        {
            var targetType = typeof(TTarget);

            if (buildFuns.ContainsKey(targetType))
            {
                debugAction?.Invoke(
                    $"Target type {targetType.Name}"
                    + $"has been remapped to build delegate {buildFunc.Method.Name}!");
            }
            
            buildFuns.Add(targetType,buildFunc);
        }
    }
}
