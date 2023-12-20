#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Yojoy.Tech.U3d.Core.Editor
{
    /// <summary>
    /// Unity编辑器下常用工具类
    /// </summary>
    public static class UnityEditorUtility
    {
        public static void DisplayTip(string tipContent)
        {
            EditorUtility.DisplayDialog(title: "tip", message:
                tipContent, ok: "ok");
        }

    }
}
#endif


