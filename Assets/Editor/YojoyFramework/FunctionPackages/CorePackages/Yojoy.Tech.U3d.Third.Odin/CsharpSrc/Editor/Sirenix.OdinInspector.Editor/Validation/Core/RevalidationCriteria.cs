#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="RevalidationCriteria.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Validation
{
    public enum RevalidationCriteria
    {
        Always,
        OnValueChange,
        OnValueChangeOrChildValueChange
    }
}
#endif
#pragma warning enable