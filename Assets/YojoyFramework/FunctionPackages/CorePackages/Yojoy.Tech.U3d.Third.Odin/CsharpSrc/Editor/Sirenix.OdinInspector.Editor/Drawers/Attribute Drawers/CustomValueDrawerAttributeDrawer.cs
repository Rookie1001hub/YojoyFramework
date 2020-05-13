#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="CustomValueDrawerAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using System.Reflection;
    using Utilities.Editor;
    using UnityEngine;
    using Utilities;
    using System.Collections;
    using System.Reflection.Emit;

    /// <summary>
    /// Draws properties marked with <see cref="ValidateInputAttribute"/>.
    /// </summary>
    /// <seealso cref="ValidateInputAttribute"/>

    [DrawerPriority(0, 0, double.MaxValue)]
    public class CustomValueDrawerAttributeDrawer<T> : OdinAttributeDrawer<CustomValueDrawerAttribute, T>
    {
        private string errorMessage;
        private IMethodInvokerWrapperThing invoker;
        private InspectorProperty memberProperty;

        /// <summary>
        /// Excludes functionality for lists and instead works on the list elements.
        /// </summary>
        public override bool CanDrawTypeFilter(Type type)
        {
            return !typeof(IList).IsAssignableFrom(type);
        }

        protected override void Initialize()
        {
            var actualValueType = this.ValueEntry.BaseValueType;
            this.memberProperty = this.Property.FindParent(p => p.Info.HasSingleBackingMember, includeSelf: true);
            
            var parentType = this.memberProperty.ParentType;
            var methodInfo = parentType
                .FindMember()
                .IsNamed(this.Attribute.MethodName)
                .IsMethod()
                .HasReturnType(actualValueType, inherit: false)
                .HasParameters(actualValueType, typeof(GUIContent))
                .GetMember<MethodInfo>(out this.errorMessage);
            
            if (methodInfo != null)
            {
                var valueParameter = methodInfo.GetParameters()[0];

                if (valueParameter.ParameterType != methodInfo.ReturnType)
                {
                    this.errorMessage = "Parameter '" + valueParameter.Name + "' must be of the exact same type as the method's return type.";
                }
            }

            if (this.errorMessage == null)
            {
                var invokerType = typeof(MethodInvokerWrapperThing<>).GetGenericTypeDefinition().MakeGenericType(typeof(T), actualValueType);
                this.invoker = (IMethodInvokerWrapperThing)Activator.CreateInstance(invokerType, methodInfo);
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
                this.CallNextDrawer(label);
            }
            else
            {
                object parent = this.memberProperty.ParentValues[0];
                this.ValueEntry.WeakSmartValue = this.invoker.Invoke(ref parent, this.ValueEntry.WeakSmartValue, label);
            }
        }

        private interface IMethodInvokerWrapperThing
        {
            object Invoke(ref object parent, object value, GUIContent label);
        }

        private class MethodInvokerWrapperThing<TBaseValue> : IMethodInvokerWrapperThing
        {
            private delegate TBaseValue InstanceDelegateWithLabel(ref object owner, TBaseValue value, GUIContent label);

            private Func<TBaseValue, GUIContent, TBaseValue> CustomValueDrawerStaticWithLabel;
            private InstanceDelegateWithLabel CustomValueDrawerInstanceWithLabel;

            public MethodInvokerWrapperThing(MethodInfo methodInfo)
            {
                var parentType = methodInfo.ReflectedType;

                if (methodInfo.IsStatic())
                {
                    this.CustomValueDrawerStaticWithLabel = (Func<TBaseValue, GUIContent, TBaseValue>)Delegate.CreateDelegate(typeof(Func<TBaseValue, GUIContent, TBaseValue>), methodInfo);
                }
                else
                {
                    DynamicMethod emittedMethod;

                    emittedMethod = new DynamicMethod("CustomValueDrawerAttributeDrawer." + typeof(T).GetNiceFullName() + "." + Guid.NewGuid().ToString(), typeof(TBaseValue), new Type[] { typeof(object).MakeByRefType(), typeof(TBaseValue), typeof(GUIContent) }, true);

                    var il = emittedMethod.GetILGenerator();

                    if (parentType.IsValueType)
                    {
                        il.DeclareLocal(typeof(TBaseValue));

                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldind_Ref);
                        il.Emit(OpCodes.Unbox_Any, parentType);
                        il.Emit(OpCodes.Stloc_0);
                        il.Emit(OpCodes.Ldloca_S, 0);
                        il.Emit(OpCodes.Ldarg_1);

                        il.Emit(OpCodes.Ldarg_2);

                        il.Emit(OpCodes.Call, methodInfo);
                        il.Emit(OpCodes.Stloc_1);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Box, parentType);
                        il.Emit(OpCodes.Stind_Ref);
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ret);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldind_Ref);
                        il.Emit(OpCodes.Castclass, parentType);
                        il.Emit(OpCodes.Ldarg_1);

                        il.Emit(OpCodes.Ldarg_2);

                        il.Emit(OpCodes.Callvirt, methodInfo);
                        il.Emit(OpCodes.Ret);
                    }
                    this.CustomValueDrawerInstanceWithLabel = (InstanceDelegateWithLabel)emittedMethod.CreateDelegate(typeof(InstanceDelegateWithLabel));
                }
            }

            public object Invoke(ref object parent, object value, GUIContent label)
            {
                if (this.CustomValueDrawerStaticWithLabel != null)
                {
                    return this.CustomValueDrawerStaticWithLabel((TBaseValue)value, label);
                }
                else
                {
                    return this.CustomValueDrawerInstanceWithLabel(ref parent, (TBaseValue)value, label);
                }
            }

        }
    }
}
#endif
#pragma warning enable