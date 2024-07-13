using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;

namespace Yojoy.Tech.U3d.Core.Editor
{
    public static class YojoyEditorSettings
    {
        //自定义目录 方便移植
        public static string YojoyToolsFolder = "YojoyFramework/Editor";

        public static string YojoyParentDirectoyId = "YojoyFramework";

        static YojoyFolderSettins yojoyFolderSettins;

        static YojoyEditorSettings()
        {
            yojoyFolderSettins = Resources.Load<YojoyFolderSettins>("YojoyFolderSettins");
            YojoyToolsFolder = yojoyFolderSettins.YojoyToolsFolder;
            YojoyParentDirectoyId = yojoyFolderSettins.YojoyParentDirectoyId;
        }



        public static readonly DelayInitializationProperty<string> PackageDirectory =
            CreateDelayInitializationProperty(() => $"{YojoyDirectory()}FunctionPackages/");
        public static readonly DelayInitializationProperty<string> CorePackagesDirectory =
            CreateDelayInitializationProperty(() => $"{PackageDirectory.Value}CorePackages/");

        public static readonly DelayInitializationProperty<string> ExtendPackagesDirectory =
            CreateDelayInitializationProperty(() => $"{PackageDirectory.Value}ExtendPackages/");
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
            CreateDelayInitializationProperty(() => $"Assets/{YojoyParentDirectoyId}/");
        public static string GetModuleAssetsDirectory(string moduleId,
            string moduleType)
        {
            var directory = CorePackagesDirectory.Value
                            + $"{moduleId}/CsharpSrc/{moduleType}/Assets/";
            return directory;
        }
        static string YojoyDirectory()
        {
            return Application.dataPath + $"/{YojoyToolsFolder}" +
            $"/{YojoyParentDirectoyId }/";
        }
    }

}
