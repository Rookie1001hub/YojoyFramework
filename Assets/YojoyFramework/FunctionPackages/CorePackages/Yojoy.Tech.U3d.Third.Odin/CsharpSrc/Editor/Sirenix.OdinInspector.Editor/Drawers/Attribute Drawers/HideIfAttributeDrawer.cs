#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="HideIfAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="ShowIfAttribute"/>.
    /// </summary>
    [DrawerPriority(100, 0, 0)]
    public sealed class HideIfAttributeDrawer : OdinAttributeDrawer<HideIfAttribute>
    {
        private IfAttributeHelper helper;

        protected override void Initialize()
        {
            this.helper = new IfAttributeHelper(this.Property, this.Attribute.MemberName);
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (this.helper.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.helper.ErrorMessage);
                this.CallNextDrawer(label);
            }
            else
            {
                bool result = this.helper.GetValue(this.Attribute.Value);

                if (this.Attribute.Animate)
                {
                    if (SirenixEditorGUI.BeginFadeGroup(UniqueDrawerKey.Create(this.Property, this), !result))
                    {
                        this.CallNextDrawer(label);
                    }
                    SirenixEditorGUI.EndFadeGroup();
                }
                else
                {
                    if (!result)
                    {
                        this.CallNextDrawer(label);
                    }
                }
            }
        }
    }
}
#endif
#pragma warning enable