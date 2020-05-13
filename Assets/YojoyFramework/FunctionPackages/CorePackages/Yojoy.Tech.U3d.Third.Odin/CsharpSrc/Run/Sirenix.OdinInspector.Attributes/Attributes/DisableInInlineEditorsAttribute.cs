#pragma warning disable
//-----------------------------------------------------------------------
// <copyright file="DisableInInlineEditorsAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// Disables a property if it is drawn within an <see cref="InlineEditorAttribute"/>.
    /// </summary>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All)]
    public class DisableInInlineEditorsAttribute : Attribute
    {
    }
}
#pragma warning enable