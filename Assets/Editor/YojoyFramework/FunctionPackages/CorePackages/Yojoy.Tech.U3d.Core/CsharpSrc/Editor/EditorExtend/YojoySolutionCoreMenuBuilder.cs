#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/6/4 22:44:38
//Email:         854327817@qq.com

#endregion

#if UNITY_EDITOR
using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.U3d.Core.Editor
{
    public class YojoySolutionCoreMenuBuilder : IProjectExpanderMenuBuilder
    {
        private string directory;

        public Type ConcernedExpanderType => typeof(YojoySolutionExpander);

        public void AddMenuItem(GenericMenu genericMenu, string path)
        {
            directory = path;
            var opneSandBoxTip = MultiLanguageString.Create(
                "IO/Open sanbox directory",
                "IO/打开沙盒目录");
            genericMenu.AddItem(new GUIContent(opneSandBoxTip.Text),
                false, OpenSandBox, null);
            var cleanMetaTip = MultiLanguageString.Create
                ("IO/Clean meta file", "IO/清理meta文件");
            genericMenu.AddItem(new GUIContent(cleanMetaTip.Text),
                false, CleanMeta, path);
        }
        private void OpenSandBox(object data) =>
            Process.Start(Application.persistentDataPath);
        private void CleanMeta(object usderData)
        {
            var metaPaths = DirectoryUtility.GetPathsContainSonDirectory(
                directory, p => p.EndsWith(".meta"));
            FileUtility.TryDeleteAll(metaPaths);
            UnityEditorUtility.DisplayTip("meta文件清理完毕！");
        }
    }
}

#endif
