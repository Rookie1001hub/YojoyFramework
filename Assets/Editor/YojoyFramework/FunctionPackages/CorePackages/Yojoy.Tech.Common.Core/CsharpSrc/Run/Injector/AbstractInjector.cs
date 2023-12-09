#region Comment Head



#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InstanceTypeMap = System.Collections.Generic.Dictionary<
System.Type,System.Type>;
using SingleStorage = System.Collections.Generic.Dictionary<
System.Type,object>;
using BuildFuncStorage = System.Collections.Generic.Dictionary<
System.Type, System.Func<object>>;
using FieldInfoStorage = System.Collections.Generic.Dictionary<
System.Type, System.Collections.Generic.List<System.Reflection.FieldInfo>>;
using PropertyInfoStorage = System.Collections.Generic.Dictionary<
System.Type, System.Collections.Generic.List<System.Reflection.PropertyInfo>>;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;

namespace Yojoy.Tech.Common.Core.Run
{
    public abstract class AbstractInjector : IInjector
    {

        private const BindingFlags BindingFlags
            = System.Reflection.BindingFlags.Public
              | System.Reflection.BindingFlags.NonPublic
              | System.Reflection.BindingFlags.Instance
              | System.Reflection.BindingFlags.Static;

        private const string HiddenConstructorId
            = "HiddenConstructor";
        private readonly object[] constructorArray
        = new object[1];

        protected readonly DelayInitializationProperty<InstanceTypeMap>
            instanceTypeMapDelay = CreateDelayInitializationProperty(() => new InstanceTypeMap());
            

        protected readonly DelayInitializationProperty<SingleStorage>
            singleStorageDelay = CreateDelayInitializationProperty(() => new SingleStorage());

        protected readonly DelayInitializationProperty<FieldInfoStorage>
            fieldInfoStorageDelay = CreateDelayInitializationProperty(()=>new FieldInfoStorage());

        protected readonly DelayInitializationProperty<PropertyInfoStorage>
            propertyInfoStorageDelay = CreateDelayInitializationProperty(()=>new PropertyInfoStorage());

        protected readonly DelayInitializationProperty<BuildFuncStorage>
            buildFuncsDelay = CreateDelayInitializationProperty(() =>new BuildFuncStorage());
            
        private void InvokeHiddenConstructor(object instance)
        {
            var methods = instance.GetType().GetMethods(
                BindingFlags).ToList();
            var hiddenConstructor = methods.Find(
                m => m.Name == HiddenConstructorId);
            hiddenConstructor?.Invoke(instance, constructorArray);
        }

        protected bool IsSingle(Type type)
            => type.GetSingleAttribute<SingleAttribute>() != null;

        private bool HasInjectAttribute(MemberInfo memberInfo)
        {
            var result = memberInfo.GetCustomAttributes(
                typeof(InjectAttribute), true).Any();
            return result;
        }
        
        public TTargetType Get<TTargetType>(bool useReflection) where TTargetType : class
        {
            var targetType = typeof(TTargetType);
            var instanceType = GetInstanceType(targetType);
            var instance = Resolve(targetType, instanceType,
                useReflection).As<TTargetType>();
            return instance;
        }
        
        protected abstract List<Type> AllTypes { get; set; }

        private InstanceTypeMap defaultInjectMap;

        public InstanceTypeMap DefaultInjectMap
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
                    var defaultInjectAttribute = type
                        .GetSingleAttribute<DefaultInjectAttribute>();
                    if (defaultInjectAttribute != null)
                    {
                        defaultInjectMap.Add(
                            defaultInjectAttribute.TargetType,type);
                    }
                }

                return defaultInjectMap;
            }
        }

        protected virtual Type GetInstanceType(Type targetType)
        {
            if (instanceTypeMapDelay.Value.ContainsKey(targetType))
            {
                return instanceTypeMapDelay.Value[targetType];
            }

            if (DefaultInjectMap.ContainsKey(targetType))
            {
                var defaultType = DefaultInjectMap[targetType];
                return defaultType;
            }

            var isAbstract = IsAbstractType(targetType);
            if (isAbstract)
            {
                throw new Exception("Abstract types and interface" +
                                    "types cannot create instance!");
            }

            return targetType;


            bool IsAbstractType(Type type)
            {
                var result = type.IsAbstract || type.IsInterface;
                return result;
            }
        }

        private object Resolve(Type targetType, Type instanceType,
            bool useReflection)
        {
            var instance = useReflection
                ? ResolveAtReflection(targetType, instanceType)
                : ResolveAtFunc(targetType, instanceType);
            return instance;
        }

        private object ResolveAtReflection(Type targetType, Type instanceType)
        {
            var instance = TryGetFormSingleStorage(targetType);
            if (instance != null)
            {
                return instance;
            }

            var isSingle = IsSingle(instanceType);
            var fieldInfos = GetRequireInjectFieldInfos(targetType,
                isSingle);
            var propertyInfos = GetRequireInjectPropertyInfos(
                targetType, isSingle);
            instance = Activator.CreateInstance(instanceType);

            if (isSingle)
            {
                singleStorageDelay.Value.Add(targetType,instance);
            }
            
            InjectFields(instance,fieldInfos,true);
            InjectProperties(instance,propertyInfos,true);
            InvokeHiddenConstructor(instance);
            
            return instance;
        }

        private void InjectFields(object instance, List<FieldInfo> fieldInfos,
            bool useReflection)
        {
            foreach (var fieldInfo in fieldInfos)
            {
                var targetType = fieldInfo.FieldType;
                var instanceType = GetInstanceType(targetType);
                var field = Resolve(targetType, instanceType, useReflection);
                fieldInfo.SetValue(instance,field);
            }
        }

        private void InjectProperties(object instance, List<PropertyInfo> propertyInfos,
            bool useReflection)
        {
            foreach (var propertyInfo in propertyInfos)
            {
                var targetType = propertyInfo.PropertyType;
                var instanceType = GetInstanceType(targetType);
                var property = Resolve(targetType, instanceType, useReflection);
                propertyInfo.SetValue(instance,property);
            }
        }

        private List<FieldInfo> GetRequireInjectFieldInfos(Type type,
            bool isSingle)
        {
            if (fieldInfoStorageDelay.Value.ContainsKey(type))
            {
                var fieldInfos = fieldInfoStorageDelay
                    .Value[type];
                return fieldInfos;
            }

            var fieldInfoArray = type.GetFields(BindingFlags);
            var targetFieldInfos = fieldInfoArray
                .Where(HasInjectAttribute).ToList();

            if (!isSingle)
            {
                fieldInfoStorageDelay.Value.Add(type,targetFieldInfos);
            }

            return targetFieldInfos;
        }


        private List<PropertyInfo> GetRequireInjectPropertyInfos(Type type,
            bool isSingle)
        {
            if (propertyInfoStorageDelay.Value.ContainsKey(type))
            {
                var propertyInfos = propertyInfoStorageDelay
                    .Value[type];
                return propertyInfos;
            }

            var propertyInfoArray = type.GetProperties(BindingFlags);
            var targetPropertyInfos = propertyInfoArray
                .Where(HasInjectAttribute).ToList();

            if (!isSingle)
            {
                propertyInfoStorageDelay.Value.Add(type,targetPropertyInfos);
            }

            return targetPropertyInfos;
        }


        private object ResolveAtFunc(Type targetType, Type instanceType)
        {
            var instance = TryGetFormSingleStorage(targetType);
            if (instance != null)
            {
                return instance;
            }

            var buildFunc = GetBuildFunc();
            instance = buildFunc();
            if (IsSingle(instanceType))
            {
                singleStorageDelay.Value.Add(targetType,instance);
            }

            var fieldInfos = GetRequireInjectFieldInfos(
                instanceType, false);
            var propertyInfos = GetRequireInjectPropertyInfos(
                instanceType, false);
            InjectFields(instance,fieldInfos,false);
            InjectProperties(instance,propertyInfos,false);
            InvokeHiddenConstructor(instance);

            return instance;
            
            Func<object> GetBuildFunc()
            {
                if (buildFuncsDelay.Value.ContainsKey(targetType))
                {
                    return buildFuncsDelay.Value[targetType];
                }
                
                throw new Exception($"The target type {targetType.Name}" +
                                    $"cannot find any build delegate!");
            }
        }

        private Action<object> debugAction;


        protected AbstractInjector(Action<object> debugAction,
            InjectorBuilder injectorBuilder)
        {
            this.debugAction = debugAction;
            constructorArray[0] = this;
            injectorBuilder.Binding(instanceTypeMapDelay.Value,
                debugAction);
            injectorBuilder.Binding(debugAction,buildFuncsDelay.Value);
        }

        private object TryGetFormSingleStorage(Type targetType)
        {
            object instance = null;

            if (singleStorageDelay.Value.ContainsKey(targetType))
            {
                instance = singleStorageDelay.Value[targetType];
            }

            return instance;
        }

    }
}
