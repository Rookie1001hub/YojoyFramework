#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/5/31 23:33:26
//Email:         854327817@qq.com

#endregion

#if UNITY_EDITOR
using System;
using UnityEditor;

namespace Yojoy.Tech.U3d.Core.Editor
{
    /// <summary>
    /// 为目标拓展器添加动态菜单
    /// </summary>
    public interface IProjectExpanderMenuBuilder
    {
        /// <summary>
        /// 自身实例所关注的编辑器扩展器类型
        /// </summary>
        Type ConcernedExpanderType { get; }

        /// <summary>
        /// 添加菜单项
        /// </summary>
        /// <param name="genericMenu"></param>
        /// <param name="path"></param>
        void AddMenuItem(GenericMenu genericMenu, string path);
    }
}
#endif

