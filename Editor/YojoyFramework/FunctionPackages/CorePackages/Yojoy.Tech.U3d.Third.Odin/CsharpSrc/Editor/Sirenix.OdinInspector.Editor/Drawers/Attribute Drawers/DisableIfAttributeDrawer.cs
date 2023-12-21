#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="DisableIfAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="DisableIfAttribute"/>.
    /// </summary>
    /// <seealso cref="DisableIfAttribute"/>
    /// <seealso cref="EnableIfAttribute"/>
    /// <seealso cref="DisableInEditorModeAttribute"/>
    /// <seealso cref="DisableInPlayModeAttribute"/>
    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public class DisableIfAttributeDrawer : OdinAttributeDrawer<DisableIfAttribute>
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
            else if (this.helper.GetValue(this.Attribute.Value))
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