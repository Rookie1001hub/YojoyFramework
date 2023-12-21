#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="DetailedInfoBoxExample.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using System;
    using UnityEngine;

    [AttributeExample(typeof(DetailedInfoBoxAttribute))]
    internal class DetailedInfoBoxExample
    {
        [DetailedInfoBox("Click the DetailedInfoBox...",
            "... to reveal more information!\n" +
            "This allows you to reduce unnecessary clutter in your editors, and still have all the relavant information available when required.")]
        public int Field;
    }
}
#endif
#pragma warning enable