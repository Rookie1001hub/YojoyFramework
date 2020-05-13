#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ValidationResult.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace Sirenix.OdinInspector.Editor.Validation
{
    using Sirenix.Utilities;
    using System;
    using System.Linq;

    public class ValidationResult
    {
        public ValidationPathStep[] Path;
        public string Message;
        public ValidationResultType ResultType;
        public object ResultValue;
        public ValidationSetup Setup;

        private string fullPath;

        public string GetFullPath()
        {
            if (this.fullPath != null) return this.fullPath;

            if (this.Path == null)
            {
                this.fullPath = "";
            }
            else
            {
                this.fullPath = string.Join(".", this.Path.Select(x => x.StepString).ToArray());
            }

            return this.fullPath;
        }

        public void RerunValidation()
        {
            if (this.Setup.Validator == null)
                return;

            this.fullPath = null;

            var result = this;

            var setupBackup = this.Setup;

            try
            {
                if (this.Setup.Kind == ValidationKind.Value)
                {
                    this.Setup.Validator.RunValueValidation(this.Setup.Value, this.Setup.Root, ref result);
                }
                else
                {
                    this.Setup.Validator.RunMemberValidation(this.Setup.ParentInstance, this.Setup.Member, this.Setup.Value, this.Setup.Root, ref result);
                }
            }
            catch (Exception ex)
            {
                this.Setup = setupBackup;

                if (this.Setup.Kind == ValidationKind.Member)
                {
                    this.Message = "Exception was thrown during validation of " + this.Setup.Member.DeclaringType.GetNiceName() + "." + this.Setup.Member.Name + ": " + ex.ToString();
                }
                else
                {
                    this.Message = "Exception was thrown during validation of value '" + (this.Setup.Value == null ? "null" : this.Setup.Value.ToString()) + "': " + ex.ToString();
                }

                this.ResultType = ValidationResultType.Error;
            }
        }

        public ValidationResult CreateCopy()
        {
            var copy = new ValidationResult();

            copy.Path = this.Path;
            copy.Message = this.Message;
            copy.ResultType = this.ResultType;
            copy.ResultValue = this.ResultValue;
            copy.Setup = this.Setup;

            return copy;
        }
    }
}
#endif
#pragma warning enable