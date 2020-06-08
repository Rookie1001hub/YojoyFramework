#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/8 22:45:41
//Email:         854327817@qq.com

#endregion

using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Yojoy.Tech.U3d.Odin.Editor
{
    [Serializable]
    
    public class SwissArmyKnife
    {
        [SerializeField]
        [HideLabel]
        [BoxGroup("Record spanner", "¼ÇÂ¼¹¤¾ß")]
        private RecordSpanner recordSpanner;

        public RecordSpanner RecordSpanner => recordSpanner;
    }
}
