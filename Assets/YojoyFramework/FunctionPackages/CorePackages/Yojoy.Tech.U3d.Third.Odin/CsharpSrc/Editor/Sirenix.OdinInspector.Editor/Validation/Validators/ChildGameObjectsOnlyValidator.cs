#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ChildGameObjectsOnlyValidator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.ChildGameObjectsOnlyValidator<>))]

namespace Sirenix.OdinInspector.Editor.Validation
{
    using System.Reflection;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [NoValidationInInspector]
    public class ChildGameObjectsOnlyValidator<T> : AttributeValidator<ChildGameObjectsOnlyAttribute, T>
        where T : UnityEngine.Object
    {
        protected override void Validate(object parentInstance, T memberValue, MemberInfo member, ValidationResult result)
        {
            GameObject go = result.Setup.Root as GameObject;

            GameObject valueGo = memberValue as GameObject;

            if (valueGo == null)
            {
                Component component = memberValue as Component;

                if (component != null)
                {
                    valueGo = component.gameObject;
                }
            }

            // Attribute doesn't apply in this context, as we're not on a GameObject
            // or are not dealing with the right kind of value
            if (go == null || valueGo == null)
            {
                result.ResultType = ValidationResultType.IgnoreResult;
                return;
            }

            if (this.Attribute.IncludeSelf && go == valueGo)
            {
                result.ResultType = ValidationResultType.Valid;
                return;
            }
            
            Transform current = valueGo.transform;

            while (true)
            {
                current = current.parent;

                if (current == null)
                    break;

                if (current.gameObject == go)
                {
                    result.ResultType = ValidationResultType.Valid;
                    return;
                }
            }

            result.ResultType = ValidationResultType.Error;
            result.Message = valueGo.name + " must be a child of " + go.name;
        }
    }
    
}
#endif
#pragma warning enable