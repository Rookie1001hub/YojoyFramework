#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/6/5 16:24:14
//Email:         854327817@qq.com

#endregion

using UnityEditor;
using Yojoy.Tech.U3d.Core.Run;

namespace Yojoy.Tech.U3d.Core.Editor
{
    /// <summary>
    /// 编辑器Api代理注入
    /// </summary>
    public class EditorApiInjector : IEditorStateChangeHandler
    {
        public PlayModeStateChange ConcernedStateChange => PlayModeStateChange.EnteredEditMode;

        public void Handle()
        {
            YojoyEditorAgent.DispalyTip = UnityEditorUtility.DisplayTip;
            YojoyEditorAgent.GetBeautifiedJson = JsonNetUtility.GetBeautifiedJson;
        }
    }
}
