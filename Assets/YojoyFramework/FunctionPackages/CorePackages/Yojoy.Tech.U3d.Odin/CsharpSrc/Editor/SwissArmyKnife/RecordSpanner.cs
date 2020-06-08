#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/8 22:45:41
//Email:         854327817@qq.com

#endregion

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;
using Yojoy.Tech.U3d.Core.Editor;
using Yojoy.Tech.U3d.Core.Run;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;
namespace Yojoy.Tech.U3d.Odin.Editor
{
    [Serializable]
    public class RecordSpanner
    {
        [SerializeField]
        [LabelText("Record name","记录名")]
        private string recordName;

        private readonly DelayInitializationProperty<List<Type>>
            recordTypeDelay = CreateDelayInitializationProperty(() =>
              {
                  var allTypes = new List<Type>();
                  var editorTypes = ReflectionUtility.GetTypeList<IRecord>
                  (false, false,
                  UnityEditorEntrance.EditorAssemblyArrary.Value);
                  var runTypes = ReflectionUtility.GetTypeList<IRecord>(
                      false, false,
                      UnityEditorEntrance.EditorAssemblyArrary.Value);
                  allTypes.AddRange(editorTypes);
                  allTypes.AddRange(runTypes);
                  return allTypes;
              });

        private void MakeCreateRecordMenu(RecordNumberType recordNumberType)
        {
            var genericMenu = new GenericMenu();
            foreach (var recordType in recordTypeDelay.Value)
            {
                var recordAttribute = recordType.GetSingleAttribute<RecordAttribute>();
                if (!SelectRecordType(recordAttribute))
                {
                    continue;
                }
                genericMenu.AddItem(new GUIContent(recordType.Name),
                    false, CreateRecord, recordType);
            }
            genericMenu.ShowAsContext();
            bool SelectRecordType(RecordAttribute recordAttribute)
            {
                var result = recordAttribute.RecordNumberType == recordNumberType;
                return result;
            }
            void CreateRecord(object data)
            {
                var type = (Type)data;
                UnityRecordLoader.Instance.LoadRecord(type, recordName);
                AssetDatabase.Refresh();
            }
        }

        [Button("Create single record","创建单例记录",ButtonSizes.Medium)]
        private void CreateSingleRecord()
        {
            MakeCreateRecordMenu(RecordNumberType.Singleton);
        }

        [Button("Create unlimited record","创建非单例实例",ButtonSizes.Medium)]
        private void CreateUnlimitedRecord()
        {
            if (!recordName.IsValid())
            {
                UnityEditorUtility.DisplayTip("Record name cannot be null!");
            }

            MakeCreateRecordMenu(RecordNumberType.Unlimited);
        }
    }
}
