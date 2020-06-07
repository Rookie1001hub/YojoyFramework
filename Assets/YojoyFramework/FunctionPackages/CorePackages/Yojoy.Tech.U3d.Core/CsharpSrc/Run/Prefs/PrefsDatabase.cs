#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/5 17:40:59
//Email:         854327817@qq.com

#endregion

using Sirenix.OdinInspector;
using System;

namespace Yojoy.Tech.U3d.Core.Run
{
    [Serializable]
    public class PrefsDatabase
    {
        [FoldoutGroup("String prefs brower",
            "字符串数据")]
        [HideLabel]
        public readonly StringPrefsValueStorage stringPrefsValueStorage
            = new StringPrefsValueStorage();
        [FoldoutGroup("Int prefs brower",
           "Int数据")]
        [HideLabel]
        public readonly IntPrefsValueStorage intPrefsValueStorage =
            new IntPrefsValueStorage();
        [FoldoutGroup("Float prefs brower",
           "Float数据")]
        [HideLabel]
        public readonly FloatPrefsValueStorage floatPrefsValueStorage
            = new FloatPrefsValueStorage();
        [FoldoutGroup("Bool prefs brower",
           "布尔数据")]
        [HideLabel]
        public readonly BoolPrefsValueStorage boolPrefsValueStorage
            = new BoolPrefsValueStorage();
    }
}
