#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/5 16:24:14
//Email:         854327817@qq.com

#endregion

using UnityEditor;
using Yojoy.Tech.U3d.Core.Run;

namespace Yojoy.Tech.U3d.Core.Editor
{
    /// <summary>
    /// ±à¼­Æ÷Api´úÀí×¢Èë
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
