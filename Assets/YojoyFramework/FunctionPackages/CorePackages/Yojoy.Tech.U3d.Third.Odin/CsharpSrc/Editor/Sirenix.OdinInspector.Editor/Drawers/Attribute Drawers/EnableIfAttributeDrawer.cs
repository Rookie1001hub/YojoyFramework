#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="EnableIfAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="EnableIfAttribute"/>.
    /// </summary>
    /// <seealso cref="EnableIfAttribute"/>
    /// <seealso cref="DisableIfAttribute"/>
    /// <seealso cref="DisableInEditorModeAttribute"/>
    /// <seealso cref="DisableInPlayModeAttribute"/>
    /// <seealso cref="ReadOnlyAttribute"/>
    /// <seealso cref="ShowIfAttribute"/>
    /// <seealso cref="HideIfAttribute"/>
    /// <seealso cref="HideInInspector"/>
    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public sealed class EnableIfAttributeDrawer : OdinAttributeDrawer<EnableIfAttribute>
    {
        private IfAttributeHelper helper;

        /// <summary>
        /// Initializes the drawer.
        /// </summary>
        protected override void Initialize()
        {
            this.helper = new IfAttributeHelper(this.Property, this.Attribute.MemberName);
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUI.enabled == false)
            {
                this.CallNextDrawer(label);
                return;
            }
            
            if (this.helper.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.helper.ErrorMessage);
                this.CallNextDrawer(label);
            }
            else if (this.helper.GetValue(this.Attribute.Value) == false)
            {
                GUIHelper.PushGUIEnabled(false);
                this.CallNextDrawer(label);
                GUIHelper.PopGUIEnabled();
            }
            else
            {
                this.CallNextDrawer(label);
            }
        }
    }
}
#endif
#pragma warning enable