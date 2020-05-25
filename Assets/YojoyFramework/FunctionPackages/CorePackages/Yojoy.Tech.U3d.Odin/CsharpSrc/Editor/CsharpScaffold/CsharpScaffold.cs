using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;
using Yojoy.Tech.Common.Core.Editor;
using Yojoy.Tech.U3d.Core.Run;
using System;

namespace Yojoy.Tech.U3d.Odin.Editor
{
    [System.Serializable]
    public class CsharpScaffold
    {
        #region Visualiztion
        public static string OutputDirectoryPrefsKey => UnityGlobalUtility
            .GetPrefsKey("OutputDirectory", typeof(CsharpScaffold));

        [BoxGroup("Base Config", "基础配置")]
        [TextArea(2, 3)]
        [SerializeField]
        [ReadOnly]
        [OnValueChanged(methodName: "OnOutputDirectoryChanged")]
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

        #region GetScriptContent
        /// <summary>
        /// 获取脚本内容
        /// </summary>
        /// <param name="csharpScriptAppender"></param>
        /// <param name="csharpCreateInfo"></param>
        /// <returns></returns>
        private string GetScriptContent(CsharpScriptAppender scriptAppender,
            CsharpCreateInfo csharpCreateInfo)
        {
            #region Local Method
            //尝试添加版权信息
            TryAppendCopyRight();

            if (requireAddIfPrecompile)
            {
                using (new IfPreCompileBlock(scriptAppender,
                    new List<string> { IfPrecompile }))
                {
                    scriptAppender.AppendLine();
                    scriptAppender.AppendCommentHeader(developerInfo.Name,
                        developerInfo.Email);
                    using (new IfPreCompileBlock(scriptAppender,
                        csharpCreateInfo.IfPreCompileInstructions))
                    {
                        using (new NameSpaceBlock(scriptAppender, globalNameSpace))
                        {
                            AppendScriptContent();
                        }
                    }
                }
            }
            else
            {
                scriptAppender.AppendCommentHeader(developerInfo.Name,
                    developerInfo.Email);
                using (new NameSpaceBlock(scriptAppender, globalNameSpace))
                {
                    AppendScriptContent();
                }
            }
            return scriptAppender.ToString();

            void AppendScriptContent()
            {
                var clasaHeadString = string.Format("public {0} {1}",
                    GetStringKeyword(csharpCreateInfo.CsharpScriptType),
                    csharpCreateInfo.ScriptName);
                scriptAppender.AppendLine(clasaHeadString);
                scriptAppender.AppenLeftBracketAndToRight();
                scriptAppender.AppendLine();
                scriptAppender.AppendToLeftAndRightBracket();
            }
            string GetStringKeyword(CsharpScriptType csharpScriptType)
            {
                switch (csharpScriptType)
                {
                    case CsharpScriptType.Class:
                        return "class";
                    case CsharpScriptType.AbstractClass:
                        return "abstrcat class";
                    case CsharpScriptType.Interface:
                        return "interface";
                    case CsharpScriptType.Enum:
                        return "enum";
                    case CsharpScriptType.Struct:
                        return "struce";
                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(csharpScriptType),csharpScriptType,null);
                }
            }
            void TryAppendCopyRight()
            {
                if (!copyRightContent.IsValid())
                    return;
                scriptAppender.AppendLine(copyRightContent);
                scriptAppender.AppendLine();
            }
            #endregion
        }
        #endregion

        #region Content Handle

        public CsharpScaffold()
        {
            copyRightContent = EditorPrefs.GetString(CopyRightPrefsKey);
            globalNameSpace = EditorPrefs.GetString(GlobalNamespacePrefsKey);
            IfPrecompile = EditorPrefs.GetString(IfPrecompilePrefsKey);

        }
        private readonly string DevelopPrefsKey = UnityGlobalUtility.
            GetPrefsKey("DevelopInfo", typeof(CsharpScaffold));
        /// <summary>
        /// 借助Json来持久话Prefs数据
        /// </summary>
        private void LoadDevelopInfo()
        {
            var jsonString = EditorPrefs.GetString(DevelopPrefsKey);
            if (string.IsNullOrEmpty(jsonString))
                return;
            developerInfo = JsonUtility.FromJson<DevelopInfo>(jsonString);
        }
        private void SaveDevelopInfo()
        {
            var jsonString = JsonUtility.ToJson(developerInfo);
            EditorPrefs.SetString(DevelopPrefsKey, jsonString);
        }
        #endregion
    }
}

