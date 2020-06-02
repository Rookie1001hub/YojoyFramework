#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/2 22:24:48
//Email:         854327817@qq.com

#endregion

using UnityEngine;
using Yojoy.Tech.Common.Core.Run;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;
namespace Yojoy.Tech.U3d.Core.Editor
{
    public class YojoySolutionExpander:AbstractProjectExpander
    {
        private readonly EditorIcon icon;

        private readonly string rootConstString;

        private readonly DelayInitializationProperty<string> iconDirectory
            = CreateDelayInitializationProperty(()=> 
            {
                var result = YojoyEditorSettings.GetModuleAssetsDirectory
                ("Yojoy.Tech.U3d.Core", "Editor");
                return result;
            });

        public YojoySolutionExpander()
        {
            icon = EditorIcon.CreateIcon(
                iconDirectory.Value,
                "yojoy_common_icon");
            rootConstString = "Assets/" +
                YojoyEditorSettings.YojoyDirectory;
        }
        public override bool CheckContext()
        {
            var result = Path.StartsWith(rootConstString);
            return result;
        }
        public override void Execute(Rect rect)
        {
            var drawRect = new Rect(0, rect.y,
                IconSize, IconSize);
            var tipContent = MultiLanguageString.Create
                ("Yojoy solution dynamic menu",
                "Yojoy解决方案动态菜单");
            var guiContent = new GUIContent(icon.Icon,
                tipContent.Text);
            if (!GUI.Button(drawRect, guiContent, GUIStyle.none))
            {
                return;
            }
            MarkMenu();
        }
    }
}
