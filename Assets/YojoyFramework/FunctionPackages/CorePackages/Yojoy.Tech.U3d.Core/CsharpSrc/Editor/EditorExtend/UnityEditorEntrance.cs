using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Linq;
using Yojoy.Tech.Common.Core.Run;
using StringAssemblyMap = System.Collections.Generic.Dictionary<string, System.Reflection.Assembly>;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;
using StateChangeHandlerMap = System.Collections.Generic.Dictionary<
    UnityEditor.PlayModeStateChange, System.Collections.Generic.List
    <Yojoy.Tech.U3d.Core.Editor.IEditorStateChangeHandler>>;

namespace Yojoy.Tech.U3d.Core.Editor
{
    [InitializeOnLoad]
    public static class UnityEditorEntrance
    {
        #region Entrance
        static UnityEditorEntrance()
        {
            InitMultiLanguageContext();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            OnPlayModeStateChanged(PlayModeStateChange.EnteredEditMode);
            Debug.Log("YojoyFramework is started");
        }
        private const string MultiLanguageStringPrefsKey = "MultiLanguageStringPrefsKey";

        public static void UpdateLanguageType(LanguageType languageType)
        {
            EditorPrefs.SetString(MultiLanguageStringPrefsKey, languageType.ToString());
        }

        public static LanguageType GetCurrentLanguageType()
        {
            var languageType = EditorPrefs.GetString(key: MultiLanguageStringPrefsKey,
                defaultValue: LanguageType.English.ToString()).AsEnum<LanguageType>();
            return languageType;
        }
        private static void InitMultiLanguageContext() => MultiLanguageString.SetLanguageType(GetCurrentLanguageType());

        #endregion
        #region Assemblies
        public static readonly DelayInitializationProperty<StringAssemblyMap>
            EditorAssemblies =
            CreateDelayInitializationProperty(InitEditorAssemblies);
        public static readonly DelayInitializationProperty<StringAssemblyMap>
            RunAssemblies =
            CreateDelayInitializationProperty(() => InitRunAssemblies());
        public static readonly DelayInitializationProperty<Assembly[]>
            EditorAssemblyArrary = CreateDelayInitializationProperty(() =>
            EditorAssemblies.Value.Values.ToArray());


        private static StringAssemblyMap InitRunAssemblies()
        {
            var tempStringAssemblyMap = new StringAssemblyMap();
            InitAssembliesAtDirectory(YojoyEditorSettings.CorePackagesDirectory.Value,
                tempStringAssemblyMap, assemblyTypeId: "Run");
            InitAssembliesAtDirectory(YojoyEditorSettings.ExtendPackagesDirectory.Value,
                tempStringAssemblyMap, assemblyTypeId: "Run");
            return tempStringAssemblyMap;
        }

        private static StringAssemblyMap InitEditorAssemblies()
        {
            var tempStringAssemblyMap = new StringAssemblyMap();
            InitAssembliesAtDirectory(YojoyEditorSettings.CorePackagesDirectory.Value,
                tempStringAssemblyMap, assemblyTypeId: "Editor");
            InitAssembliesAtDirectory(YojoyEditorSettings.ExtendPackagesDirectory.Value,
                tempStringAssemblyMap, assemblyTypeId: "Editor");
            return tempStringAssemblyMap;
        }

        private static void InitAssembliesAtDirectory(string packageDirectory,
            StringAssemblyMap stringAssemblyMap, string assemblyTypeId)
        {
            var packageDirectories = DirectoryUtility.GetAllFirstSonDirectory(packageDirectory);
            foreach (var item in packageDirectories)
            {
                var packageName = DirectoryUtility.GetDirectoryName(item);
                var assemblyId = packageName + assemblyTypeId;
                if (YojoyEditorSettings.IgnoreEditorAssemblyIds.Value.Contains(assemblyTypeId))
                {
                    continue;
                }
                var assemblyPath = UnityEditorConstant.scriptAssembliesDirectory.Value +
                    assemblyId + ".dll";
                if (!File.Exists(assemblyPath))
                {
                    continue;
                }
                var assembly = Assembly.LoadFile(assemblyPath);
                stringAssemblyMap.Add(assemblyId, assembly);
            }
        }

        #endregion
        #region EditorPlayModeStateChange
        private static readonly
            DelayInitializationProperty<StateChangeHandlerMap>
            stateChangeHandlersDelay = CreateDelayInitializationProperty(() =>
              {
                  var tempMap = new StateChangeHandlerMap();
                  var handlers = ReflectionUtility.GetAllInstance
                  <IEditorStateChangeHandler>(EditorAssemblyArrary.Value);
                  foreach (var handler in handlers)
                  {
                      if (!tempMap.ContainsKey(handler.ConcernedStateChange))
                      {
                          tempMap.Add(handler.ConcernedStateChange,
                              new List<IEditorStateChangeHandler>());
                      }
                      var targetHandlers = tempMap[handler.ConcernedStateChange];
                      if (!targetHandlers.Contains(handler))
                      {
                          targetHandlers.Add(handler);
                      }
                  }
                  return tempMap;
              });

        private static void OnPlayModeStateChanged(
            PlayModeStateChange playModeStateChange)
        {
            stateChangeHandlersDelay.Value.TryGetValue
                (playModeStateChange).value?.ForEach
                (handler => handler.Handle());
        }

        #endregion
    }
}

