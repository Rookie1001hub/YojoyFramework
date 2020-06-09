#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/9 11:49:15
//Email:         854327817@qq.com

#endregion

//抽象对象和实现类型之间的映射
using InstanceTypeMap = System.Collections.Generic.Dictionary<
System.Type, System.Type>;
//类型和单例实例之间的映射
using SingleStorage = System.Collections.Generic.Dictionary<
    System.Type, object>;
//类型和构造委托之间的映射
using BuildFuncStorage = System.Collections.Generic.Dictionary<
    System.Type, System.Func<object>>;
//类型的字段信息
using FieldInfoStorage = System.Collections.Generic.Dictionary
    <System.Type, System.Collections.Generic.List<System.Reflection.FieldInfo>>;
//类型的属性信息
using PropertyInfoStorage = System.Collections.Generic.Dictionary
    <System.Type, System.Collections.Generic.List<System.Reflection.PropertyInfo>>;
using System.Reflection;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Yojoy.Tech.Common.Core.Run
{
    /// <summary>
    /// 注入器在开发阶段使用反射完成注入流程
    /// 发布阶段使用委托完成注入流程
    /// </summary>
    public abstract class AbstractInjector : IInjector
    {
        /// <summary>
        /// 反射作用区域
        /// </summary>
        private const BindingFlags BindingFlags =
           System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static;

        private const string HiddenConstructorId = "HiddenConstructor";

        private readonly object[] constructorArray = new object[1];

        protected readonly DelayInitializationProperty<InstanceTypeMap>
            instanceTypeDelay = CreateDelayInitializationProperty(() => new InstanceTypeMap());
        protected readonly DelayInitializationProperty<SingleStorage>
            singleStorageDelay = CreateDelayInitializationProperty(() => new SingleStorage());
        protected readonly DelayInitializationProperty<BuildFuncStorage>
            buildFuncsDelay = CreateDelayInitializationProperty(() => new BuildFuncStorage());
        protected readonly DelayInitializationProperty<FieldInfoStorage>
            fieldInfoStorageDelay = CreateDelayInitializationProperty(() => new FieldInfoStorage());
        protected readonly DelayInitializationProperty<PropertyInfoStorage>
            propertyInfoStorageDelay = CreateDelayInitializationProperty(() => new PropertyInfoStorage());



        private void InvokeHiddenConstructor(object instance)
        {
            var methods = instance.GetType().GetMethods(BindingFlags).ToList();
            var hiddenConstructor = methods.Find(m => m.Name == HiddenConstructorId);
            hiddenConstructor?.Invoke(instance, constructorArray);
        }
        protected bool IsSingle(Type type) => type.GetSingleAttribute<SingleAttribute>() != null;

        protected bool HasInjectorAttribute(MemberInfo memberInfo)
        {
            var result = memberInfo.GetCustomAttributes(typeof(InjectAttribute), true).Any();
            return result;
        }

        public TTargetType Get<TTargetType>(bool useReflection) where TTargetType : class
        {
            throw new System.NotImplementedException();
        }

        protected abstract List<Type> AllTypes { get; set; }

        private InstanceTypeMap defaultInjectMap;

        public InstanceTypeMap DefautInjectMap
        {
            get
            {
                if (defaultInjectMap != null)
                {
                    return defaultInjectMap;
                }
                defaultInjectMap = new InstanceTypeMap();

                foreach (var type in AllTypes)
                {
                    var defaultInjectAttribute = type.GetSingleAttribute<DefaultInjectAttribute>();
                    if (defaultInjectAttribute != null)
                    {
                        defaultInjectMap.Add(defaultInjectAttribute.TargetType, type);
                    }
                }
                return defaultInjectMap;
            }
        }
        protected virtual Type GetInstanceType(Type targetType)
        {
            if (instanceTypeDelay.Value.ContainsKey(targetType))
            {
                return instanceTypeDelay.Value[targetType];
            }
            if (defaultInjectMap.ContainsKey(targetType))
            {
                return defaultInjectMap[targetType];
            }
            var isAbstract = IsAbstract(targetType);

            if (isAbstract)
            {
                throw new Exception("Abstract types and interface" +
                                       "types cannot create instance!");
            }
            return targetType;

            bool IsAbstract(Type type)
            {
                var result = type.IsAbstract || type.IsInterface;
                return result;
            }
        }
    }
}
