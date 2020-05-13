#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="CustomContextMenuAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using System.Reflection;
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using Utilities;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Adds a generic menu option to properties marked with <see cref="CustomContextMenuAttribute"/>.
    /// </summary>
    /// <seealso cref="CustomContextMenuAttribute"/>
    /// <seealso cref="DisableContextMenuAttribute"/>
    /// <seealso cref="OnInspectorGUIAttribute"/>

    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public sealed class CustomContextMenuAttributeDrawer : OdinAttributeDrawer<CustomContextMenuAttribute>, IDefinesGenericMenuItems
    {
        private class ContextMenuInfo
        {
            public string ErrorMessage;
            public string Name;
            public Action<object> MethodCaller;
        }

        private ContextMenuInfo info;
        private PropertyContext<Dictionary<CustomContextMenuAttribute, ContextMenuInfo>> contextMenuInfos;
        private PropertyContext<bool> populated;

        /// <summary>
        /// Populates the generic menu for the property.
        /// </summary>
        public void PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            if (this.populated.Value)
            {
                // Another custom context menu drawer has already populated the menu with all custom menu items
                // this is done so we can have menu item ordering consistency.
                return;
            }
            else
            {
                this.populated.Value = true;
            }
            
            if (this.contextMenuInfos.Value != null && this.contextMenuInfos.Value.Count > 0)
            {
                if (genericMenu.GetItemCount() > 0)
                {
                    genericMenu.AddSeparator("");
                }

                foreach (var item in this.contextMenuInfos.Value.OrderBy(n => n.Key.MenuItem ?? ""))
                {
                    var info = item.Value;

                    if (info.MethodCaller == null)
                    {
                        genericMenu.AddDisabledItem(new GUIContent(item.Key.MenuItem + " (Invalid)"));
                    }
                    else
                    {
                        genericMenu.AddItem(new GUIContent(info.Name), false, () =>
                        {
                            for (int i = 0; i < property.ParentValues.Count; i++)
                            {
                                info.MethodCaller(property.ParentValues[i]);
                            }
                        });
                    }
                }
            }
        }

        protected override void Initialize()
        {
            var property = this.Property;
            var attribute = this.Attribute;

            this.contextMenuInfos = property.Context.GetGlobal("CustomContextMenu", (Dictionary<CustomContextMenuAttribute, ContextMenuInfo>)null);
            this.populated = property.Context.GetGlobal("CustomContextMenu_Populated", false);

            if (contextMenuInfos.Value == null)
            {
                contextMenuInfos.Value = new Dictionary<CustomContextMenuAttribute, ContextMenuInfo>();
            }
            
            if (!contextMenuInfos.Value.TryGetValue(attribute, out this.info))
            {
                this.info = new ContextMenuInfo();

                var methodInfo = property.ParentType
                    .FindMember()
                    .IsMethod()
                    .IsInstance()
                    .HasNoParameters()
                    .ReturnsVoid()
                    .IsNamed(attribute.MethodName)
                    .GetMember<MethodInfo>(out this.info.ErrorMessage);

                if (this.info.ErrorMessage == null)
                {
                    this.info.Name = attribute.MenuItem;
                    this.info.MethodCaller = EmitUtilities.CreateWeakInstanceMethodCaller(methodInfo);
                }

                contextMenuInfos.Value[attribute] = this.info;
            }

        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            this.populated.Value = false;

            if (this.info.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(info.ErrorMessage);
            }

            this.CallNextDrawer(label);
        }
    }
}
#endif
#pragma warning enable