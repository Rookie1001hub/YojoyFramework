#pragma warning disable
//-----------------------------------------------------------------------
// <copyright file="HideInPrefabAssetsAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// Hides a property if it is drawn from a prefab asset.
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All)]
    public class HideInPrefabAssetsAttribute : Attribute
    {
    }
}
#pragma warning enable