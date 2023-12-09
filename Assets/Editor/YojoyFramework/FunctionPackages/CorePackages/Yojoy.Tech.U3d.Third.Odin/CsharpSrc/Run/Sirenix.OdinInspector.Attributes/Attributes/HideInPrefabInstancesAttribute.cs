#pragma warning disable
//-----------------------------------------------------------------------
// <copyright file="HideInPrefabInstancesAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// Hides a property if it is drawn from a prefab instance.
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All)]
    public class HideInPrefabInstancesAttribute : Attribute
    {
    }
}
#pragma warning enable