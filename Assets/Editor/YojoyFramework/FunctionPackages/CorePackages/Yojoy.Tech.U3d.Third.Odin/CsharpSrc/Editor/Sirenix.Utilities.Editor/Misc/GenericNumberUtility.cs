#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="GenericNumberUtility.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class GenericNumberUtility
    {
        private static HashSet<Type> Numbers = new HashSet<Type>(FastTypeComparer.Instance)
        {
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(IntPtr),
            typeof(UIntPtr),
        };

        private static HashSet<Type> Vectors = new HashSet<Type>(FastTypeComparer.Instance)
        {
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
        };

        public static bool IsNumber(Type type)
        {
            return Numbers.Contains(type) || Vectors.Contains(type);
        }

        public static bool NumberIsInRange(object number, double min, double max)
        {
            if (number is sbyte)
            {
                var n = (sbyte)number;
                return n >= min && n <= max;
            }
            else if (number is byte)
            {
                var n = (byte)number;
                return n >= min && n <= max;
            }
            else if (number is short)
            {
                var n = (short)number;
                return n >= min && n <= max;
            }
            else if (number is ushort)
            {
                var n = (ushort)number;
                return n >= min && n <= max;
            }
            else if (number is int)
            {
                var n = (int)number;
                return n >= min && n <= max;
            }
            else if (number is uint)
            {
                var n = (uint)number;
                return n >= min && n <= max;
            }
            else if (number is long)
            {
                var n = (long)number;
                return n >= min && n <= max;
            }
            else if (number is ulong)
            {
                var n = (ulong)number;
                return n >= min && n <= max;
            }
            else if (number is float)
            {
                var n = (float)number;
                return n >= min && n <= max;
            }
            else if (number is double)
            {
                var n = (double)number;
                return n >= min && n <= max;
            }
            else if (number is decimal)
            {
                var n = (decimal)number;
                return n >= (decimal)min && n <= (decimal)max;
            }
            else if (number is Vector2)
            {
                var n = (Vector2)number;
                return n.x >= min && n.x <= max
                    && n.y >= min && n.y <= max;
            }
            else if (number is Vector3)
            {
                var n = (Vector3)number;
                return n.x >= min && n.x <= max
                    && n.y >= min && n.y <= max
                    && n.z >= min && n.z <= max;
            }
            else if (number is Vector4)
            {
                var n = (Vector4)number;
                return n.x >= min && n.x <= max
                    && n.y >= min && n.y <= max
                    && n.z >= min && n.z <= max
                    && n.w >= min && n.w <= max;
            }
            else if (number is IntPtr)
            {
                var n = (long)(IntPtr)number;
                return n >= min && n <= max;
            }
            else if (number is UIntPtr)
            {
                var n = (ulong)(UIntPtr)number;
                return n >= min && n <= max;
            }

            return false;
        }
        
        public static T ConvertNumber<T>(object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
#endif
#pragma warning enable