#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="IfAttributeHelper.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using System.Reflection;
    using UnityEngine;
    using Utilities;
    using Sirenix.Utilities.Editor.Expressions;

    public class IfAttributeHelper
    {
        private readonly InspectorProperty property;
        //private readonly string memberName;

        private readonly Func<bool> staticMemberGetter;
        private readonly Func<object, bool> instanceMemberGetter;
        private readonly Func<object> staticObjectMemberGetter;
        private readonly Func<object, object> instanceObjectMemberGetter;

#if !ODIN_LIMITED_VERSION
        private readonly Delegate expressionMethod;
        private readonly bool expressionIsStatic;
#endif

        private readonly bool useNullComparison;
        private bool result;

        public string ErrorMessage { get; private set; }

        public IfAttributeHelper(InspectorProperty property, string memberName)
        {
            this.property = property;
            //this.memberName = memberName;

            string error;
            Type returnType;

            if (memberName != null && memberName.Length > 0 && memberName[0] == '@')
            {
#if !ODIN_LIMITED_VERSION
                var expression = memberName.Substring(1);

                Type[] parameters = new Type[] { typeof(InspectorProperty) };
                string[] parameterNames = new string[] { "property" };

                this.expressionIsStatic = this.property.ParentValueProperty == null && this.property.Tree.IsStatic;
                this.expressionMethod = ExpressionUtility.ParseExpression(expression, this.expressionIsStatic, this.property.ParentType, parameters, parameterNames, out error);

                returnType = this.expressionMethod != null ? this.expressionMethod.Method.ReturnType : null;
#else
                    returnType = null;
                    error = "Expressions are only available in Odin Inspector Commercial and up.";
#endif
            }
            else
            {
                if (memberName != null && memberName.Length > 0 && memberName[0] == '$')
                {
                    memberName = memberName.Substring(1);
                }

                returnType = null;
                MemberInfo memberInfo = this.property.ParentType
                    .FindMember()
                    .IsNamed(memberName)
                    .HasNoParameters()
                    .GetMember(out error);

                if (memberInfo != null)
                {
                    string name = (memberInfo is MethodInfo) ? memberInfo.Name + "()" : memberInfo.Name;

                    if (memberInfo.GetReturnType() == typeof(bool))
                    {
                        if (memberInfo.IsStatic())
                        {
                            this.staticMemberGetter = DeepReflection.CreateValueGetter<bool>(this.property.ParentType, name);
                        }
                        else
                        {
                            this.instanceMemberGetter = DeepReflection.CreateWeakInstanceValueGetter<bool>(this.property.ParentType, name);
                        }
                    }
                    else
                    {
                        if (memberInfo.IsStatic())
                        {
                            this.staticObjectMemberGetter = DeepReflection.CreateValueGetter<object>(this.property.ParentType, name);
                        }
                        else
                        {
                            this.instanceObjectMemberGetter = DeepReflection.CreateWeakInstanceValueGetter<object>(this.property.ParentType, name);
                        }
                    }

                    returnType = memberInfo.GetReturnType();
                }
            }

            if (returnType != null) // Should only be null in case of errors.
            {
                this.useNullComparison = returnType != typeof(string) && (returnType.IsClass || returnType.IsInterface);
            }

            this.ErrorMessage = error;
        }

        public bool GetValue(object value)
        {
            if (Event.current.type == EventType.Layout && this.ErrorMessage == null)
            {
                this.result = false;

#if !ODIN_LIMITED_VERSION
                if (this.expressionMethod != null)
                {
                    for (int i = 0; i < this.property.ParentValues.Count; i++)
                    {
                        object val;
                        if (this.expressionIsStatic)
                        {
                            val = this.expressionMethod.DynamicInvoke(this.property);
                        }
                        else
                        {
                            val = this.expressionMethod.DynamicInvoke(this.property.ParentValues[i], this.property);
                        }

                        if (this.useNullComparison)
                        {
                            if (val is UnityEngine.Object)
                            {
                                // Unity objects can be 'fake null', and to detect that we have to test the value as a Unity object.
                                if (((UnityEngine.Object)val) != null)
                                {
                                    this.result = true;
                                    break;
                                }
                            }
                            else if (val != null)
                            {
                                this.result = true;
                                break;
                            }
                        }
                        else if (val is bool)
                        {
                            this.result = (bool)val;
                            break;
                        }
                        else if (Equals(val, value))
                        {
                            this.result = true;
                            break;
                        }
                    }
                }
                else if (this.instanceMemberGetter != null)
#else
                if (this.instanceMemberGetter != null)
#endif
                {
                    for (int i = 0; i < this.property.ParentValues.Count; i++)
                    {
                        if (this.instanceMemberGetter(this.property.ParentValues[i]))
                        {
                            this.result = true;
                            break;
                        }
                    }
                }
                else if (this.staticMemberGetter != null)
                {
                    if (this.staticMemberGetter())
                    {
                        this.result = true;
                    }
                }
                else if (this.instanceObjectMemberGetter != null)
                {
                    for (int i = 0; i < this.property.ParentValues.Count; i++)
                    {
                        var val = this.instanceObjectMemberGetter(property.ParentValues[i]);
                        if (this.useNullComparison)
                        {
                            if (val is UnityEngine.Object)
                            {
                                // Unity objects can be 'fake null', and to detect that we have to test the value as a Unity object.
                                if (((UnityEngine.Object)val) != null)
                                {
                                    this.result = true;
                                    break;
                                }
                            }
                            else if (val != null)
                            {
                                this.result = true;
                                break;
                            }
                        }
                        else if (Equals(val, value))
                        {
                            this.result = true;
                            break;
                        }
                    }
                }
                else if (this.staticObjectMemberGetter != null)
                {
                    var val = this.staticObjectMemberGetter();
                    if (this.useNullComparison)
                    {
                        if (val is UnityEngine.Object)
                        {
                            // Unity objects can be 'fake null', and to detect that we have to test the value as a Unity object.
                            if (((UnityEngine.Object)val) != null)
                            {
                                this.result = true;
                            }
                        }
                        else if (val != null)
                        {
                            this.result = true;
                        }
                    }
                    else if (Equals(val, value))
                    {
                        this.result = true;
                    }
                }
            }

            return this.result;
        }
    }

    //internal static class IfAttributesHelper
    //{
    //    //        private class IfAttributesContext
    //    //        {
    //    //            public Func<bool> StaticMemberGetter;
    //    //            public Func<object, bool> InstanceMemberGetter;
    //    //            public Func<object> StaticObjectMemberGetter;
    //    //            public Func<object, object> InstanceObjectMemberGetter;

    //    //#if !ODIN_LIMITED_VERSION
    //    //            public Delegate ExpressionMethod;
    //    //            public bool expressionIsStatic;
    //    //#endif

    //    //            public string ErrorMessage;
    //    //            public bool Result;
    //    //            public bool UseNullComparison;
    //    //        }

    //    //        public static void HandleIfAttributesCondition(OdinDrawer drawer, InspectorProperty property, string memberName, object value, out bool result, out string errorMessage)
    //    //        {
    //    //            var context = property.Context.Get(drawer, "IfAttributeContext", (IfAttributesContext)null);

    //    //            if (context.Value == null)
    //    //            {
    //    //                Type returnType;

    //    //                context.Value = new IfAttributesContext();

    //    //                if (memberName != null && memberName.Length > 0 && memberName[0] == '$')
    //    //                {
    //    //#if !ODIN_LIMITED_VERSION
    //    //                    var expression = memberName.Substring(1);

    //    //                    context.Value.expressionIsStatic = property.ParentValueProperty == null && property.Tree.IsStatic;
    //    //                    context.Value.ExpressionMethod = ExpressionUtility.ParseExpression(expression, context.Value.expressionIsStatic, property.ParentType, out context.Value.ErrorMessage);

    //    //                    returnType = context.Value.ExpressionMethod != null ? context.Value.ExpressionMethod.Method.ReturnType : null;
    //    //#else
    //    //                    returnType = null;
    //    //                    context.Value.ErrorMessage = "Expressions are not available in Odin Inspector Non-Commercial.";
    //    //#endif
    //    //                }
    //    //                else
    //    //                {
    //    //                    returnType = null;
    //    //                    MemberInfo memberInfo = property.ParentType
    //    //                        .FindMember()
    //    //                        .IsNamed(memberName)
    //    //                        .HasNoParameters()
    //    //                        .GetMember(out context.Value.ErrorMessage);

    //    //                    if (memberInfo != null)
    //    //                    {
    //    //                        string name = (memberInfo is MethodInfo) ? memberInfo.Name + "()" : memberInfo.Name;

    //    //                        if (memberInfo.GetReturnType() == typeof(bool))
    //    //                        {
    //    //                            if (memberInfo.IsStatic())
    //    //                            {
    //    //                                context.Value.StaticMemberGetter = DeepReflection.CreateValueGetter<bool>(property.ParentType, name);
    //    //                            }
    //    //                            else
    //    //                            {
    //    //                                context.Value.InstanceMemberGetter = DeepReflection.CreateWeakInstanceValueGetter<bool>(property.ParentType, name);
    //    //                            }
    //    //                        }
    //    //                        else
    //    //                        {
    //    //                            if (memberInfo.IsStatic())
    //    //                            {
    //    //                                context.Value.StaticObjectMemberGetter = DeepReflection.CreateValueGetter<object>(property.ParentType, name);
    //    //                            }
    //    //                            else
    //    //                            {
    //    //                                context.Value.InstanceObjectMemberGetter = DeepReflection.CreateWeakInstanceValueGetter<object>(property.ParentType, name);
    //    //                            }
    //    //                        }

    //    //                        returnType = memberInfo.GetReturnType();
    //    //                    }
    //    //                }

    //    //                if (returnType != null) // Should only be null in case of errors.
    //    //                {
    //    //                    context.Value.UseNullComparison = returnType != typeof(string) && (returnType.IsClass || returnType.IsInterface);
    //    //                }
    //    //            }
    //    //            errorMessage = context.Value.ErrorMessage;

    //    //            if (Event.current.type != EventType.Layout)
    //    //            {
    //    //                result = context.Value.Result;
    //    //                return;
    //    //            }

    //    //            context.Value.Result = false;

    //    //            if (context.Value.ErrorMessage == null)
    //    //            {
    //    //#if !ODIN_LIMITED_VERSION
    //    //                if (context.Value.ExpressionMethod != null)
    //    //                {
    //    //                    for (int i = 0; i < property.ParentValues.Count; i++)
    //    //                    {
    //    //                        object val;
    //    //                        if (context.Value.expressionIsStatic)
    //    //                        {
    //    //                            val = context.Value.ExpressionMethod.DynamicInvoke();
    //    //                        }
    //    //                        else
    //    //                        {
    //    //                            val = context.Value.ExpressionMethod.DynamicInvoke(property.ParentValues[i]);
    //    //                        }

    //    //                        if (context.Value.UseNullComparison)
    //    //                        {
    //    //                            if (val is UnityEngine.Object)
    //    //                            {
    //    //                                // Unity objects can be 'fake null', and to detect that we have to test the value as a Unity object.
    //    //                                if (((UnityEngine.Object)val) != null)
    //    //                                {
    //    //                                    context.Value.Result = true;
    //    //                                    break;
    //    //                                }
    //    //                            }
    //    //                            else if (val != null)
    //    //                            {
    //    //                                context.Value.Result = true;
    //    //                                break;
    //    //                            }
    //    //                        }
    //    //                        else if (val is bool)
    //    //                        {
    //    //                            context.Value.Result = (bool)val;
    //    //                            break;
    //    //                        }
    //    //                        else if (Equals(val, value))
    //    //                        {
    //    //                            context.Value.Result = true;
    //    //                            break;
    //    //                        }
    //    //                    }
    //    //                }
    //    //                else if (context.Value.InstanceMemberGetter != null)
    //    //#else
    //    //                if (context.Value.InstanceMemberGetter != null)
    //    //#endif
    //    //                {
    //    //                    for (int i = 0; i < property.ParentValues.Count; i++)
    //    //                    {
    //    //                        if (context.Value.InstanceMemberGetter(property.ParentValues[i]))
    //    //                        {
    //    //                            context.Value.Result = true;
    //    //                            break;
    //    //                        }
    //    //                    }
    //    //                }
    //    //                else if (context.Value.StaticMemberGetter != null)
    //    //                {
    //    //                    if (context.Value.StaticMemberGetter())
    //    //                    {
    //    //                        context.Value.Result = true;
    //    //                    }
    //    //                }
    //    //                else if (context.Value.InstanceObjectMemberGetter != null)
    //    //                {
    //    //                    for (int i = 0; i < property.ParentValues.Count; i++)
    //    //                    {
    //    //                        var val = context.Value.InstanceObjectMemberGetter(property.ParentValues[i]);
    //    //                        if (context.Value.UseNullComparison)
    //    //                        {
    //    //                            if (val is UnityEngine.Object)
    //    //                            {
    //    //                                // Unity objects can be 'fake null', and to detect that we have to test the value as a Unity object.
    //    //                                if (((UnityEngine.Object)val) != null)
    //    //                                {
    //    //                                    context.Value.Result = true;
    //    //                                    break;
    //    //                                }
    //    //                            }
    //    //                            else if (val != null)
    //    //                            {
    //    //                                context.Value.Result = true;
    //    //                                break;
    //    //                            }
    //    //                        }
    //    //                        else if (Equals(val, value))
    //    //                        {
    //    //                            context.Value.Result = true;
    //    //                            break;
    //    //                        }
    //    //                    }
    //    //                }
    //    //                else if (context.Value.StaticObjectMemberGetter != null)
    //    //                {
    //    //                    var val = context.Value.StaticObjectMemberGetter();
    //    //                    if (context.Value.UseNullComparison)
    //    //                    {
    //    //                        if (val is UnityEngine.Object)
    //    //                        {
    //    //                            // Unity objects can be 'fake null', and to detect that we have to test the value as a Unity object.
    //    //                            if (((UnityEngine.Object)val) != null)
    //    //                            {
    //    //                                context.Value.Result = true;
    //    //                            }
    //    //                        }
    //    //                        else if (val != null)
    //    //                        {
    //    //                            context.Value.Result = true;
    //    //                        }
    //    //                    }
    //    //                    else if (Equals(val, value))
    //    //                    {
    //    //                        context.Value.Result = true;
    //    //                    }
    //    //                }
    //    //            }

    //    //            result = context.Value.Result;
    //    //        }

    //    public static void HandleIfAttributesCondition(OdinDrawer drawer, InspectorProperty property, string memberName, object value, out bool result, out string errorMessage)
    //    {
    //        var context = property.Context.Get(drawer, "IfAttributeContext", (IfHelper)null);
    //        if (context.Value == null)
    //        {
    //            context.Value = new IfHelper(property, memberName);
    //        }

    //        errorMessage = context.Value.ErrorMessage;
    //        result = context.Value.GetValue(value);
    //    }
    //}
}
#endif
#pragma warning enable