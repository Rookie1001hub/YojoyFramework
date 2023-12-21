#pragma warning disable
//-----------------------------------------------------------------------
// <copyright file="OdinRegisterAttributeAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class OdinRegisterAttributeAttribute : Attribute
    {
        public Type AttributeType;
        public string Categories;
        public string Description;
        public string DocumentationUrl;
        
        public OdinRegisterAttributeAttribute(Type attributeType, string category, string description)
        {
            this.AttributeType = attributeType;
            this.Categories = category;
            this.Description = description;
        }
        public OdinRegisterAttributeAttribute(Type attributeType, string category, string description, string url)
        {
            this.AttributeType = attributeType;
            this.Categories = category;
            this.Description = description;
            this.DocumentationUrl = url;
        }
    }
}
#pragma warning enable