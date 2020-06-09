#region Comment Head


#endregion

using Sirenix.OdinInspector;

namespace Yojoy.Tech.U3d.Odin.Editor
{
    [System.Serializable]
    public class PrecompileConfigNode
    {
        [GUIColor(0.6f,0.8f,0.8f)]
        [LabelText("Precompile instruction",
            "预编译指令")]
        public string Instruction;

        [LabelText("Platform type","编译平台")]
        public BuildPlatformType BuildPlatformType
            = BuildPlatformType.Android
              | BuildPlatformType.iOS
              | BuildPlatformType.WebGL
              | BuildPlatformType.Standalone;
    }
}
