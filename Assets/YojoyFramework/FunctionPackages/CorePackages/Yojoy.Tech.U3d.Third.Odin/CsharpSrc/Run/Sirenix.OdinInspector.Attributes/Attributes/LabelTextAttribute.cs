#pragma warning disable
//-----------------------------------------------------------------------
// <copyright file="LabelTextAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;
    using Yojoy.Tech.Common.Core.Run;
    /// <summary>
    /// <para>LabelText is used to change the labels of properties.</para>
    /// <para>Use this if you want a different label than the name of the property.</para>
    /// </summary>
    /// <example>
    /// <para>The following example shows how LabelText is applied to a few property fields.</para>
    /// <code>
    /// public MyComponent : MonoBehaviour
    /// {
    ///		[LabelText("1")]
    ///		public int MyInt1;
    ///
    ///		[LabelText("2")]
    ///		public int MyInt2;
    ///
    ///		[LabelText("3")]
    ///		public int MyInt3;
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="TitleAttribute"/>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class LabelTextAttribute : Attribute
    {
        private MultiLanguageString multiLanguageString;
        /// <summary>
        /// The new text of the label.
        /// </summary>
        public string Text
        {
            get => multiLanguageString.Text;
            private set => multiLanguageString = MultiLanguageString.Create(englishValue: value, chinesValue: value);
        }

        /// <summary>
        /// Give a property a custom label.
        /// </summary>
        /// <param name="text">The new text of the label.</param>
        public LabelTextAttribute(string english,string chinese=null)
        {
            multiLanguageString = MultiLanguageString.Create(english, chinese);
        }
    }
}
#pragma warning enable