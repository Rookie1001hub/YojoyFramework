#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="EnableGUIExample.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    [AttributeExample(typeof(EnableGUIAttribute))]
    internal class EnableGUIExample
    {
        [ShowInInspector]
        public int GUIDisabledProperty { get { return 10; } }

        [ShowInInspector, EnableGUI]
        public int GUIEnabledProperty { get { return 10; } }
    }
}
#endif
#pragma warning enable