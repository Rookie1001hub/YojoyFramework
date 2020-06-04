#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/2 23:18:32
//Email:         854327817@qq.com

#endregion

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;
namespace Yojoy.Tech.U3d.Core.Editor
{
    public class ProjectEntrance : IEditorStateChangeHandler
    {
        public PlayModeStateChange ConcernedStateChange
            => PlayModeStateChange.EnteredEditMode;

        public void Handle()
        {
           EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
        }
        private readonly DelayInitializationProperty<List<IProjectExpander>>
            expanderDelay = CreateDelayInitializationProperty(() => 
            {
                var expanders = ReflectionUtility.GetAllInstance
                <IProjectExpander>(UnityEditorEntrance.
                EditorAssemblyArrary.Value);
                return expanders;
            });

        private void ProjectWindowItemOnGUI(string guid,
            Rect selectionRect)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
           
            path = path.EnsureDirectoryFormat();
            foreach (var item in expanderDelay.Value)
            {
                item.SaveContext(guid, path);
                if (!item.CheckContext())
                {
                    continue;
                }
                item.Execute(selectionRect);
            }
        }
    }
}
