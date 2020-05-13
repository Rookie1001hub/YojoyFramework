using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Yojoy.Tech.U3d.Odin.Editor
{
    public interface IWindowMenuBuilder
    {
        Type MapWindowType { get; }

        void BuildMenu(OdinMenuTree menuTree);
    }
}

