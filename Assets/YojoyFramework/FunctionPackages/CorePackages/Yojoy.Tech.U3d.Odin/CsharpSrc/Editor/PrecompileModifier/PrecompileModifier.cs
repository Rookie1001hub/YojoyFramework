#region Comment Head

// Author:        liuruoyu1981
// CreateDate:    2020/1/12 16:07:41
// Email:         xxxx@qq.com

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.U3d.Odin.Editor
{
    [System.Serializable]
    public class PrecompileModifier : AbstractOnceFirstActiveItem
    {
        [SerializeField] [LabelText("Precompile nodes", "预编译配置")]
        private List<PrecompileConfigNode> precompileConfigNodes;

        public List<PrecompileConfigNode> PrecompileConfigNodes
            => precompileConfigNodes;

        [Button("Reload Precompile context"
            , "重载预编译配置", ButtonSizes.Medium)]
        private void ReloadPrecompileContext()
        {
            precompileConfigNodes = new List<PrecompileConfigNode>();
            var enums = Enum.GetValues(typeof(BuildPlatformType));

            foreach (BuildPlatformType platformType in enums)
            {
                var buildTargetGroup = platformType.ToString()
                    .AsEnum<BuildTargetGroup>();
                var platformSymbols = PlayerSettings
                    .GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                if (string.IsNullOrEmpty(platformSymbols))
                {
                    continue;
                }

                var instructions = platformSymbols
                    .Split(';');

                foreach (var instruction in instructions)
                {
                    var targetNode = EnsureNodeExist(instruction);
                    targetNode.BuildPlatformType |= platformType;
                }

                PrecompileConfigNode EnsureNodeExist(string instruction)
                {
                    var targetNode = precompileConfigNodes
                        .Find(n => n.Instruction
                                   == instruction);
                    if (targetNode == null)
                    {
                        targetNode = new PrecompileConfigNode {Instruction = instruction};
                        targetNode.BuildPlatformType = BuildPlatformType.Standalone;
                        precompileConfigNodes.Add(targetNode);
                    }

                    return targetNode;
                }
            }
        }

        [Button("Apply modification", "应用修改",ButtonSizes.Medium)]
        private void ApplyModification()
        {
            var buildTargetInstructionMap = new Dictionary<BuildTargetGroup, string>();

            ParsePrecompileNodes();
            CleanAllPrecompileSettings();
            UpdatePrecompileSettings();

            void ParsePrecompileNodes()
            {
                foreach (var configNode in precompileConfigNodes)
                {
                    if (configNode.BuildPlatformType.ToString() == "0")
                    {
                        continue;
                    }

                    var platformTypeStrings = configNode.BuildPlatformType
                        .ToString().Split(',');

                    foreach (var platformTypeString in platformTypeStrings)
                    {
                        var buildTargetGroup = platformTypeString
                            .AsEnum<BuildTargetGroup>();
                        if (!buildTargetInstructionMap.ContainsKey(buildTargetGroup))
                        {
                            buildTargetInstructionMap.Add(buildTargetGroup,
                                configNode.Instruction);
                        }
                        else
                        {
                            var existInstruction = buildTargetInstructionMap
                                [buildTargetGroup];
                            var newInstruction = existInstruction
                                                 + ";" + configNode.Instruction;
                            buildTargetInstructionMap[buildTargetGroup] = newInstruction;
                        }
                    }
                }
            }

            void CleanAllPrecompileSettings()
            {
                var platformTypes = CommonExtend.GetAllEnumValues<
                    BuildPlatformType>();

                foreach (var platformType in platformTypes)
                {
                    var buildTargetGroup = platformType.ToString()
                        .AsEnum<BuildTargetGroup>();
                }
            }

            void UpdatePrecompileSettings()
            {
                foreach (var pair in buildTargetInstructionMap)
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(
                        pair.Key, pair.Value);
                }
            }
        }

        protected override void DoActive() => ReloadPrecompileContext();
    }
}