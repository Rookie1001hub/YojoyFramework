using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;
namespace Yojoy.Tech.U3d.Core.Editor
{
    public class UnityEditorConstant
    {
        public static readonly DelayInitializationProperty<string>
            scriptAssembliesDirectory = CreateDelayInitializationProperty(() =>
            UnityEngine.Application.dataPath.Replace(oldValue:"Assets",newValue:"")
            +"Library/ScriptAssemblies/");
    }

}
