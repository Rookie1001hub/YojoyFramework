using Sirenix.OdinInspector.Editor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;
using Yojoy.Tech.U3d.Core.Editor;

namespace Yojoy.Tech.U3d.Odin.Editor
{
    public abstract class AbstractMenuWindow : OdinMenuEditorWindow
    {
        #region Switch Menu
        private Type currentMenuType;
        private int currentMenuIndex;
        private readonly List<Type> menuTypes = new List<Type>();

        protected OdinMenuTree OdinMenuTree { get; private set; }

        protected void SwitchToTargetMenu(Type menuType)
        {
            var index = menuTypes.FindIndex(match: m => m == menuType);
            SwitchToTargetMenu(index);
        }

        protected virtual void SwitchToTargetMenu(int index)
        {
            if (index < 0 || index >= menuTypes.Count)
                return;
            var item = OdinMenuTree.MenuItems[index];
            item.Value.As<IOnActive>()?.OnActive();
            item.Select();
        }
        #endregion

        #region Build Menu
       

        private readonly DelayInitializationProperty<List<IWindowMenuBuilder>>
            menuBuilders = CreateDelayInitializationProperty(() => {
               return ReflectionUtility.GetAllInstance<IWindowMenuBuilder>(UnityEditorEntrance
                    .EditorAssemblyArrary.Value);
            });
        protected virtual void BuildTopToolBar()
        {

        }
        protected virtual void BuildFixedMenus(OdinMenuTree odinMenuTree)
        {

        }
        protected virtual void BuildDynamicMenus(OdinMenuTree odinMenuTree)
        {
            var windowType = GetType();
            var builders = menuBuilders.Value.FindAll(m => m.MapWindowType == windowType);
            builders.ForEach(m => m.BuildMenu(odinMenuTree));
        }
        /// <summary>
        /// 打开之前的窗口激活项
        /// </summary>
        protected void OpenLastMenu() => SwitchToTargetMenu(index: currentMenuIndex);
        /// <summary>
        /// 添加对象并缓存
        /// </summary>
        /// <param name="odinMenuTree">将要添加功能项的菜单树实例</param>
        /// <param name="menuTitle">功能项的标题</param>
        /// <param name="menuObject">功能项实例</param>
        protected virtual void AddItemAndCacheIndex(OdinMenuTree odinMenuTree,
            string menuTitle,object menuObject)
        {
            odinMenuTree.Add(menuTitle, menuObject);
            odinMenuTree.MenuItems.Last().OnLeftClick += OnItemSelect;

            var menuType = menuObject.GetType();
            if (!menuTypes.Contains(menuType))
            {
                menuTypes.Add(menuType);
            }

            void OnItemSelect(OdinMenuItem targetItem)
            {
                currentMenuIndex = odinMenuTree.MenuItems.FindIndex
                    (match: item => item == targetItem);
                targetItem.As<IOnActive>()?.OnActive();
            }
        }
        
        #endregion
        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree = new OdinMenuTree();
            BuildTopToolBar();
            BuildFixedMenus(OdinMenuTree);
            BuildDynamicMenus(OdinMenuTree);
            OpenLastMenu();
            return OdinMenuTree;
        }
    }
}

