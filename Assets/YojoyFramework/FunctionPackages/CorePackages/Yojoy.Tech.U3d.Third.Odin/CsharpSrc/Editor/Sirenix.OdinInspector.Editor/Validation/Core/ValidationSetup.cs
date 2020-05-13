#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ValidationSetup.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Validation
{
    using System.Reflection;

    public struct ValidationSetup
    {
        public Validator Validator;
        public MemberInfo Member;
        public object Value;
        public object ParentInstance;
        public ValidationKind Kind;
        public UnityEngine.Object Root;
    }
}
#endif
#pragma warning enable