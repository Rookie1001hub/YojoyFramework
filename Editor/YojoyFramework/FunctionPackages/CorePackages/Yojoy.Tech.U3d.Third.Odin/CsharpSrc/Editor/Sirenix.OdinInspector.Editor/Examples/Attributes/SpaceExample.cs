#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="SpaceExample.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using System;
    using UnityEngine;

    [AttributeExample(typeof(SpaceAttribute))]
    [AttributeExample(typeof(PropertySpaceAttribute))]
    internal class SpaceExample
    {
        // Delayed and Space attributes are virtually identical...
        [Space]
        public int Space;

        // ... but the PropertySpace can, as the name suggests, also be applied to properties.
        [ShowInInspector, PropertySpace]
        public string Property { get; set; }

        // You can also control spacing both before and after the PropertySpace attribute.
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 60), PropertyOrder(2)]
        public int BeforeAndAfter;
    }
}
#endif
#pragma warning enable