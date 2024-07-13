#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/6/4 22:16:53
//Email:         854327817@qq.com

#endregion

#if UNITY_EDITOR
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;

namespace Yojoy.Tech.U3d.Core.Editor
{
    public class YojoySolutionExpander : AbstractProjectExpander
    {
        private readonly EditorIcon yojoyIcon;

        private readonly string yojoyRootConstString;

        private readonly DelayInitializationProperty<string> iconDirectory
            = CreateDelayInitializationProperty(() =>
            {
                var resuslt = YojoyEditorSettings.GetModuleAssetsDirectory(
                    "Yojoy.Tech.U3d.Core", "Editor");
                return resuslt;
            });


        public YojoySolutionExpander()
        {
            yojoyIcon = EditorIcon.CreateIcon(
                iconDirectory.Value,
                "yojoy_common_icon");
            yojoyRootConstString =
                "Assets/" + YojoyEditorSettings.YojoyToolsFolder+"/" + YojoyEditorSettings.YojoyParentDirectoyId;
        }

        public override bool CheckContext()
        {
            var result = Path.StartsWith(yojoyRootConstString);
            return result;
        }

        public override void Execute(Rect rect)
        {
            var drawRect = new Rect(0, rect.y,
                IconSize, IconSize);
            var tipContent = MultiLanguageString.Create(
                "Yojoy solution dynamic menu",
                "Yojoy解决方案动态菜单");
            var guiContent = new GUIContent(yojoyIcon.Icon,
                tipContent.Text);
            if (!GUI.Button(drawRect, guiContent, GUIStyle.none))
            {
                return;
            }

            MarkMenu();
        }
    }
}

#endif
