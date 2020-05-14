using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Yojoy.Tech.U3d.Odin.Editor
{
    [MenuWindowSizeAttirbute(500,500)]
    [MenuWindowTitle("Function Center","功能中心")]
    public class FuntionCenterWindow : AbstractMenuWindowGeneric<FuntionCenterWindow>
    {
        #region Open Window
        [MenuItem("Framework/Funtion Center %k")]
        public static void Open() => OpenSingleWindow();
        #endregion
    }
}
