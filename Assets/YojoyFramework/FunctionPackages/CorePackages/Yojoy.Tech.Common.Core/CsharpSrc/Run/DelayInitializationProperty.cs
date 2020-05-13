using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Yojoy.Tech.Common.Core.Run
{
    public class DelayInitializationProperty<TValue>where TValue:class
    {
        private readonly Func<TValue> buildFunc;

        public DelayInitializationProperty(Func<TValue> func) => buildFunc = func;

        private TValue value;

        public TValue Value
        {
            get
            {
                if (value != null)
                    return value;
                value = buildFunc();
                return value;
            }
        }

        public void SetNull() => value = null;
    }
}

