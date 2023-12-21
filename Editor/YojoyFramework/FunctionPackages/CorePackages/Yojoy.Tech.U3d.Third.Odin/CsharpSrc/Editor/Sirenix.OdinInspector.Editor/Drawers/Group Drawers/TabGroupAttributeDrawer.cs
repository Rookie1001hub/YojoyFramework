#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="TabGroupAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System.Collections.Generic;
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws all properties grouped together with the <see cref="TabGroupAttribute"/>
    /// </summary>
    /// <seealso cref="TabGroupAttribute"/>
    public class TabGroupAttributeDrawer : OdinGroupDrawer<TabGroupAttribute>
    {
        private GUITabGroup tabGroup;
        private LocalPersistentContext<int> currentPage;
        private List<Tab> tabs;

        private class Tab
        {
            public string TabName;
            public List<InspectorProperty> InspectorProperties = new List<InspectorProperty>();
            public StringMemberHelper Title;
        }

        protected override void Initialize()
        {
            this.tabGroup = SirenixEditorGUI.CreateAnimatedTabGroup(this.Property);
            this.currentPage = this.GetPersistentValue<int>("CurrentPage", 0);
            this.tabs = new List<Tab>();
            var addLastTabs = new List<Tab>();

            for (int j = 0; j < this.Property.Children.Count; j++)
            {
                var child = this.Property.Children[j];
                var added = false;

                if (child.Info.PropertyType == PropertyType.Group)
                {
                    var attrType = child.GetAttribute<PropertyGroupAttribute>().GetType();

                    if (attrType.IsNested && attrType.DeclaringType == typeof(TabGroupAttribute))
                    {
                        // This is a tab subgroup; add all its children to a tab for that subgroup
                        var tab = new Tab();
                        tab.TabName = child.NiceName;
                        tab.Title = new StringMemberHelper(this.Property, child.Name.TrimStart('#'));
                        for (int i = 0; i < child.Children.Count; i++)
                        {
                            tab.InspectorProperties.Add(child.Children[i]);
                        }

                        this.tabs.Add(tab);
                        added = true;
                    }
                }

                if (!added)
                {
                    // This is a group member of the tab group itself, so it gets its own tab
                    var tab = new Tab();
                    tab.TabName = child.NiceName;
                    tab.Title = new StringMemberHelper(this.Property, child.Name.TrimStart('#'));
                    tab.InspectorProperties.Add(child);
                    addLastTabs.Add(tab);
                }
            }

            foreach (var tab in addLastTabs)
            {
                this.tabs.Add(tab);
            }

            for (int i = 0; i < this.tabs.Count; i++)
            {
                this.tabGroup.RegisterTab(this.tabs[i].TabName);
            }

            var currentTab = this.tabs[this.currentPage.Value];
            var selectedTabGroup = this.tabGroup.RegisterTab(currentTab.TabName);
            this.tabGroup.SetCurrentPage(selectedTabGroup);
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var property = this.Property;
            var attribute = this.Attribute;

            if (attribute.HideTabGroupIfTabGroupOnlyHasOneTab && this.tabs.Count <= 1)
            {
                for (int i = 0; i < this.tabs.Count; i++)
                {
                    int pageCount = this.tabs[i].InspectorProperties.Count;
                    for (int j = 0; j < pageCount; j++)
                    {
                        var child = this.tabs[i].InspectorProperties[j];
                        child.Update(); 
                        child.Draw(child.Label);
                    }
                }
                return;
            }

            this.tabGroup.AnimationSpeed = 1 / SirenixEditorGUI.TabPageSlideAnimationDuration;
            this.tabGroup.FixedHeight = attribute.UseFixedHeight;

            if (this.currentPage.Value >= this.tabs.Count || this.currentPage.Value < 0)
            {
                this.currentPage.Value = 0;
            }

            SirenixEditorGUI.BeginIndentedVertical(SirenixGUIStyles.PropertyPadding);
            tabGroup.BeginGroup(true, attribute.Paddingless ? GUIStyle.none : null);

            for (int i = 0; i < this.tabs.Count; i++)
            {
                var page = tabGroup.RegisterTab(this.tabs[i].TabName);
                page.Title = this.tabs[i].Title.GetString(property);
                if (page.BeginPage())
                {
                    this.currentPage.Value = i;
                    int pageCount = this.tabs[i].InspectorProperties.Count;
                    for (int j = 0; j < pageCount; j++)
                    {
                        var child = this.tabs[i].InspectorProperties[j];
                        child.Update(); // Since the property is not fetched through the property system, ensure it's updated before drawing it.
                        child.Draw(child.Label);
                    }
                }
                page.EndPage();
            }

            tabGroup.EndGroup();
            SirenixEditorGUI.EndIndentedVertical();
        }
    }
}
#endif
#pragma warning enable