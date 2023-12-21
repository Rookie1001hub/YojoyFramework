#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ValidateInputAttributeValidator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Editor.Validation.ValidateInputAttributeValidator<>))]

namespace Sirenix.OdinInspector.Editor.Validation
{
    using System;
    using System.Reflection;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor.Expressions;

    public class ValidateInputAttributeValidator<T> : AttributeValidator<ValidateInputAttribute, T>
    {
        private delegate bool ValidateDelegateStatic1(T value, ref string message);
        private delegate bool ValidateDelegateStatic2(T value, ref string message, ref InfoMessageType? messageType);
        private delegate bool ValidateDelegateInstance1(object parentInstance, T value, ref string message);
        private delegate bool ValidateDelegateInstance2(object parentInstance, T value, ref string message, ref InfoMessageType? messageType);

        public override RevalidationCriteria RevalidationCriteria
        {
            get
            {
                if (this.Attribute.ContinuousValidationCheck)
                    return RevalidationCriteria.Always;

                if (this.Attribute.IncludeChildren)
                    return RevalidationCriteria.OnValueChangeOrChildValueChange;

                return RevalidationCriteria.OnValueChange;
            }
        }

        private string memberErrorMessage;
        private string defaultMessageString;
        private ValidationResultType defaultResultType;

        private Func<T, bool> staticValidationMethodCaller;
        private Func<object, T, bool> instanceValidationMethodCaller;
        private ValidateDelegateStatic1 staticValidationMethodCallerWithMessage;
        private ValidateDelegateStatic2 staticValidationMethodCallerWithMessageAndType;
        private ValidateDelegateInstance1 instanceValidationMethodCallerWithMessage;
        private ValidateDelegateInstance2 instanceValidationMethodCallerWithMessageAndType;
        private StringMemberHelper validationMessageHelper;

        private ExpressionFunc<T, bool> staticValidationExpression;
        private Delegate instanceValidationExpression;

        public override void Initialize(MemberInfo member, Type memberValueType)
        {
            if (this.Attribute.MemberName != null && this.Attribute.MemberName.Length > 0 && this.Attribute.MemberName[0] == '@')
            {
                var expression = this.Attribute.MemberName.Substring(1);

                var emitContext = new EmitContext()
                {
                    IsStatic = member.IsStatic(),
                    ReturnType = typeof(bool),
                    Type = member.ReflectedType,
                    Parameters = new Type[] { member.GetReturnType() },
                    ParameterNames = new string[] { "value" }
                };

                var del = ExpressionUtility.ParseExpression(expression, emitContext, out this.memberErrorMessage);
                
                if (emitContext.IsStatic)
                {
                    this.staticValidationExpression = (ExpressionFunc<T, bool>)del;
                }
                else
                {
                    this.instanceValidationExpression = del;
                }
            }
            else
            {
                LegacyFindMember(member);
            }

            this.defaultMessageString = this.Attribute.DefaultMessage ?? "Value is invalid for member '" + member.Name + "'";
            this.defaultResultType = this.Attribute.MessageType.ToValidationResultType();

            if (this.Attribute.DefaultMessage != null)
            {
                this.validationMessageHelper = new StringMemberHelper(member.ReflectedType, false, this.Attribute.DefaultMessage);

                if (this.validationMessageHelper.ErrorMessage != null)
                {
                    if (this.memberErrorMessage != null)
                    {
                        this.memberErrorMessage += "\n\n" + this.validationMessageHelper.ErrorMessage;
                    }
                    else
                    {
                        this.memberErrorMessage = this.validationMessageHelper.ErrorMessage;
                    }

                    this.validationMessageHelper = null;
                }
            }
        }

        private void LegacyFindMember(MemberInfo member)
        {
            MethodInfo methodInfo = member.ReflectedType.FindMember()
                                        .IsMethod()
                                        .HasReturnType<bool>()
                                        .HasParameters(member.GetReturnType())
                                        .IsNamed(this.Attribute.MemberName)
                                        .GetMember<MethodInfo>(out this.memberErrorMessage);

            if (this.memberErrorMessage == null)
            {
                if (methodInfo.IsStatic())
                {
                    this.staticValidationMethodCaller = (Func<T, bool>)Delegate.CreateDelegate(typeof(Func<T, bool>), methodInfo);
                }
                else
                {
                    object[] args = new object[1];
                    this.instanceValidationMethodCaller = (object parentInstance, T value) =>
                    {
                        args[0] = value;
                        return (bool)methodInfo.Invoke(parentInstance, args);
                    };
                }
            }
            else
            {
                string errorMsg;

                methodInfo = member.ReflectedType.FindMember()
                    .IsMethod()
                    .HasReturnType<bool>()
                    .HasParameters(member.GetReturnType(), typeof(string).MakeByRefType())
                    .IsNamed(this.Attribute.MemberName)
                    .GetMember<MethodInfo>(out errorMsg);

                if (errorMsg == null)
                {
                    this.memberErrorMessage = null;

                    if (methodInfo.IsStatic())
                    {
                        this.staticValidationMethodCallerWithMessage = (ValidateDelegateStatic1)Delegate.CreateDelegate(typeof(ValidateDelegateStatic1), methodInfo);
                    }
                    else
                    {
                        object[] args = new object[2];
                        this.instanceValidationMethodCallerWithMessage = delegate (object parentInstance, T value, ref string message)
                        {
                            args[0] = value;
                            args[1] = message;
                            bool result = (bool)methodInfo.Invoke(parentInstance, args);
                            message = args[1] as string ?? "";
                            return result;
                        };
                    }
                }
                else
                {
                    this.memberErrorMessage += "\nor\n" + errorMsg;

                    methodInfo = member.ReflectedType.FindMember()
                        .IsMethod()
                        .HasReturnType<bool>()
                        .HasParameters(member.GetReturnType(), typeof(string).MakeByRefType(), typeof(Nullable<>).MakeGenericType(typeof(InfoMessageType)).MakeByRefType())
                        .IsNamed(this.Attribute.MemberName)
                        .GetMember<MethodInfo>(out errorMsg);

                    if (errorMsg == null)
                    {
                        this.memberErrorMessage = null;

                        if (methodInfo.IsStatic())
                        {
                            this.staticValidationMethodCallerWithMessageAndType = (ValidateDelegateStatic2)Delegate.CreateDelegate(typeof(ValidateDelegateStatic2), methodInfo);
                        }
                        else
                        {
                            object[] args = new object[3];
                            this.instanceValidationMethodCallerWithMessageAndType = delegate (object parentInstance, T value, ref string message, ref InfoMessageType? messageType)
                            {
                                args[0] = value;
                                args[1] = message;
                                args[2] = messageType;
                                bool result = (bool)methodInfo.Invoke(parentInstance, args);
                                message = args[1] as string ?? "";
                                messageType = args[2] == null ? null : (InfoMessageType?)args[2];
                                return result;
                            };
                        }
                    }
                    else
                    {
                        this.memberErrorMessage += "\nor\n" + errorMsg;
                    }
                }
            }
        }

        protected override void Validate(object parentInstance, T memberValue, MemberInfo member, ValidationResult result)
        {
            if (this.memberErrorMessage != null)
            {
                result.Message = this.memberErrorMessage;
                result.ResultType = ValidationResultType.Error;
                return;
            }

            string messageParam = null;
            InfoMessageType? messageTypeParam = null;
            
            if (this.staticValidationExpression != null)
            {
                if (!this.staticValidationExpression(memberValue))
                {
                    result.ResultType = this.defaultResultType;
                    result.Message = this.GetDefaultMessage(parentInstance);
                }
            }
            else if (this.instanceValidationExpression != null)
            {
                var boolResult = (bool)this.instanceValidationExpression.DynamicInvoke(new object[] { parentInstance, memberValue });
                if (!boolResult)
                {
                    result.ResultType = this.defaultResultType;
                    result.Message = this.GetDefaultMessage(parentInstance);
                }
            }
            else if (this.staticValidationMethodCaller != null)
            {
                if (!this.staticValidationMethodCaller(memberValue))
                {
                    result.ResultType = this.defaultResultType;
                    result.Message = this.GetDefaultMessage(parentInstance);
                }
            }
            else if (this.instanceValidationMethodCaller != null)
            {
                if (!this.instanceValidationMethodCaller(parentInstance, memberValue))
                {
                    result.ResultType = this.defaultResultType;
                    result.Message = this.GetDefaultMessage(parentInstance);
                }
            }
            else if (this.staticValidationMethodCallerWithMessage != null)
            {
                if (!this.staticValidationMethodCallerWithMessage(memberValue, ref messageParam))
                {
                    result.ResultType = this.defaultResultType;
                    result.Message = messageParam;
                }
            }
            else if (this.instanceValidationMethodCallerWithMessage != null)
            {
                if (!this.instanceValidationMethodCallerWithMessage(parentInstance, memberValue, ref messageParam))
                {
                    result.ResultType = this.defaultResultType;
                    result.Message = messageParam;
                }
            }
            else if (this.staticValidationMethodCallerWithMessageAndType != null)
            {
                if (!this.staticValidationMethodCallerWithMessageAndType(memberValue, ref messageParam, ref messageTypeParam))
                {
                    result.Message = messageParam;

                    if (!messageTypeParam.HasValue)
                        result.ResultType = this.defaultResultType;
                    else
                        result.ResultType = messageTypeParam.Value.ToValidationResultType();
                }
            }
            else if (this.instanceValidationMethodCallerWithMessageAndType != null)
            {
                if (!this.instanceValidationMethodCallerWithMessageAndType(parentInstance, memberValue, ref messageParam, ref messageTypeParam))
                {
                    result.Message = messageParam;

                    if (!messageTypeParam.HasValue)
                        result.ResultType = this.defaultResultType;
                    else
                        result.ResultType = messageTypeParam.Value.ToValidationResultType();
                }
            }
        }

        private string GetDefaultMessage(object parentInstance)
        {
            if (this.validationMessageHelper != null)
            {
                return this.validationMessageHelper.GetString(parentInstance);
            }

            return this.defaultMessageString;
        }
    }
}
#endif
#pragma warning enable