#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="OnValueChangedAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System.Reflection;
    using Utilities.Editor;
    using UnityEngine;
    using Utilities;
    using System;

    /// <summary>
    /// Draws properties marked with <see cref="OnValueChangedAttribute"/>.
    /// </summary>
    /// <seealso cref="OnValueChangedAttribute"/>
    /// <seealso cref="OnInspectorGUIAttribute"/>
    /// <seealso cref="ValidateInputAttribute"/>
    /// <seealso cref="InfoBoxAttribute"/>

    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public sealed class OnValueChangedAttributeDrawer<T> : OdinAttributeDrawer<OnValueChangedAttribute, T>
    {
        private MethodInfo methodInfo;      
        private string errorMessage;

        protected override void Initialize()
        {
            this.methodInfo =
                this.Property.ParentType.FindMember().IsMethod().IsNamed(this.Attribute.MethodName).HasParameters<T>().GetMember<MethodInfo>(out this.errorMessage)
                ?? this.Property.ParentType.FindMember().IsMethod().IsNamed(this.Attribute.MethodName).HasNoParameters().GetMember<MethodInfo>(out this.errorMessage);

            if (this.methodInfo != null)
            {
                var parameters = this.methodInfo.GetParameters();

                Action<int> action;

                if (this.methodInfo.IsStatic)
                {
                    if (parameters.Length == 0)
                    {
                        action = (int index) =>
                        {
                            this.methodInfo.Invoke(null, null);
                        };
                    }
                    else
                    {
                        action = (int index) =>
                        {
                            this.methodInfo.Invoke(null, new object[] { this.ValueEntry.WeakValues[index] });
                        };
                    }
                }
                else
                {
                    if (parameters.Length == 0)
                    {
                        action = (int index) =>
                        {
                            object inst = this.ValueEntry.Property.ParentValues[index];
                            this.methodInfo.Invoke(inst, null);

                            if (this.ValueEntry.ParentType.IsValueType && this.Property.ParentValueProperty != null)
                            {
                                this.Property.ParentValueProperty.ValueEntry.WeakValues[index] = inst;
                                GUIHelper.RequestRepaint();
                            }
                        };
                    }
                    else
                    {
                        action = (int index) =>
                        {
                            object inst = this.ValueEntry.Property.ParentValues[index];
                            this.methodInfo.Invoke(this.ValueEntry.Property.ParentValues[index], new object[] { this.ValueEntry.WeakValues[index] });

                            if (this.ValueEntry.ParentType.IsValueType && this.Property.ParentValueProperty != null)
                            {
                                this.Property.ParentValueProperty.ValueEntry.WeakValues[index] = inst;
                                GUIHelper.RequestRepaint();
                            }
                        };
                    }
                }

                this.ValueEntry.OnValueChanged += action;

                if (this.Attribute.IncludeChildren || typeof(T).IsValueType)
                {
                    this.ValueEntry.OnChildValueChanged += action;
                }
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (this.errorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.errorMessage);
            }

            this.CallNextDrawer(label);
        }
    }
}
#endif
#pragma warning enable