#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/5/31 23:22:59
//Email:         854327817@qq.com

#endregion

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.U3d.Core.Editor
{
    public abstract class AbstractProjectExpander:IProjectExpander
    {
        protected string Guid { get; private set; }
        protected string Path { get; private set; }

        public abstract void Execute(Rect rect);
        public void SaveContext(string guid,string path)
        {
            Guid = guid;Path = path;
        }
        public virtual bool CheckContext() => true;
        /// <summary>
        /// 编辑器图标啊小
        /// </summary>
        protected virtual float IconSize => EditorGUIUtility.singleLineHeight;
        /// <summary>
        /// 获取图标绘制范围
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected Rect GetIconRect(float x, float y)
            => new Rect(x, y, IconSize, IconSize);
        protected readonly List<IProjectExpanderMenuBuilder> menuBuilders;
        protected AbstractProjectExpander()
        {
            var allBuilders = ReflectionUtility.GetAllInstance
                <IProjectExpanderMenuBuilder>(UnityEditorEntrance
                .EditorAssemblyArrary.Value);
            menuBuilders = allBuilders.FindAll(b => b.ConcernedExpanderType
              == GetType());
        }
        /// <summary>
        /// 完成菜单构建
        /// </summary>
        protected virtual void MarkMenu()
        {
            var menu = new GenericMenu();
            foreach (var menuBuilder in menuBuilders)
            {
                menuBuilder.AddMenuItem(menu, Path);
            }
            menu.ShowAsContext();
        }

    }
}
