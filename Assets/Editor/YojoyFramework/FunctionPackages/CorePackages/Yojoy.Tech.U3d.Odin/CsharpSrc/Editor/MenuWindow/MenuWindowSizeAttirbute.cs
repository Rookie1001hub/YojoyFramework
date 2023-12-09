using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yojoy.Tech.U3d.Odin.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MenuWindowSizeAttirbute : Attribute
    {
        public int Min { get; private set; }
        public int Max { get; private set; }
        public MenuWindowSizeAttirbute(int min,int max)
        {
            Min = min;
            Max = max;
        }
    }
}


