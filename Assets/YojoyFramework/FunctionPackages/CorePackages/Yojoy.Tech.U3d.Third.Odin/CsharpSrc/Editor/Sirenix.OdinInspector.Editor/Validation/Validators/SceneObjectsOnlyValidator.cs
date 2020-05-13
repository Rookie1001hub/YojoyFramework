#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="SceneObjectsOnlyValidator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.SceneObjectsOnlyValidator<>))]

namespace Sirenix.OdinInspector.Editor.Validation
{
    using System.Reflection;
    using Sirenix.OdinInspector;
    using Sirenix.Utilities;
    using UnityEditor;
    using UnityEngine;

    public class SceneObjectsOnlyValidator<T> : AttributeValidator<SceneObjectsOnlyAttribute, T>
        where T : UnityEngine.Object
    {
        protected override void Validate(object parentInstance, T memberValue, MemberInfo member, ValidationResult result)
        {
            if (memberValue != null && AssetDatabase.Contains(memberValue))
            {
                string name = memberValue.name;
                var component = memberValue as Component;
                if (component != null)
                {
                    name = "from " + component.gameObject.name;
                }

                result.ResultType = ValidationResultType.Error;
                result.Message = (memberValue as object).GetType().GetNiceName() + " " + name + " cannot be an asset.";
            }
        }
    }
    
}
#endif
#pragma warning enable