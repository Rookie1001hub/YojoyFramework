#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="StringMemberHelper.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;
    using UnityEngine;
    using Sirenix.Utilities.Editor.Expressions;
    using Sirenix.Utilities;
    using System.Reflection;

    /// <summary>
    ///	Helper class to handle strings for labels and other similar purposes.
    ///	Allows for a static string, or for refering to string member fields, properties or methods,
    ///	by name, if the first character is a '$'.
    ///	Also supports expressions in Odin Pro version.
    /// </summary>
    public class StringMemberHelper
    {
        private string buffer;
        private string rawString;
        private string errorMessage;
        private readonly Type objectType;
        private bool isStatic;

        private Func<object> staticValueGetter;
        private Func<object, object> instanceValueGetter;

#if !ODIN_LIMITED_VERSION
        private InspectorProperty property;
        private Delegate expressionMethod;
#endif

        /// <summary>
        /// If any error occurred while looking for members, it will be stored here.
        /// </summary>
        public string ErrorMessage { get { return this.errorMessage; } }

        /// <summary>
        /// Obsolete. Use other constructor.
        /// </summary>
        [Obsolete("Use the contructor with the InspectorProperty argument instead.")]
        public StringMemberHelper(Type objectType, string path, bool allowInstanceMember = true, bool allowStaticMember = true)
        {
            this.rawString = path;
            this.objectType = objectType;

            if (path != null && objectType != null && path.Length > 0 && path[0] == '$')
            {
                path = path.Substring(1);

                var finder = MemberFinder.Start(objectType)
                    .HasReturnType<object>(true)
                    .IsNamed(path)
                    .HasNoParameters();

                if (!allowInstanceMember && !allowStaticMember)
                {
                    throw new InvalidOperationException("Require either allowInstanceMember or allowStaticMember to be true.");
                }
                else if (!allowInstanceMember)
                {
                    finder.IsStatic();
                }
                else if (!allowStaticMember)
                {
                    finder.IsInstance();
                }

                MemberInfo member;
                if (finder.TryGetMember(out member, out this.errorMessage))
                {
                    if (member is MethodInfo)
                    {
                        path += "()";
                    }

                    if (member.IsStatic())
                    {
                        this.staticValueGetter = DeepReflection.CreateValueGetter<object>(objectType, path);
                    }
                    else
                    {
                        this.instanceValueGetter = DeepReflection.CreateWeakInstanceValueGetter<object>(objectType, path);
                    }
                }
            }
        }
        
        /// <summary>
        /// Obsolete. Use other constructor.
        /// </summary>
        [Obsolete("Use the contructor with the InspectorProperty argument instead.")]
        public StringMemberHelper(Type objectType, string path, ref string errorMessage, bool allowInstanceMember = true, bool allowStaticMember = true)
            : this(objectType, path, allowInstanceMember, allowStaticMember)
        {
            if (errorMessage == null)
            {
                errorMessage = this.ErrorMessage;
            }
        }

        /// <summary>
        /// Creates a StringMemberHelper to get a display string.
        /// </summary>
        /// <param name="property">Inspector property to get string from.</param>
        /// <param name="text">The input string. If the first character is a '$', then StringMemberHelper will look for a member string field, property or method, and will try to parse it as an expression if it starts with '@'.</param>
        public StringMemberHelper(InspectorProperty property, string text) : this(property.ParentType, property.ParentValueProperty == null && property.Tree.IsStatic, text, property)
        {
        }

        /// <summary>
        /// Creates a StringMemberHelper to get a display string.
        /// </summary>
        /// <param name="property">Inspector property to get string from.</param>
        /// <param name="text">The input string. If the first character is a '$', then StringMemberHelper will look for a member string field, property or method, and will try to parse it as an expression if it starts with '@'.</param>/// <param name="text">The input string. If the first character is a '$', then StringMemberHelper will look for a member string field, property or method.</param>
        public StringMemberHelper(InspectorProperty property, string text, ref string errorMessage) : this(property, text)
        {
            if (errorMessage == null)
            {
                errorMessage = this.ErrorMessage;
            }
        }

        /// <summary>
        /// Creates a StringMemberHelper to get a display string.
        /// </summary>
        /// <param name="objectType">The type of the parent, to get a member string from.</param>
        /// <param name="isStatic">Value indicating if the context should be static.</param>
        /// <param name="text">The input string. If the first character is a '$', then StringMemberHelper will look for a member string field, property or method, and will try to parse it as an expression if it starts with '@'.</param>/// <param name="text">The input string. If the first character is a '$', then StringMemberHelper will look for a member string field, property or method.</param>
        


        public StringMemberHelper(Type objectType, bool isStatic, string text) : this(objectType, isStatic, text, null)
        {

        }

        private StringMemberHelper(Type objectType, bool isStatic, string text, InspectorProperty property)
        {
            this.rawString = text;
            this.objectType = objectType;
            this.isStatic = isStatic;

            if (string.IsNullOrEmpty(text) == false && objectType != null && text.Length > 0)
            {
                if (text[0] == '$')
                {
                    text = text.Substring(1);

                    var finder = MemberFinder.Start(objectType)
                        .HasReturnType<object>(true)
                        .IsNamed(text)
                        .HasNoParameters();

                    if (isStatic)
                    {
                        finder = finder.IsStatic();
                    }

                    MemberInfo member;
                    if (finder.TryGetMember(out member, out this.errorMessage))
                    {
                        if (member is MethodInfo)
                        {
                            text += "()";
                        }

                        if (member.IsStatic())
                        {
                            this.staticValueGetter = DeepReflection.CreateValueGetter<object>(objectType, text);
                        }
                        else
                        {
                            this.instanceValueGetter = DeepReflection.CreateWeakInstanceValueGetter<object>(objectType, text);
                        }
                    }
                }
                else if (text[0] == '@')
                {

#if !ODIN_LIMITED_VERSION

                    Type[] parameters = null;
                    string[] parameterNames = null;

                    this.property = property;

                    if (property != null)
                    {
                        parameters = new Type[] { typeof(InspectorProperty) };
                        parameterNames = new string[] { "property" };
                    }

                    this.expressionMethod = ExpressionUtility.ParseExpression(text.Substring(1), this.isStatic, objectType, parameters, parameterNames, out this.errorMessage); 
#else
                    this.errorMessage += "\nExpressions are only available in Odin Inspector Commercial and up.";
#endif
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the string is retrived from a from a member. 
        /// </summary>
        public bool IsDynamicString
        {
            get
            {
                return
#if !ODIN_LIMITED_VERSION
                    this.expressionMethod != null ||
#endif
                    this.instanceValueGetter != null || this.staticValueGetter != null;
            }
        }

        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        public Type ObjectType { get { return this.objectType; } }

        /// <summary>
        /// Gets the string from the StringMemberHelper.
        /// Only updates the string buffer in Layout events.
        /// </summary>
        /// <param name="entry">The property entry, to get the instance reference from.</param>
        /// <returns>The current display string.</returns>
        public string GetString(IPropertyValueEntry entry)
        {
            return this.GetString(entry.Property.ParentValues[0]);
        }

        /// <summary>
        /// Gets the string from the StringMemberHelper.
        /// Only updates the string buffer in Layout events.
        /// </summary>
        /// <param name="property">The property, to get the instance reference from.</param>
        /// <returns>The current string.</returns>
        public string GetString(InspectorProperty property)
        {
            return this.GetString(property.ParentValues[0]);
        }

        /// <summary>
        /// Gets the string from the StringMemberHelper.
        /// Only updates the string buffer in Layout events.
        /// </summary>
        /// <param name="instance">The instance, for evt. member references.</param>
        /// <returns>The current string.</returns>
        public string GetString(object instance)
        {
            if (this.buffer == null || Event.current == null || Event.current.type == EventType.Layout)
            {
                this.buffer = this.ForceGetString(instance);
            }

            return this.buffer;
        }

        /// <summary>
        /// Gets the string from the StringMemberHelper.
        /// </summary>
        /// <param name="entry">The property entry, to get the instance reference from.</param>
        /// <returns>The current string.</returns>
        public string ForceGetString(IPropertyValueEntry entry)
        {
            return this.ForceGetString(entry.Property.ParentValues[0]);
        }

        /// <summary>
        /// Gets the string from the StringMemberHelper.
        /// </summary>
        /// <param name="property">The property, to get the instance reference from.</param>
        /// <returns>The current string.</returns>
        public string ForceGetString(InspectorProperty property)
        {
            return this.ForceGetString(property.ParentValues[0]);
        }

        /// <summary>
        /// Gets the string from the StringMemberHelper.
        /// </summary>
        /// <param name="instance">The instance, for evt. member references.</param>
        /// <returns>The current string.</returns>
        public string ForceGetString(object instance)
        {
            if (this.errorMessage != null)
            {
                return "Error";
            }

#if !ODIN_LIMITED_VERSION
            if (this.expressionMethod != null)
            {
                object o;
                if (this.isStatic)
                {
                    if (this.property != null)
                    {
                        o = this.expressionMethod.DynamicInvoke(this.property);
                    }
                    else
                    {
                        o = this.expressionMethod.DynamicInvoke();
                    }
                }
                else
                {
                    if (this.property != null)
                    {
                        o = this.expressionMethod.DynamicInvoke(instance, this.property);
                    }
                    else
                    {
                        o = this.expressionMethod.DynamicInvoke(instance);
                    }
                }

                return (o == null) ? "Null" : o.ToString();
            }
#endif

            // TODO: Will this ever be used anymore?
            if (this.staticValueGetter != null)
            {
                var o = this.staticValueGetter();
                return (o == null) ? "Null" : o.ToString();
            }

            if (instance != null && this.instanceValueGetter != null)
            {
                var o = this.instanceValueGetter(instance);
                return (o == null) ? "Null" : o.ToString();
            }

            return this.rawString;
        }
    }
}
#endif
#pragma warning enable