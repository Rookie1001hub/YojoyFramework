using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Sirenix.Utilities.Editor;
using Yojoy.Tech.U3d.Core.Editor;
using Yojoy.Tech.Common.Core.Run;
using Sirenix.OdinInspector.Editor;

namespace Yojoy.Tech.U3d.Odin.Editor
{
    [MenuWindowSizeAttirbute(500, 500)]
    [MenuWindowTitle("Function Center", "功能中心")]
    public class FuntionCenterWindow : AbstractMenuWindowGeneric<FuntionCenterWindow>
    {
        #region Open Window
        [MenuItem("Framework/Funtion Center %k")]
        public static void Open() => OpenSingleWindow();
        #endregion
        #region BuildTree
        protected override void BuildFixedMenus(OdinMenuTree odinMenuTree)
        {
            base.BuildFixedMenus(odinMenuTree);
            BuildMenuObject<CsharpScaffold>("Csharp Scaffold", "Csharp脚手架");

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
                var switchLanguageContent = $"Langeage:{languageType}";
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
