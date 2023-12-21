#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/5/28 22:28:03
//Email:         854327817@qq.com

#endregion
#if UNITY_EDITOR
using UnityEditor;

namespace Yojoy.Tech.U3d.Core.Editor
{
    public interface IEditorStateChangeHandler
    {
        PlayModeStateChange ConcernedStateChange { get; }
        void Handle();
    }
}

#endif

