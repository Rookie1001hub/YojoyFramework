#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/5 17:36:23
//Email:         854327817@qq.com

#endregion

using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Yojoy.Tech.U3d.Core.Run
{
    [Serializable]
    public class PrefsSpanner
    {
        [HideLabel]
        [SerializeField]
        private PrefsDatabase prefsDatabase;
        /// <summary>
        /// prefs二进制数据位置
        /// </summary>
        private string DatabasePath => UnityGlobalUtility.IsEditorMode ?
            Application.dataPath + "/.prefs.binary" : Application.persistentDataPath + "/.prefs.binary";

    }
}
