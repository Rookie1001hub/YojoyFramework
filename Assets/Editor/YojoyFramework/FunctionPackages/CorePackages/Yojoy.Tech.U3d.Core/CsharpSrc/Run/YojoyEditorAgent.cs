#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/6/5 16:21:34
//Email:         854327817@qq.com

#endregion

using System;

namespace Yojoy.Tech.U3d.Core.Run
{
    /// <summary>
    /// 编辑器API代理
    /// </summary>
    public static class YojoyEditorAgent
    {
        public static Action<string> DispalyTip;

        public static Func<string, string> GetBeautifiedJson;
    }
}
