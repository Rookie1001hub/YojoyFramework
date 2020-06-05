#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/5 9:44:03
//Email:         854327817@qq.com

#endregion

using System;
using UnityEngine;

namespace Yojoy.Tech.U3d.Core.Run
{
    public abstract class AbstractPrefsValueGeneric<TValue> : IPrefsValueGeneric<TValue>
    {
        [SerializeField]
        private string key;
        public string Key => key;

        [SerializeField]
        private TValue value;
        public TValue Value => value;

        [SerializeField]
        [TextArea(2,3)]
        private string description;
        public string Description => description;

        public void Init(string key, TValue value, string description)
        {
            this.key = key;
            this.value = value;
            this.description = description;
        }
    }
}
