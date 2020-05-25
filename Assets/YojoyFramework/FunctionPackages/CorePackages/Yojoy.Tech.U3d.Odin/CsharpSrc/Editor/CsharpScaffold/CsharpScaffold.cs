using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;
using Yojoy.Tech.U3d.Core.Run;

namespace Yojoy.Tech.U3d.Odin.Editor
{
    [System.Serializable]
    public class CsharpScaffold 
    {
        #region Visualiztion
        public static string OutputDirectoryPrefsKey => UnityGlobalUtility
            .GetPrefsKey("OutputDirectory", typeof(CsharpScaffold));

        [BoxGroup("Base Config","基础配置")]
        [TextArea(2,3)]
        [SerializeField]
        [ReadOnly]
        [OnValueChanged(methodName:"OnOutputDirectoryChanged")]
        private string outputDirectory;

        private void OnOutputDirectoryChanged()
        {
            EditorPrefs.SetString(OutputDirectoryPrefsKey, outputDirectory);
        }
        [LabelText("CopyRight Content", "版权信息")]
        [TextArea(3, 6)]
        [SerializeField]
        [OnValueChanged("OnCopyRightContentChanged")]
        private string copyRightContent;

        private string CopyRightPrefsKey => UnityGlobalUtility.GetPrefsKey(
            "CopyRightContent", typeof(CsharpScaffold));

        private void OnCopyRightContentChanged()
        {
            EditorPrefs.SetString(CopyRightPrefsKey, copyRightContent);
        }

        [BoxGroup("Global Config", "全局配置")]
        [OnValueChanged("OnGlobalNameSpaceChanged")]
        [LabelText("Global Namespace", "全局命名空间")]
        [SerializeField]
        private string globalNameSpace;

        private string GlobalNamespacePrefsKey =>
            UnityGlobalUtility.GetPrefsKey(
                "GlobalNamespace", typeof(CsharpScaffold));

        private void OnGlobalNameSpaceChanged()
        {
            EditorPrefs.SetString(GlobalNamespacePrefsKey, globalNameSpace);
            IfPrecompile = globalNameSpace.EveryToBig().Replace(
                ".", "_");
            OnIfPrecompileChanged();
            TryCloseAutoAddIfPrecompile();
        }
        private void TryCloseAutoAddIfPrecompile()
        {
            //requireAddIfPrecompile =
            //    !ModuleUtility.IsCoreModule(globalNameSpace);
        }

        [BoxGroup("Global Config", "全局配置")]
        [LabelText("If Precompile", "IF预编译指令")]
        [OnValueChanged("OnIfPrecompileChanged")]
        [SerializeField]
        private string IfPrecompile;

        private string IfPrecompilePrefsKey => UnityGlobalUtility
            .GetPrefsKey("IfPrecompile", typeof(CsharpScaffold));


        [BoxGroup("Global Config", "全局配置")]
        [LabelText("Require Add IfPrecompile",
            "是否需要添加if预编译指令")]
        [SerializeField]
        private bool requireAddIfPrecompile = true;

        private void OnIfPrecompileChanged()
        {
            EditorPrefs.SetString(IfPrecompilePrefsKey, IfPrecompile);
        }

        [BoxGroup("Developer Info", "开发者信息")]
        [HideLabel]
        [SerializeField]
        private DevelopInfo developerInfo;

        [LabelText("Csharp Create Infos", "脚本创建信息")]
        [HideLabel]
        [SerializeField]
        private List<CsharpCreateInfo> csharpCreateInfos;

        #endregion
    }
}

