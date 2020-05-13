#pragma warning disable
//-----------------------------------------------------------------------
// <copyright file="ShowIfGroupAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    /// <summary>
    /// <p>ShowIfGroup allows for showing or hiding a group of properties based on a condition.</p>
    /// <p>The attribute is a group attribute and can therefore be combined with other group attributes, and even be used to show or hide entire groups.</p>
    /// </summary>
    /// <seealso cref="ShowIfAttribute"/>
    /// <seealso cref="HideIfAttribute"/>
    /// <seealso cref="HideIfGroupAttribute"/>
    /// <seealso cref="ShowInInspectorAttribute"/>
    /// <seealso cref="UnityEngine.HideInInspector"/>
    public class ShowIfGroupAttribute : PropertyGroupAttribute
    {
        private string memberName;

        /// <summary>
        /// Whether or not to slide the properties in and out when the state changes.
        /// </summary>
        public bool Animate;

        /// <summary>
        /// The optional member value.
        /// </summary>
        public object Value;

        /// <summary>
        /// Name of member to use when to show the group. Defaults to the name of the group, by can be overriden by setting this property.
        /// </summary>
        public string MemberName
        {
            get
            {
                return string.IsNullOrEmpty(this.memberName)
                    ? this.GroupName
                    : this.memberName;
            }
            set { this.memberName = value; }
        }

        /// <summary>
        /// Makes a group that can be shown or hidden based on a condition.
        /// </summary>
        /// <param name="path">The group path.</param>
        /// <param name="animate">If <c>true</c> then a fade animation will be played when the group is hidden or shown.</param>
        public ShowIfGroupAttribute(string path, bool animate = true) : base(path)
        {
            this.Animate = animate;
        }

        /// <summary>
        /// Makes a group that can be shown or hidden based on a condition.
        /// </summary>
        /// <param name="path">The group path.</param>
        /// <param name="value">The value the member should equal for the property to shown.</param>
        /// <param name="animate">If <c>true</c> then a fade animation will be played when the group is hidden or shown.</param>
        public ShowIfGroupAttribute(string path, object value, bool animate = true) : base(path)
        {
            this.Value = value;
            this.Animate = animate;
        }

        /// <summary>
        /// Combines ShowIfGroup attributes.
        /// </summary>
        /// <param name="other">Another ShowIfGroup attribute.</param>
        protected override void CombineValuesWith(PropertyGroupAttribute other)
        {
            var attr = other as ShowIfGroupAttribute;
            if (string.IsNullOrEmpty(this.memberName) == false)
            {
                attr.memberName = this.memberName;
            }

            if (this.Animate == false)
            {
                attr.Animate = this.Animate;
            }

            if (this.Value != null)
            {
                attr.Value = this.Value;
            }
        }
    }
}
#pragma warning enable