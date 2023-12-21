#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="MemberEvaluator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.ValueResolver
{
    using Sirenix.Utilities;
    using System;
    using System.Reflection;

    internal class MemberEvaluator<TContext, TResult> : ProviderEvaluator<TContext, TResult> where TContext : ResolverContext
    {
        public override ValueProvider<TResult> TryEvaluate(TContext context, string reference)
        {
            if (string.IsNullOrEmpty(reference) == false && (this.Settings.RequireSymbolForMemberReferences == false || reference[0] == '$'))
            {
                string name = reference.TrimStart('$');

                var finder = MemberFinder.Start(context.GetParentType())
                    .HasNoParameters()
                    .HasConvertableReturnType<TResult>()
                    .IsNamed(name);

                if (this.IsStatic(context))
                {
                    finder = finder.IsStatic();
                }

                string error;
                MemberInfo member;
                if (finder.TryGetMember(out member, out error))
                {
                    bool isStatic = member.IsStatic();
                    Func<object, TResult> getter;

                    if (isStatic)
                    {
                        if (member is MethodInfo)
                        {
                            // TODO: Emit.
                            var method = member as MethodInfo;
                            getter = instance =>
                            {
                                return ConvertUtility.Convert<TResult>(method.Invoke(null, null));
                            };
                        }
                        else
                        {
                            // TODO: Emit
                            getter = instance =>
                            {
                                return ConvertUtility.Convert<TResult>(member.GetMemberValue(null));
                            };
                        }
                    }
                    else
                    {
                        if (member is MethodInfo)
                        {
                            // TODO: Emit.
                            var method = member as MethodInfo;
                            getter = instance =>
                            {
                                if (instance == null)
                                {
                                    throw new NullReferenceException("Parent instance missing.");
                                }

                                return ConvertUtility.Convert<TResult>(method.Invoke(instance, null));
                            };
                        }
                        else
                        {
                            // TODO: Emit
                            getter = instance =>
                            {
                                if (instance == null)
                                {
                                    throw new NullReferenceException("Parent instance missing.");
                                }

                                return ConvertUtility.Convert<TResult>(member.GetMemberValue(instance));
                            };
                        }
                    }

                    return new FuncValueProvider<TResult>(context, isStatic == false, getter);

                    //if (member.IsStatic())
                    //{
                    //    if (member is MethodInfo)
                    //    {
                    //        var method = member as MethodInfo;
                    //        return new FuncValueProvider<TResult>(context, false, o => ConvertUtility.Convert<TResult>(method.Invoke(null, null)));
                    //    }
                    //    // TODO: Can PropertyInfo and FieldInfo be straight as one?
                    //    else if (member is PropertyInfo)
                    //    {

                    //        EmitUtilities.CreateGetter
                    //    }
                    //    else if (member is FieldInfo)
                    //    {
                    //        var field = member as FieldInfo;
                    //        //EmitUtilities.CreateStaticFieldGetter
                    //        //return new FuncValueProvider<TResult>(context, false, o => ConvertUtility.Convert<TResult>(member.GetMemberValue(null)));
                    //        if (typeof(TResult).IsAssignableFrom(field.GetReturnType()))
                    //        {
                    //            var getter = EmitUtilities.CreateStaticFieldGetter<TResult>(field);
                    //            return new StaticFuncValueProvider<TResult>(context, getter); 
                    //        }
                    //        else
                    //        {
                    //            var getter = EmitUtilities.CreateWeakStaticFieldGetter(field);
                    //            return new StaticFuncValueProvider<TResult>(context, () => ConvertUtility.Convert<TResult>(getter()));
                    //        }
                    //    }
                    //    else
                    //    {
                    //        throw new InvalidOperationException("Hang on that's not right.");
                    //    }
                    //}
                    //else
                    //{
                    //    return new MemberValueProvider<TResult>(context, member);
                    //}
                }
                else
                {
                    context.LogFailInfo(this, "Cannot find a member in type '" + context.GetParentType().GetNiceName() + "' named '" + name + "' with a return type castable to '" + typeof(TResult).GetNiceName() + "'");
                }
            }

            return null;
        }
    }
}
#endif
#pragma warning enable