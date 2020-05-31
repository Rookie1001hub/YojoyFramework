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
using Yojoy.Tech.U3d.Core.Editor;
using System.IO;

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
        /// <summary>
        /// 获取当前命名空间是否在核心目录下
        /// </summary>
        private void TryCloseAutoAddIfPrecompile()
        {
            requireAddIfPrecompile =
                !ModuleUtility.IsCoreModule(globalNameSpace);
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
                        return "abstract class";
                    case CsharpScriptType.Interface:
                        return "interface";
                    case CsharpScriptType.Enum:
                        return "enum";
                    case CsharpScriptType.Struct:
                        return "struct";
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
            LoadDevelopInfo();
            UpdateOutputDirectory();
        }
        private readonly string DevelopPrefsKey = UnityGlobalUtility.
            GetPrefsKey("DevelopInfo", typeof(CsharpScaffold));
        /// <summary>
        /// 借助Json来持久化Prefs数据
        /// </summary>
        private void LoadDevelopInfo()
        {
            var jsonString = EditorPrefs.GetString(DevelopPrefsKey);
            if (string.IsNullOrEmpty(jsonString))
                return;
            developerInfo = JsonUtility.FromJson<DevelopInfo>(jsonString);
        }
        private void SaveDeveloperInfo()
        {
            var jsonString = JsonUtility.ToJson(developerInfo);
            EditorPrefs.SetString(DevelopPrefsKey, jsonString);
        }
        /// <summary>
        /// 利用Prefs获取当前输出目录
        /// 如果获取输出目录拿到的模块id为空直接返回
        /// 不为空则更改全局命名空间
        /// </summary>
        private void UpdateOutputDirectory()
        {
            outputDirectory = EditorPrefs.GetString(OutputDirectoryPrefsKey);
            var moduleId = ModuleUtility.GetModuleId(outputDirectory);
            if (moduleId == null)
            {
                return;
            }
            globalNameSpace = moduleId;
            OnGlobalNameSpaceChanged();
        }
        #endregion
        #region CreateScript
        private string GetScriptOutPath(CsharpCreateInfo createInfo,
            string directory)
        {
            var finalPath = directory.EnsureDirectoryFormat() +
                createInfo.ScriptName + ".cs";
            return finalPath;
        }
        private bool CheckCreateInfoError()
        {
            if (!outputDirectory.IsValid())
            {
                UnityEditorUtility.DisplayTip("No script cretion" +
                    "directory selected!");
                return true;
            }
            if (!Directory.Exists(outputDirectory))
            {
                UnityEditorUtility.DisplayTip(
                    "The target directory is not a valid directory!");
                return true;
            }
            if (csharpCreateInfos == null || csharpCreateInfos.Count == 0)
            {
                UnityEditorUtility.DisplayTip(
                    "Create information cannot be empty");
                return true;
            }
            foreach (var createInfo in csharpCreateInfos)
            {
                if (!createInfo.ScriptName.IsValid())
                {
                    UnityEditorUtility.DisplayTip(
                        "Script Name cannot be empty");
                    return true;
                }
                if (!globalNameSpace.IsValid())
                {
                    UnityEditorUtility.DisplayTip(
                        "Global namespace cannot be empty");
                    return true;
                }
                var finalPath = GetScriptOutPath(createInfo, outputDirectory);
                if (File.Exists(finalPath))
                {
                    UnityEditorUtility.DisplayTip(
                        "Script already exists on the target path!");
                    return true;
                }
            }
            return false;
        }
        private void CreateScript()
        {
            if (CheckCreateInfoError())
            {
                return;
            }
            var appender = new CsharpScriptAppender();
            foreach (var createInfo in csharpCreateInfos)
            {
                appender.Clean();
                var scriptPath = GetScriptOutPath(createInfo, outputDirectory);
                var scripteContent = GetScriptContent(appender, createInfo);
                FileUtility.WriteAllText(scriptPath, scripteContent);
            }
            SaveDeveloperInfo();
            AssetDatabase.Refresh();
            UnityEditorUtility.DisplayTip("Script is created");
        }
        #endregion

        //两种脚本创建方法
        //1.弹出目录选择窗口，开发者选择指定输出目录
        //2.直接使用当前脚本输出目录创建脚本
        #region Visual Invoke
        [Button("Create script in selcet directory","选择" +
            "目录创建脚本",ButtonSizes.Medium)]
        private void CreateScriptInSelectDirectory()
        {
            var defaultDirectory = outputDirectory ?? Application.dataPath;
            var targetDirectory = EditorUtility.OpenFolderPanel
                ("Please select script output directory",
                defaultDirectory, null);
            if (!Directory.Exists(targetDirectory))
            {
                UnityEditorUtility.DisplayTip("No directory selected!");
                return;
            }
            outputDirectory = targetDirectory.EnsureDirectoryFormat();
            CreateScript();
        }
        [Button("Create script in output directory",
            "在输出目录下创建脚本",ButtonSizes.Medium)]
        private void CreateScriptOutputDirectory() => CreateScript();
        #endregion
    }
}

