#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/5 10:46:09
//Email:         854327817@qq.com

#endregion


using System;
using System.Collections.Generic;

namespace Yojoy.Tech.U3d.Core.Run
{
    [Serializable]
    public abstract class AbstractPrefsValueSetGeneric<TPrefsValue,TValue>
        where TPrefsValue:IPrefsValueGeneric<TValue>,new()
    {
        public readonly List<TPrefsValue> prefsValues =
            new List<TPrefsValue>();
        public TPrefsValue GetPrefsValue(string key)
        {
            var prefsValue = prefsValues.Find(match: p => p.Key == key);
            return prefsValue;
        }
        public void AddOrUpdatePrefsValue(TPrefsValue prefsValue)
        {
            var exitsPrfs = GetPrefsValue(prefsValue.Key);
            if (exitsPrfs != null)
            {
                prefsValues.Remove(exitsPrfs);
            }
            prefsValues.Add(prefsValue);
        }
        public void DeletPrefsValue(string key)
        {
            var existPrefs = GetPrefsValue(key);
            if (existPrefs != null)
            {
                prefsValues.Remove(existPrefs);
            }
        }
    }
}
