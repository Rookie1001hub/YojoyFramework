#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ShowIfGroupAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;

    public class ShowIfGroupAttributeDrawer : OdinGroupDrawer<ShowIfGroupAttribute>
    {
        private IfAttributeHelper helper;

        protected override void Initialize()
        {
            this.helper = new IfAttributeHelper(this.Property, this.Attribute.MemberName);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            bool show = true;

            if (this.helper.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.helper.ErrorMessage);
            }
            else
            {
                show = this.helper.GetValue(this.Attribute.Value);
            }

            if (this.Attribute.Animate)
            {
                if (SirenixEditorGUI.BeginFadeGroup(this, show))
                {
                    for (int i = 0; i < this.Property.Children.Count; i++)
                    {
                        this.Property.Children[i].Draw();
                    }
                }
                SirenixEditorGUI.EndFadeGroup();
            }
            else if (show)
            {
                for (int i = 0; i < this.Property.Children.Count; i++)
                {
                    this.Property.Children[i].Draw();
                }
            }
        }
    }
}
#endif
#pragma warning enable