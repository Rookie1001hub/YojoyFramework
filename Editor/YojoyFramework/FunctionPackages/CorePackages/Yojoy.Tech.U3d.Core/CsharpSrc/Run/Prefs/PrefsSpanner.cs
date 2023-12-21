#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/6/5 17:36:23
//Email:         854327817@qq.com

#endregion

using Sirenix.OdinInspector;
using System;
using System.IO;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.U3d.Core.Run
{
    [Serializable]
    public class PrefsSpanner
    {
        [HideLabel]
        [SerializeField]
        private PrefsDatabase prefsDatabase;
        /// <summary>
        /// prefs二进制数据位置
        /// </summary>
        private string DatabasePath => UnityGlobalUtility.IsEditorMode ?
            Application.dataPath + "/.prefs.binary" :
            Application.persistentDataPath + "/.prefs.binary";

        private void LoadPrefsDatabase()
        {
            if (!File.Exists(DatabasePath))
            {
                prefsDatabase = new PrefsDatabase();
                return;
            }
            var bytes = File.ReadAllBytes(DatabasePath);
            if (bytes.Length==0)
            {
                return;
            }
            prefsDatabase = SerializeUtility.DeSerialize<PrefsDatabase>(bytes);
        }
        #region Singleton

        private PrefsSpanner() => LoadPrefsDatabase();
        public static readonly PrefsSpanner Instance = new PrefsSpanner();

        #endregion
        #region Save Delete

        [Button("Save","保存",ButtonSizes.Medium)]
        private void Save()
        {
            var bytes = SerializeUtility.Serialize(prefsDatabase);
            FileUtility.WriteAllBytes(DatabasePath, bytes);
        }
        private void DelteAll()
        {
            prefsDatabase = null;
            Save();
        }
        #endregion
        #region SetPrefs

        private void SetValue<TPrefsValueStorage,TPrefsValue,TValue>
            (
            string key,
            TValue value,
            AbstractPrefsValueStorageGeneric<TPrefsValueStorage,TPrefsValue,TValue>
            prefsValues,
            string description
            )
            where TPrefsValue:IPrefsValueGeneric<TValue>,new()
            where TPrefsValueStorage:AbstractPrefsValueSetGeneric<TPrefsValue,
                TValue>,new()
        {
            var prefs = new TPrefsValue();
            prefs.Init(key, value, description);
            prefsValues.PrefsValueSet.AddOrUpdatePrefsValue(prefs);
            Save();
        }
        public void SetString(string key,string value,
            string description = null)
        {
            SetValue(key, value, prefsDatabase.stringPrefsValueStorage, description);
        }
        public void SetBool(string key, bool value,
            string description = null)
        {
            SetValue(key, value, prefsDatabase.boolPrefsValueStorage, description);
        
        }
        public void SetFloat(string key, float value,
            string description = null)
        {
            SetValue(key, value, prefsDatabase.floatPrefsValueStorage, description);
        }

        public void SetInt(string key, int value,
            string description = null)
        {
            SetValue(key, value, prefsDatabase.intPrefsValueStorage, description);
        }
        #endregion
        #region GetPrefs

        private TValue GetValue<TPrefsStorage, TPrefsValue, TValue>
            (
            string key,
            TValue defaultValue,
            AbstractPrefsValueStorageGeneric<TPrefsStorage,TPrefsValue,TValue>
            prefsStorage,
            string description
            )
            where TPrefsValue:IPrefsValueGeneric<TValue>,new()
            where TPrefsStorage:
            AbstractPrefsValueSetGeneric<TPrefsValue,TValue>,new()
        {
            var prefs = prefsStorage.GetPrefsValue(key);
            if (prefs!=null)
            {
                return prefs.Value;
            }
            SetValue(key, defaultValue, prefsStorage, description);
            return defaultValue;
        }
        public string GetString(string key,string defaultValue
            ,string description = null)
        {
            var prefs = GetValue(key, description, prefsDatabase.
                stringPrefsValueStorage, description);
            return prefs;
        }
        public bool GetBool(string key, bool defaultValue,
            string description = null)
        {
            var prefs = GetValue(key, defaultValue, prefsDatabase
                .boolPrefsValueStorage, description);
            return prefs;
        }

        public float GetFloat(string key, float defaultValue,
            string description = null)
        {
            var prefs = GetValue(key, defaultValue, prefsDatabase
                .floatPrefsValueStorage, description);
            return prefs;
        }

        public int GetInt(string key, int defaultValue,
            string description = null)
        {
            var prefs = GetValue(key, defaultValue,
                prefsDatabase.intPrefsValueStorage,
                description);
            return prefs;
        }
        #endregion
    }
}
