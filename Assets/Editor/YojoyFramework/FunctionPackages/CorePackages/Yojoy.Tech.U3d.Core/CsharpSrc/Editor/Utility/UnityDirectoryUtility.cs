using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.U3d.Core.Editor
{
    public static  class UnityDirectoryUtility 
    {
        public static string GetFullPath(string path)
        {
            if(path.StartsWith(Application.dataPath))
            {
                return path;
            }
            var fullPath = Application.dataPath.Replace("Assets", "")
                + path;
            fullPath = fullPath.EnsureDirectoryFormat();
            return fullPath;
        }
    }
}

