using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Yojoy.Tech.Common.Core.Run;
namespace Yojoy.Tech.U3d.Odin.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MenuWindowTitleAttribute : Attribute
    {
        public MultiLanguageString TitleString { get; private set; }

        public MenuWindowTitleAttribute(string english,string chinese)
        {
           TitleString = MultiLanguageString.Create(english,chinese);
        }
    }
}

