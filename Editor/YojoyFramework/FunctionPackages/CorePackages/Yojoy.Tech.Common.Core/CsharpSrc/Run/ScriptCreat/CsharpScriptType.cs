using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yojoy.Tech.Common.Core.Run
{
    [System.Serializable]
    public enum CsharpScriptType : byte
    {
        Class,
        AbstractClass,
        Interface,
        Enum,
        Struct,
        MonoScript,
        ScriptableObject,
    }
}
