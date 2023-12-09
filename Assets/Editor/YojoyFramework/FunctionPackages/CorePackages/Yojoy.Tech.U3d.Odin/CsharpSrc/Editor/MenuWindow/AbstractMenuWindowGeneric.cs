using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TypeToWindowMap = System.Collections.Generic.Dictionary<System.Type, object>;
using System;
using Yojoy.Tech.Common.Core.Run;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;


namespace Yojoy.Tech.U3d.Odin.Editor
{
    /// <summary>
    /// 多菜单窗口泛型基类
    /// </summary>
    /// <typeparam name="TWindow"></typeparam>
    public abstract class AbstractMenuWindowGeneric<TWindow> : AbstractMenuWindow where TWindow : AbstractMenuWindow
    {
        #region SingleWindow
        private static Type WindowType => typeof(TWindow);

        private static readonly DelayInitializationProperty<TypeToWindowMap>
            singleWindows = CreateDelayInitializationProperty(() => new TypeToWindowMap());
        protected static readonly DelayInitializationProperty<TWindow>
            SingleWindow = CreateDelayInitializationProperty(() =>
            {
                var window = GetWindow<TWindow>();
                singleWindows.Value.Add(WindowType, window);
                return window;
            });

        #endregion
        #region Pipeline

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SingleWindow.SetNull();
            singleWindows.Value.Remove(GetType());
        }

        #endregion
        #region Open

        protected static void OpenSingleWindow()
        {
            InitTitle();
            InitSize();
            FocusAndShow();

            void InitTitle()
            {
                var titleAttribute =
                    WindowType.GetSingleAttribute<MenuWindowTitleAttribute>();
                var titleString = titleAttribute == null ?
                    MultiLanguageString.Create(WindowType.Name, WindowType.Name) :
                   titleAttribute.TitleString;
                SingleWindow.Value.titleContent = new GUIContent(titleString.Text);
            }

            void InitSize()
            {
                var sizeAttribute = WindowType.GetSingleAttribute<MenuWindowSizeAttirbute>();
                // ?. NULL检查运算符 ?? 空合并运算符
                var min = sizeAttribute?.Min ?? 300;
                var max = sizeAttribute?.Max ?? 600;
                Vector2 initSize = new Vector2(min, max);
                SingleWindow.Value.minSize = initSize;
            }
            void FocusAndShow()
            {
                SingleWindow.Value.Focus();
                SingleWindow.Value.Show();
            }
        }

        #endregion
    }
}

