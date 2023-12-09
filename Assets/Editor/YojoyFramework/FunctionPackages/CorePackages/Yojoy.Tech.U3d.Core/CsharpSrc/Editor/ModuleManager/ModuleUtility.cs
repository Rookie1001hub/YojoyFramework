using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;
namespace Yojoy.Tech.U3d.Core.Editor
{
    public static  class ModuleUtility 
    {
        private static readonly DelayInitializationProperty<HashSet<string>>
            coreModuleIdsDelay = CreateDelayInitializationProperty(() =>
             {
                 var temModuleIds = new HashSet<string>();
                 var asmdefPaths = DirectoryUtility
                 .GetAllDirectoryContainSelf(YojoyEditorSettings.
                 CorePackagesDirectory.Value, path =>
                 path.EndsWith(".asmdef"));

                 foreach (var asmdefPath in asmdefPaths)
                 {
                     var moduleId = FileUtility
                     .GetFileIdWithoutExtension(asmdefPath);
                     temModuleIds.Add(moduleId);
                 }

                 return temModuleIds;
             });

        private static readonly DelayInitializationProperty<List<string>>
            moduleAssemblyPathsDelay = CreateDelayInitializationProperty(()=> 
            {
                var temPaths = new List<string>();
                var corePackagePths = DirectoryUtility.
                GetAllDirectoryContainSelf(YojoyEditorSettings.
                CorePackagesDirectory.Value, SelectAssemblyDef);
                var extendPackagePaths = DirectoryUtility
                .GetAllDirectoryContainSelf(YojoyEditorSettings.
                ExtendPackagesDirectory.Value, SelectAssemblyDef);
                temPaths.AddRange(corePackagePths);
                temPaths.AddRange(extendPackagePaths);
                return temPaths;

                bool SelectAssemblyDef(string path) =>
                path.EndsWith(".asmdef");
            });

        private static readonly DelayInitializationProperty<
            Dictionary<string, string>> moduleFullDirectoriesDelay =
            CreateDelayInitializationProperty(()=> 
            {
                var temPathMap = new Dictionary<string, string>();
                var asmdefPaths = moduleAssemblyPathsDelay.Value;
                foreach (var asmdefPath in asmdefPaths)
                {
                    var directory = DirectoryUtility.GetFileLocDirectory(asmdefPath);
                    var moduleId = FileUtility.GetFileIdWithoutExtension(asmdefPath);
                    temPathMap.Add(moduleId, directory);
                }
                return temPathMap;
            });
        /// <summary>
        /// 判断给定路径是否属于核心模块
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public static bool IsCoreModule(string moduleId) =>
            coreModuleIdsDelay.Value.Contains(moduleId);

        public static string GetModuleId(string path)
        {
            var fullDirectory = UnityDirectoryUtility.GetFullPath(path);
            var dictionary = moduleFullDirectoriesDelay.Value;
            var moduleId = dictionary.FindKeyVale(
               p=>fullDirectory==p||fullDirectory.StartsWith(p)).Key;
            return moduleId;
        }
    }
}

