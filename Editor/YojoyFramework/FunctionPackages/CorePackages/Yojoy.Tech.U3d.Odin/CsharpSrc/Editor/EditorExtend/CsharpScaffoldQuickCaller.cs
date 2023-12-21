#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/6/4 23:13:37
//Email:         854327817@qq.com

#endregion

using UnityEditor;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;
using Yojoy.Tech.U3d.Core.Editor;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;
namespace Yojoy.Tech.U3d.Odin.Editor
{
    public class CsharpScaffoldQuickCaller : AbstractProjectExpander
    {
        private readonly DelayInitializationProperty<string> iconDirectory
            = CreateDelayInitializationProperty(() =>
            {
                var path = YojoyEditorSettings.GetModuleAssetsDirectory
                    ("Yojoy.Tech.U3d.Odin", "Editor");
                return path;
            });
        private readonly EditorIcon csharpScaffoldIcon;
        public CsharpScaffoldQuickCaller()
        {
            csharpScaffoldIcon = EditorIcon.CreateIcon
                (iconDirectory.Value,"csharp_scaffold_white_icon"
                ,"csharp_scaffold_black_icon");
        }

        public override void Execute(Rect rect)
        {
            var drawRect = GetIconRect(rect.width + rect.x - IconSize, 
                rect.y);
            var tip = MultiLanguageString.Create("Open" +
                " csharp scaffold", "打开Cshap脚手架");
            var guiContent = new GUIContent(csharpScaffoldIcon.Icon,
                tip.Text);
            if (!GUI.Button(drawRect, guiContent, GUIStyle.none))
            {
                return;
            }
            EditorPrefs.SetString(CsharpScaffold.OutputDirectoryPrefsKey
                , Path);
            FunctionCenterWindow.OpneTargetMenu(typeof(CsharpScaffold));
        }
    }
}
