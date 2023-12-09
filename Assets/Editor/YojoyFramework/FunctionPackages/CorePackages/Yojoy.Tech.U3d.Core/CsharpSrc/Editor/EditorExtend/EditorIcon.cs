#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/6/4 22:14:23
//Email:         854327817@qq.com

#endregion

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Yojoy.Tech.U3d.Core.Run;

namespace Yojoy.Tech.U3d.Core.Editor
{
    public class EditorIcon
    {
        private readonly Texture2D whiteIcon;
        private readonly Texture2D blackIcon;

        public EditorIcon(Texture2D whiteIcon, Texture2D blackIcon)
        {
            this.whiteIcon = whiteIcon;
            this.blackIcon = blackIcon;
        }

        public Texture2D Icon =>
            EditorGUIUtility.isProSkin ? blackIcon : whiteIcon;

        public static EditorIcon CreateIcon(string iconDirectory,
            string whiteId, string blackId = null)
        {
            if (!Directory.Exists(iconDirectory))
            {
                throw new Exception($"" +
                                    $"The target directory {iconDirectory} is not exist!");
            }

            var finalBlackId = blackId ?? whiteId;
            var whitePath = GetIconPath(iconDirectory, whiteId);
            var blackPath = GetIconPath(iconDirectory, finalBlackId);
            TryShowIconNotExistError(whitePath);
            TryShowIconNotExistError(blackPath);

            var whiteIcon = AssetDatabaseUtility.LoadAssetAtPath<Texture2D>(
                whitePath);
            var blackIcon = AssetDatabaseUtility.LoadAssetAtPath<Texture2D>(
                blackPath);
            var editorIcon = new EditorIcon(whiteIcon, blackIcon);
            return editorIcon;


            void TryShowIconNotExistError(string iconPath)
            {
                if (!File.Exists(iconPath))
                {
                    throw new Exception(
                        $"The target path {iconPath} is not exist!");
                }
            }

            string GetIconPath(string directory, string id)
                => directory + id + ".png";
        }
    }
}
