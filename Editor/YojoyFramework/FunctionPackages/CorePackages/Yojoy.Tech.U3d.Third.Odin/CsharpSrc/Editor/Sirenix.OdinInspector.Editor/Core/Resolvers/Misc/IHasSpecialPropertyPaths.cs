#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="IHasSpecialPropertyPaths.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    public interface IHasSpecialPropertyPaths
    {
        string GetSpecialChildPath(int childIndex);
    }
}
#endif
#pragma warning enable