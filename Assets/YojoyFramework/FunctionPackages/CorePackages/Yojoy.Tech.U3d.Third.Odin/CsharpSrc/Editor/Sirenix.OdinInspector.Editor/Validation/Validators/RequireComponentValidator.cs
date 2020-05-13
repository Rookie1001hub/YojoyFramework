#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="RequireComponentValidator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.RequireComponentValidator<>))]

namespace Sirenix.OdinInspector.Editor.Validation
{
    using System;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using UnityEngine;
    

    public class RequireComponentValidator<T> : ValueValidator<T>
        where T : Component
    {
        private static readonly RequireComponent Attribute = typeof(T).GetAttribute<RequireComponent>(true);

        public override bool CanValidateValue(Type type)
        {
            return Attribute != null;
        }

        protected override void Validate(T value, ValidationResult result)
        {
            bool ignore = false;

            if ((value as UnityEngine.Object) == null)
                ignore = true;

            // If we have a root, then we only run this validation if we are doing it directly on the root
            if (result.Setup.Root != null && value != result.Setup.Root)
                ignore = true;

            if (ignore)
            {
                result.ResultType = ValidationResultType.IgnoreResult;
                return;
            }

            if (Attribute.m_Type0 != null)
            {
                if (value.gameObject.GetComponent(Attribute.m_Type0) == null)
                {
                    result.Message = "GameObject is missing required component of type '" + Attribute.m_Type0.GetNiceName() + "'";
                    result.ResultType = ValidationResultType.Error;
                }
            }

            if (Attribute.m_Type1 != null)
            {
                if (value.gameObject.GetComponent(Attribute.m_Type1) == null)
                {
                    result.Message += "\n\nGameObject is missing required component of type '" + Attribute.m_Type1.GetNiceName() + "'";
                    result.ResultType = ValidationResultType.Error;
                }
            }

            if (Attribute.m_Type2 != null)
            {
                if (value.gameObject.GetComponent(Attribute.m_Type2) == null)
                {
                    result.Message += "\n\nGameObject is missing required component of type '" + Attribute.m_Type2.GetNiceName() + "'";
                    result.ResultType = ValidationResultType.Error;
                }
            }
        }
    }
    
}
#endif
#pragma warning enable