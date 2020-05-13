using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Yojoy.Tech.Common.Core.Run
{
   
    public static class CommonExtend
    {
        #region Common
        public static TValue As<TValue>(this object instance)
            where TValue : class
        {
            var result = instance as TValue;
            return result;
        }
        #endregion

        #region String
        public static string EnsureDirectoryFormat(this string directory)
        {
            if (!directory.EndsWith("/"))
            {
                directory += "/";
            }
            return directory;
        }
        #endregion

        #region Enum
        public static TEnum AsEnum<TEnum>(this string str)where TEnum :Enum
        {
            var result = (TEnum)Enum.Parse(typeof(TEnum), str);
            return result;
        }
        #endregion
        #region Relection
        public static List<ATTR> GetAttributes<ATTR>(this Type type) where ATTR:Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(ATTR), true).
                Select(t=>(ATTR)t).ToList();
            return attributes;
        }
        public static ATTR GetSingleAttribute<ATTR>(this Type type) where ATTR : Attribute
        {
            var attributes = type.GetAttributes<ATTR>();
            if (attributes.Count > 1)
            {
                throw new Exception(message: "The number of target" + typeof(ATTR).Name + "is greater than 1!");
            }
            var attribute = attributes.FirstOrDefault();
            return attribute;
        }
        #endregion
    }
}

