#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="IValidatorLocator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public interface IValidatorLocator
    {
        bool PotentiallyHasValidatorsFor(Type valueType);
        bool PotentiallyHasValidatorsFor(MemberInfo member, Type memberValueType, bool isCollectionElement);
        IList<Validator> GetValidators(Type valueType);
        IList<Validator> GetValidators(MemberInfo member, Type memberValueType, bool isCollectionElement);
    }
}
#endif
#pragma warning enable