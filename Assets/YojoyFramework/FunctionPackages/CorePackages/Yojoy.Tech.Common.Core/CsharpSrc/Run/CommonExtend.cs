using System;
using System.Collections;
using System.Collections.Generic;
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
    }
}

