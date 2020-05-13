#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ValidationDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Validation
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    [DrawerPriority(0, 10000, 0)]
    public class ValidationDrawer<T> : OdinValueDrawer<T>, IDisposable
    {
        public static readonly ValidationRunner ValidationRunner = new ValidationRunner()
        {
            ValidatorLocator = new DefaultValidatorLocator()
            {
                CustomValidatorFilter = (type) =>
                {
                    if (type.IsDefined<NoValidationInInspectorAttribute>(true))
                        return false;

                    return true;
                }
            }
        };

        private List<ValidationResult> validationResults;
        private bool rerunFullValidation;
        private object shakeGroupKey;

        private static MemberInfo GetMember(InspectorProperty property)
        {
            while (property.Parent != null && property.Parent.ChildResolver is ICollectionResolver)
                property = property.Parent;

            return property.Info.GetMemberInfo();
        }

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            var member = GetMember(property);
            bool isCollectionElement = property.Parent != null && property.Parent.ChildResolver is ICollectionResolver;

            if (member != null && ValidationRunner.ValidatorLocator.PotentiallyHasValidatorsFor(member, typeof(T), isCollectionElement))
            {
                return true;
            }

            if (ValidationRunner.ValidatorLocator.PotentiallyHasValidatorsFor(typeof(T)))
            {
                return true;
            }

            return false;
        }

        protected override void Initialize()
        {
            var member = GetMember(this.Property);
            bool isCollectionElement = this.Property.Parent != null && this.Property.Parent.ChildResolver is ICollectionResolver;

            if (member != null && ValidationRunner.ValidatorLocator.PotentiallyHasValidatorsFor(member, typeof(T), isCollectionElement))
            {
                ValidationRunner.ValidateMember(this.Property.ParentValues[0], member, this.ValueEntry.Values[0], typeof(T), this.Property.Tree.WeakTargets[0] as UnityEngine.Object, isCollectionElement, ref this.validationResults);
            }
            else 
            {
                ValidationRunner.ValidateValue(this.ValueEntry.Values[0], this.Property.Tree.WeakTargets[0] as UnityEngine.Object, ref this.validationResults);
            }

            if (this.validationResults.Count > 0)
            {
                this.shakeGroupKey = UniqueDrawerKey.Create(this.Property, this);

                this.Property.Tree.OnUndoRedoPerformed += this.OnUndoRedoPerformed;
                this.ValueEntry.OnValueChanged += this.OnValueChanged;
                this.ValueEntry.OnChildValueChanged += this.OnChildValueChanged;
            }
            else
            {
                this.SkipWhenDrawing = true;
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (this.validationResults.Count == 0)
            {
                this.CallNextDrawer(label);
                return;
            }

            GUILayout.BeginVertical();
            SirenixEditorGUI.BeginShakeableGroup(this.shakeGroupKey);

            for (int i = 0; i < this.validationResults.Count; i++)
            {
                var result = this.validationResults[i];

                if (Event.current.type == EventType.Layout && (this.rerunFullValidation || result.Setup.Validator.RevalidationCriteria == RevalidationCriteria.Always))
                {
                    var formerResultType = result.ResultType;

                    result.Setup.ParentInstance = this.Property.ParentValues[0];
                    result.Setup.Value = this.ValueEntry.Values[0];

                    result.RerunValidation();

                    if (formerResultType != result.ResultType && result.ResultType != ValidationResultType.Valid)
                    {
                        // We got a new result that was not valid
                        SirenixEditorGUI.StartShakingGroup(this.shakeGroupKey);
                    }
                }

                if (result.ResultType == ValidationResultType.Error)
                {
                    SirenixEditorGUI.ErrorMessageBox(result.Message);
                }
                else if (result.ResultType == ValidationResultType.Warning)
                {
                    SirenixEditorGUI.WarningMessageBox(result.Message);
                }
            }

            if (Event.current.type == EventType.Layout)
            {
                this.rerunFullValidation = false;
            }

            this.CallNextDrawer(label);
            SirenixEditorGUI.EndShakeableGroup(this.shakeGroupKey);
            GUILayout.EndVertical();
        }

        public void Dispose()
        {
            if (this.validationResults.Count > 0)
            {
                this.Property.Tree.OnUndoRedoPerformed -= this.OnUndoRedoPerformed;
                this.ValueEntry.OnValueChanged -= this.OnValueChanged;
                this.ValueEntry.OnChildValueChanged -= this.OnChildValueChanged;
            }
        }

        private void OnUndoRedoPerformed()
        {
            this.rerunFullValidation = true;
        }

        private void OnValueChanged(int index)
        {
            this.rerunFullValidation = true;
        }

        private void OnChildValueChanged(int index)
        {
            this.rerunFullValidation = true;
        }
    }
}
#endif
#pragma warning enable