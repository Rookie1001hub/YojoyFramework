using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Sirenix.Utilities.Editor;
using Yojoy.Tech.U3d.Core.Editor;
using Yojoy.Tech.Common.Core.Run;
using Sirenix.OdinInspector.Editor;
using System;
using Yojoy.Tech.U3d.Core.Run;

namespace Yojoy.Tech.U3d.Odin.Editor
{
    [MenuWindowSizeAttirbute(500, 500)]
    [MenuWindowTitle("Function Center", "功能中心")]
    public class FunctionCenterWindow : AbstractMenuWindowGeneric<FunctionCenterWindow>
    {
        #region Open Window
        [MenuItem("Framework/Function Center %k")]
        public static void Open() => OpenSingleWindow();

        public static void OpneTargetMenu(Type menuType)
        {
            OpenSingleWindow();
            SingleWindow.Value.SwitchToTargetMenu(menuType);
        }

        #endregion
        #region BuildTree
        protected override void BuildFixedMenus(OdinMenuTree odinMenuTree)
        {
            base.BuildFixedMenus(odinMenuTree);
            BuildMenuObject<CsharpScaffold>("Csharp Scaffold", "Csharp脚手架");
            var prefsSpannerTip = MultiLanguageString.Create
                ("Prefs Spanner", "Prefs工具");
            AddItemAndCacheIndex(OdinMenuTree, prefsSpannerTip.Text
                , PrefsSpanner.Instance);
            BuildMenuObject<SwissArmyKnife>("Swiss army knife", "瑞士军刀");
            BuildMenuObject<PrecompileModifier>("PreCompile Modifier", "预编译修改");
        }
        private void BuildMenuObject<TMenuObject>(string englishTitle,
            string chinesTitle)
            where TMenuObject : class, new()
        {
            var finalTitle = englishTitle ?? typeof(TMenuObject).Name;
            AddItemAndCacheIndex(OdinMenuTree, MultiLanguageString.Create
                (finalTitle, chinesTitle).Text, ReflectionUtility.CreateInstance<TMenuObject>());
        }
        #endregion
        #region TopToolbar

        protected override void BuildTopToolBar()
        {
            base.BuildTopToolBar();
            OdinMenuTree.DefaultMenuStyle.IconSize = 28.00f;
            OdinMenuTree.Config.DrawSearchToolbar = true;
        }
        protected override void OnBeginDrawEditors()
        {
            base.OnBeginDrawEditors();
            if (OdinMenuTree == null)
                return;
            var select = MenuTree.Selection.FirstOrDefault();
            var toolBarHeight = MenuTree.Config.SearchToolbarHeight;

            SirenixEditorGUI.BeginHorizontalToolbar(toolBarHeight);
            {
                if (select != null)
                    GUILayout.Label(select.Name);
                DrawLanguageSwitch();
            }
            SirenixEditorGUI.EndHorizontalToolbar();

            void DrawLanguageSwitch()
            {
                var languageType = UnityEditorEntrance.GetCurrentLanguageType();
                var switchLanguageContent = $"Language:{languageType}";
                if (SirenixEditorGUI.ToolbarButton(
                    new GUIContent(switchLanguageContent)))
                {
                    MakeSwitchLanguageMenu();
                }
            }
            void MakeSwitchLanguageMenu()
            {
                var genericMenu = new GenericMenu();
                var languageTypes =
                    CommonExtend.GetAllEnumValues<LanguageType>();
                foreach (var item in languageTypes)
                {
                    genericMenu.AddItem(new GUIContent(
                        item.ToString()), false, SwitchLanguage, item.ToString());
                }
                genericMenu.ShowAsContext();
            }
            void SwitchLanguage(object data)
            {
                var languageType = ((string)data).AsEnum<LanguageType>();
                MultiLanguageString.SetLanguageType(languageType);
                UnityEditorEntrance.UpdateLanguageType(languageType);
                ForceMenuTreeRebuild();
                OpenLastMenu();

                var switchTip = MultiLanguageString.Create($"Yojoy已切换" +
                    $"为{languageType}!", $"Yojoy has been switched to " +
                    $"{languageType}!");
                Debug.Log(switchTip.Text);
            }
        }
        #endregion
    }
}
