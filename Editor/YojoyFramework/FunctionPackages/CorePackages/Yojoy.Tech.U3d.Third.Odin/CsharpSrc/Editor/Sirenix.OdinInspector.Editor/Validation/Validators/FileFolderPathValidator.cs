#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="FileFolderPathValidator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.FilePathValidator))]
[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.FolderPathValidator))]

namespace Sirenix.OdinInspector.Editor.Validation
{
    using Sirenix.OdinInspector.Editor.ValueResolver;
    using System;
    using System.IO;
    using System.Reflection;

    public sealed class FilePathValidator : AttributeValidator<FilePathAttribute, string>
    {
        private static readonly IValueResolver<string> ParentPathResolver = ValueResolverUtility.GetResolver<string>()
            .TryMemberReference()
            .TryExpression();

        private bool requireExistingPath;
        private IValueProvider<string> parentPathProvider;

        public override void Initialize(MemberInfo member, Type memberValueType)
        {
#pragma warning disable CS0618 // Type or member is obsolete.
            this.requireExistingPath = this.Attribute.RequireExistingPath || this.Attribute.RequireValidPath;
#pragma warning restore CS0618 // Type or member is obsolete.

            if (this.requireExistingPath)
            {
                var context = ValueResolverUtility.CreateContext(this, member);
                this.parentPathProvider = ParentPathResolver.Resolve(context, this.Attribute.ParentFolder, null);
            }
        }

        protected override void Validate(object parentInstance, string memberValue, MemberInfo member, ValidationResult result)
        {
            if (this.requireExistingPath)
            {
                string path = memberValue;
                string parent = this.parentPathProvider.GetValue(parentInstance);

                if (string.IsNullOrEmpty(parent) == false)
                {
                    path = Path.Combine(parent, path);
                }

                if (File.Exists(path))
                {
                    result.ResultType = ValidationResultType.Valid;
                }
                else
                {
                    result.ResultType = ValidationResultType.Error;
                    result.Message = "The path does not exist.";
                }
            }
            else
            {
                result.ResultType = ValidationResultType.IgnoreResult;
            }
        }
    }

    public sealed class FolderPathValidator : AttributeValidator<FolderPathAttribute, string>
    {
        private static readonly IValueResolver<string> ParentPathResolver = ValueResolverUtility.GetResolver<string>()
            .TryMemberReference()
            .TryExpression();

        private bool requireExistingPath;
        private IValueProvider<string> parentPathProvider;

        public override void Initialize(MemberInfo member, Type memberValueType)
        {
#pragma warning disable CS0618 // Type or member is obsolete.
            this.requireExistingPath = this.Attribute.RequireExistingPath || this.Attribute.RequireValidPath;
#pragma warning restore CS0618 // Type or member is obsolete.

            if (this.requireExistingPath)
            {
                var context = ValueResolverUtility.CreateContext(this, member);
                this.parentPathProvider = ParentPathResolver.Resolve(context, this.Attribute.ParentFolder, null);
            }
        }

        protected override void Validate(object parentInstance, string memberValue, MemberInfo member, ValidationResult result)
        {
            if (this.requireExistingPath)
            {
                string path = memberValue;
                string parent = this.parentPathProvider.GetValue(parentInstance);

                if (string.IsNullOrEmpty(parent) == false)
                {
                    path = Path.Combine(parent, path);
                }

                if (Directory.Exists(path))
                {
                    result.ResultType = ValidationResultType.Valid;
                }
                else
                {
                    result.ResultType = ValidationResultType.Error;
                    result.Message = "The path does not exist.";
                }
            }
            else
            {
                result.ResultType = ValidationResultType.IgnoreResult;
            }
        }
    }
}
#endif
#pragma warning enable