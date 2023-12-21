using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Yojoy.Tech.Common.Core.Run
{
    public static class CommonGlobalUtility
    {
        public static  DelayInitializationProperty<TValue>
            CreateDelayInitializationProperty<TValue>(Func<TValue> func)
            where TValue : class
        {
            var delayProperty = new DelayInitializationProperty<TValue>(func);
            return delayProperty;
        }

        public static string NowDateString() => DateTime.Now.ToString("G");
    }
   
}

