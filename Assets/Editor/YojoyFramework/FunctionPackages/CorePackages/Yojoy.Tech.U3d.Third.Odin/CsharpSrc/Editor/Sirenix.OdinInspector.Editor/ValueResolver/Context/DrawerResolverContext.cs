#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="DrawerResolverContext.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    using System;

#if SIRENIX_INTERNAL
    public class DrawerResolverContext<TDrawer> : ResolverContext
#else
    internal class DrawerResolverContext<TDrawer> : ResolverContext
#endif
        where TDrawer: OdinDrawer
    {
        public readonly TDrawer Drawer;

        public DrawerResolverContext(TDrawer drawer)
        {
            this.Drawer = drawer;
            this.AddExpressionParameter<InspectorProperty>("property", () => this.Drawer.Property);

            if (this.Drawer.Property.ValueEntry != null)
            {
                this.AddExpressionParameter("value", this.Drawer.Property.ValueEntry.BaseValueType, () => this.Drawer.Property.ValueEntry.WeakSmartValue);
            }
        }

        public override object GetParentInstance()
        {
            return this.Drawer.Property.ParentValues[0];
        }

        public override Type GetParentType()
        {
            return this.Drawer.Property.ParentType;
        }
    }
}
#endif
#pragma warning enable