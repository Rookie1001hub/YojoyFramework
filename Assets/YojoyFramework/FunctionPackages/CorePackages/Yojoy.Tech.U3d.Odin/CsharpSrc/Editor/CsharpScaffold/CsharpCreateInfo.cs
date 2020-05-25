using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.U3d.Odin.Editor
{
    [System.Serializable]
    public class CsharpCreateInfo
    {
        [GUIColor(0.6f,0.8f,0.8f)]
        [LabelText("Script Name","脚本名")]
        public string ScriptName;
        [LabelText("Script Type","脚本类型")]
        public CsharpScriptType CsharpScriptType;
        [LabelText("If Compile Instructions","预编译指令")]
        public List<string> IfPreCompileInstructions;
    }
}

