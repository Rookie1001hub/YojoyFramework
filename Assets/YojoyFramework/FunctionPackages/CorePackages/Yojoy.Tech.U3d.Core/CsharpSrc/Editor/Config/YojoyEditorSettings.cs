using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;

namespace Yojoy.Tech.U3d.Core.Editor
{
    public static class YojoyEditorSettings
    {
        private static readonly string YojoyRootDirectoyId = "YojoyFramework";

        public static readonly DelayInitializationProperty<string> YojoyDirectory =
            CreateDelayInitializationProperty(() =>
            Application.dataPath + $"/{YojoyRootDirectoyId}/");

        public static readonly DelayInitializationProperty<string> PackageDirectory =
            CreateDelayInitializationProperty(() => $"{YojoyDirectory.Value}FunctionPackages");
        public static readonly DelayInitializationProperty<string> CorePackagesDirectory =
            CreateDelayInitializationProperty(()=>$"{PackageDirectory.Value}CorePackages");

        public static readonly DelayInitializationProperty<string> ExtendPackagesDirectory =
            CreateDelayInitializationProperty(() => $"{PackageDirectory.Value}ExtendPackages");
        public static readonly DelayInitializationProperty<HashSet<string>> IgnoreEditorAssemblyIds
            = CreateDelayInitializationProperty(() =>
             {
                 var result = new HashSet<string>();
                 result.Add(item: "Yojoy.Tech.U3d.Third.Odin.Editor");
                 result.Add(item: "Yojoy.Tech.U3d.Third.Odin.Run");
                 return result;
             });

        public static readonly DelayInitializationProperty<string> AssetsConst =
            CreateDelayInitializationProperty(() => "Assets/");
        public static readonly DelayInitializationProperty<string> AssetsHeadConst =
            CreateDelayInitializationProperty(() => $"Assets/{YojoyRootDirectoyId}/");
    }

}
