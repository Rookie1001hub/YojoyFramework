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
            
    }
}

