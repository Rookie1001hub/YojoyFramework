using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace Yojoy.Tech.Common.Core.Run
{
    public class ReflectionUtility
    {
        public static List<Type> GetTypeList<TOject>(bool isInterface, bool isAbstract,
            params Assembly[] assemblies)
        {
            var types = new List<Type>();
            foreach (var item in assemblies)
            {
                var tempTypes = item.GetTypes();
                var targetTypes = tempTypes.Where(t => typeof(TOject).IsAssignableFrom(t))
                    .Where(t => t.IsInterface == isInterface && t.IsAbstract == isAbstract)
                    .ToList();
                types.AddRange(targetTypes);
            }
            return types;
        }

        public static void SetProperty(object obj,string propertyId,
            object value)
        {
            var propertyInfos = obj.GetType().GetProperties().ToList();
            var propertyInfo = propertyInfos.Find(
                f => f.Name == propertyId);
            propertyInfo.SetValue(obj, value);
        }

        public static TObject CreateInstance<TObject>(Type type)
       where TObject : class
        {
            var instance = (TObject)Activator.CreateInstance(type);
            return instance;
        }
        public static TObject CreateInstance<TObject>() where TObject : class
        {
            var instance = (TObject)Activator.CreateInstance(typeof(TObject));
            return instance;
        }

        public static List<TObject> GetAllInstance<TObject>(params Assembly[] assemblies)
            where TObject : class
        {
            var instances = new List<TObject>();
            var targetTypes = GetTypeList<TObject>(isInterface: false, isAbstract: false, assemblies);
            foreach (var item in targetTypes)
            {
                var instance = CreateInstance<TObject>(item);
                instances.Add(instance);
            }
            return instances;
        }
            

        public static object InvokeMethod(object obj,string methodName,
            object[]args)
        {
            var targetType = obj.GetType();
            var methodInfos = targetType.GetMethods(BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.NonPublic).ToList();
            var targetMethodInfo = methodInfos.Find(m => m.Name == methodName);
            if (targetMethodInfo==null)
            {
                throw new Exception(
                    $"Unable to find a method named {methodName}" +
                    $"on the type {targetType.Name}!");
            }
            var result = targetMethodInfo.Invoke(obj, args);
            return result;
        }
    }
}

