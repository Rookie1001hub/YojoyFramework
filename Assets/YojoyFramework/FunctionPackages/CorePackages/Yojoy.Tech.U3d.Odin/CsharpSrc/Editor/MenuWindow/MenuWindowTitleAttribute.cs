using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Yojoy.Tech.U3d.Odin.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MenuWindowTitleAttribute : Attribute
    {
        public string Title { get; private set; }
        public MenuWindowTitleAttribute(string str)
        {
            Title = str;
        }
    }
}

