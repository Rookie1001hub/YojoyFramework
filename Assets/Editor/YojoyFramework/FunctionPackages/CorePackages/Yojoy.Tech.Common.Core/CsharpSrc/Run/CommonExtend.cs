using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <summary>
        /// 确保目录格式
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string EnsureDirectoryFormat(this string directory)
        {
            if (!directory.EndsWith("/"))
            {
                directory += "/";
            }
            return directory;
        }
        /// <summary>
        /// 将字段内容全部转成大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EveryToBig(this string str)
        {
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                //char.IsLetter是否为Unicode字母？
                if (!char.IsLetter(c))
                {
                    sb.Append(c);
                }
                else
                {
                    var bigC = char.ToUpper(c);
                    sb.Append(bigC);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 判定字符串是否可用
        /// 内容不能为空也不能为空格键
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValid(this string str)
        {
            var results = !string.IsNullOrEmpty(str) &&
                !string.IsNullOrWhiteSpace(str);
            return results;
        }
        #endregion

        #region Enum
        public static TEnum AsEnum<TEnum>(this string str) where TEnum : Enum
        {
            var result = (TEnum)Enum.Parse(typeof(TEnum), str);
            return result;
        }
        /// <summary>
        /// 获取指定枚举类型所有字段
        /// </summary>
        /// <typeparam name="TEnum">指定枚举类型</typeparam>
        /// <returns></returns>
        public static List<TEnum> GetAllEnumValues<TEnum>() where TEnum : Enum
        {
            var enumList = new List<TEnum>();
            var enums = Enum.GetValues(typeof(TEnum));
            foreach (var item in enums)
            {
                var strValue = item.ToString();
                var enumValue = strValue.AsEnum<TEnum>();
                enumList.Add(enumValue);
            }
            return enumList;
        }
        #endregion
        #region Relection
        public static List<ATTR> GetAttributes<ATTR>(this Type type) where ATTR : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(ATTR), true).
                Select(t => (ATTR)t).ToList();
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
        #region Dictionary
        public static KeyValuePair<TKey, TValue> FindKeyVale<TKey, TValue>
        (
            this IDictionary<TKey, TValue> dictionary,
            Func<TValue, bool> valueSelector
        )
        {
            foreach (var kv in dictionary)
            {
                if (valueSelector(kv.Value))
                {
                    return kv;
                }
            }
            return default;
        }
        public static (bool hasValue,TValue value)TryGetValue
            <TKey,TValue>(this IDictionary<TKey,TValue> dictionary
            ,TKey key)
        {
            if(!dictionary.ContainsKey(key))
            {
                return (false, default);
            }
            var value = dictionary[key];
            return (true, value);
        }

        #endregion
    }
}

