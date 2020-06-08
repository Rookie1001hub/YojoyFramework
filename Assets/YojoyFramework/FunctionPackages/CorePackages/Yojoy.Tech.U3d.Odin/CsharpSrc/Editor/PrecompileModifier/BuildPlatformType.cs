#region Comment Head

// Author:        liuruoyu1981
// CreateDate:    2020/1/12 16:07:41
// Email:         xxxx@qq.com

#endregion

using System;

namespace Yojoy.Tech.U3d.Odin.Editor
{
    [System.Serializable]
    [Flags]
    public enum BuildPlatformType
    {
        Standalone = 1 << 1,
        iOS = 1 << 2,
        Android = 1 << 3,
        WebGL = 1 << 4,
        PS4 = 1 << 5,
        XboxOne = 1 << 6,
        Switch = 1 << 7,
        Lumin = 1 << 8,
    }
}
