using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Yojoy.Tech.Common.Core.Run
{
    [SerializeField]
    public class MultiLanguageString
    {
        #region Data structure
        protected string EnglishValue;
        protected string ChineseValue;

        private static readonly Dictionary<string, MultiLanguageString>
            globalStrings = new Dictionary<string, MultiLanguageString>();
        private static LanguageType globalLanguageType;

        public static void SetLanguageType(LanguageType languageType)
            => globalLanguageType = languageType;

        public virtual string Text
        {
            get
            {
                string text;
                switch (globalLanguageType)
                {
                    case LanguageType.English:
                        text = EnglishValue;
                        break;
                    case LanguageType.Chinese:
                        text = ChineseValue;
                        break;
                    default:
                        throw new ArgumentNullException();
                }
                return text;
            }
        }
        #endregion
        #region Create instance 
        public MultiLanguageString(string englishValue,string chineseValue)
        {
            EnglishValue = englishValue;
            ChineseValue = chineseValue;
        }

        public static MultiLanguageString Create(string englishValue,string chinesValue=null)
        {
            MultiLanguageString targetString;
            if (globalStrings.ContainsKey(englishValue))
            {
                targetString = globalStrings[englishValue];
            }
            else
            {
                targetString = new MultiLanguageString(englishValue, chinesValue);
                globalStrings.Add(englishValue, targetString);
            }
            return targetString;
        }
        #endregion
    }
}

