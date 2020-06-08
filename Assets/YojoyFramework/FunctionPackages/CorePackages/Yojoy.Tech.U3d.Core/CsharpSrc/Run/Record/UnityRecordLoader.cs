#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/8 14:00:41
//Email:         854327817@qq.com

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;
using Yojoy.Tech.U3d.Core.Editor;
using static Yojoy.Tech.Common.Core.Run.CommonGlobalUtility;
namespace Yojoy.Tech.U3d.Core.Run
{
    public class UnityRecordLoader : IRecordLoader
    {
        private string recordRootDirectory;

        #region Singleton

        private UnityRecordLoader()
        {
            if (UnityGlobalUtility.IsEditorMode)
            {
                recordRootDirectory = Application.dataPath + "/YojoyFramework/Records";
            }
            else
            {
                recordRootDirectory = Application.persistentDataPath + "/YojoyFramework/Records";
            }
        }
        #endregion
        
        private RecordAttribute CheckMustAttribute(Type recordType)
        {
            var serializableAttribute = recordType.
                GetSingleAttribute<SerializableAttribute>();
            if (serializableAttribute == null)
            {
                throw new Exception("Cannot find serializable attribute");
            }
            var recordAttribute = recordType.GetSingleAttribute<RecordAttribute>();
            if (recordAttribute == null)
            {
                throw new Exception("Cannot find recorde attribute");
            }
            return recordAttribute;
        }
        private string GetTargetRecordRootDirectory(Type recordType,
           RecordAttribute recordAttribute)
        {
            var scopeString = recordAttribute.RecordScopeType + "/";
            var parentString = recordAttribute.RecordNumberType == RecordNumberType.Singleton ?
                null : recordType.Name+"/";
            var directory = recordRootDirectory + scopeString + parentString;
            return directory;
        }

        private string GetPath(Type recordType,RecordAttribute recordAttribute
            ,string recordName)
        {
            var targetRootDirectory = GetTargetRecordRootDirectory(recordType, recordAttribute);
            string path;
            string fileRecordName = recordName;
            if (!recordName.IsValid()&&recordAttribute.RecordNumberType!=
                RecordNumberType.Singleton)
            {
                var allPath = DirectoryUtility.GetAllDirectoryContainSelf(targetRootDirectory,
                    p => p.EndsWith(".json") && !p.EndsWith("/.json"));
                if (allPath.Count != 0)
                {
                    var firstPath = allPath.First();
                    fileRecordName = FileUtility.GetFileIdWithoutExtension(firstPath);
                }
                else
                {
                    return null;
                }
            }
            if (recordAttribute.RecordNumberType == RecordNumberType.Singleton)
            {
                path = targetRootDirectory + recordType.Name + ".json";
            }
            else 
            {
                path = targetRootDirectory + fileRecordName+ ".json";
            }
            return path;
        }
        private readonly DelayInitializationProperty<Dictionary<Type, object>>
            singleRecords = CreateDelayInitializationProperty(() => new Dictionary<Type, object>());

        private object LoadRecordAtPath(string path,Type recordType,
            RecordAttribute recordAttribute)
        {
            if (path == null)
            {
                return null;
            }
            object record;
            var recordName = FileUtility.GetFileIdWithoutExtension(path);
            if (File.Exists(path))
            {
                var content = File.ReadAllText(path);
                record = JsonUtility.FromJson(content, recordType);
            }
            else
            {
                record = Activator.CreateInstance(recordType);
                if (recordAttribute.RecordNumberType == RecordNumberType.Singleton)
                {
                    singleRecords.Value.Add(recordType, record);
                }
                else
                {
                    ReflectionUtility.SetProperty(record,
                        "RecordName", recordName);
                }
                var content = YojoyEditorAgent.GetBeautifieldJson(JsonUtility.ToJson(record));
                FileUtility.WriteAllText(path, content);
            }
            return record;
        }

        public object LoadRecord(Type recordType, string recordName)
        {
            var recordAttribute = CheckMustAttribute(recordType);
            var path = GetPath(recordType, recordAttribute, recordName);
            var record = LoadRecordAtPath(path, recordType, recordAttribute);
            return record;
        }

        public void SaveRecord(IRecord record, bool deleteExist = false)
        {
            var recordType = record.GetType();
            var recordAttribute = CheckMustAttribute(recordType);
            var path = GetPath(recordType, recordAttribute,
                record.RecordName);
            if (File.Exists(path) && deleteExist == false)
            {
                return;
            }

            var content = YojoyEditorAgent.GetBeautifiedJson(
                JsonUtility.ToJson(record));
            FileUtility.WriteAllText(path, content);
        }
        public List<TRecord> LoadRecords<TRecord>()
           where TRecord : class, new()
        {
            var recordType = typeof(TRecord);
            var recordAttribute = CheckMustAttribute(recordType);
            var recordDirectory = GetTargetRecordRootDirectory(
                recordType, recordAttribute);

            var paths = DirectoryUtility.GetPathsContainSonDirectory(
                recordDirectory, p => p.EndsWith(".json")
                                      && !p.EndsWith("/.json"));
            var records = new List<TRecord>();

            foreach (var path in paths)
            {
                var record = LoadRecordAtPath(path, recordType,
                    recordAttribute).As<TRecord>();
                records.Add(record);
            }

            return records;
        }
    }
}
