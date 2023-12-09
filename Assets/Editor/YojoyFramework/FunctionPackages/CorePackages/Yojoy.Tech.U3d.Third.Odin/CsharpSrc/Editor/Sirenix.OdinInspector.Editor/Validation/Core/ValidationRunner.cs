#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ValidationRunner.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Validation
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Serialization;
    using Sirenix.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using UnityEngine;

    public class ValidationRunner
    {
        private static readonly IMemberSelector UnityMemberSelector = new SerializationPolicyMemberSelector(new CustomSerializationPolicy("UNITY_ONLY_VALIDATOR", true, (member) => UnitySerializationUtility.GuessIfUnityWillSerialize(member) || member.IsDefined<ShowInInspectorAttribute>(true)));
        private static readonly IMemberSelector OdinMemberSelector = new SerializationPolicyMemberSelector(new CustomSerializationPolicy("ODIN_VALIDATOR", true, (member) => member.IsDefined<ShowInInspectorAttribute>(true) ? true : SerializationPolicies.Unity.ShouldSerializeMember(member)));

        public IValidatorLocator ValidatorLocator = new DefaultValidatorLocator();
        public int MaxScanDepth = 100;
        public bool WarnOnScanDepthReached = true;
        public bool RecurseThroughUnityObjectReferences = false;

        public List<ValidationResult> ValidateUnityObjectRecursively(UnityEngine.Object value)
        {
            List<ValidationResult> result = null;
            ValidateUnityObjectRecursively(value, ref result);
            return result;
        }

        public virtual void ValidateUnityObjectRecursively(UnityEngine.Object value, ref List<ValidationResult> results)
        {
            if (results == null)
                results = new List<ValidationResult>();

            if (object.ReferenceEquals(value, null)) return;

            var type = value.GetType();

            bool odinSerialized = type.IsDefined<ShowOdinSerializedPropertiesInInspectorAttribute>(inherit: true);
            bool odinSerializesUnityMembers = false;

            IMemberSelector selector;

            if (odinSerialized)
            {
                var policyOverride = value as IOverridesSerializationPolicy;

                if (policyOverride != null)
                {
                    selector = new SerializationPolicyMemberSelector(policyOverride.SerializationPolicy ?? SerializationPolicies.Unity);
                    odinSerializesUnityMembers = policyOverride.OdinSerializesUnityFields;
                }
                else
                {
                    selector = OdinMemberSelector;
                }
            }
            else
            {
                selector = UnityMemberSelector;
            }

            this.ValidateValue(value, value, ref results);

            HashSet<object> seenReferences = new HashSet<object>(ReferenceEqualityComparer<object>.Default);
            HashSet<MemberInfo> seenMembers = new HashSet<MemberInfo>(FastMemberComparer.Instance);
            
            foreach (var member in selector.SelectMembers(type))
            {
                IMemberSelector childSelector = selector;

                if (odinSerialized && !odinSerializesUnityMembers && UnitySerializationUtility.GuessIfUnityWillSerialize(member))
                {
                    childSelector = UnityMemberSelector;
                }

                var memberValue = GetMemberValue(member, value, results);
                var memberValueType = memberValue == null ? member.GetReturnType() : memberValue.GetType();

                ValidateMemberRecursive(value, member, memberValue, memberValueType, childSelector, value, new List<ValidationPathStep>() { new ValidationPathStep() { Value = memberValue, Member = member, StepString = member.Name } }, seenReferences, seenMembers, results, 0);
            }
        }

        private object GetMemberValue(MemberInfo member, object value, List<ValidationResult> results)
        {
            try
            {
                return member.GetMemberValue(value);
            }
            catch (Exception ex) 
            {
                results.Add(new ValidationResult()
                {
                    Message = "An exception was thrown trying to get the value of member '" + member.DeclaringType.FullName + "." + member.Name + "': " + ex.ToString(),
                    ResultType = ValidationResultType.Error,
                    Setup = new ValidationSetup()
                    {
                        Member = member,
                        ParentInstance = value,
                    }
                });

                var type = member.GetReturnType();

                if (type == null) throw new Exception("Ummm");
                
                if (type.IsValueType)
                    return Activator.CreateInstance(type);
                else
                    return null;
            }
        }

        public virtual void ValidateMembers(object value, IMemberSelector selector, UnityEngine.Object root, bool isCollectionElement, ref List<ValidationResult> results)
        {
            if (results == null)
                results = new List<ValidationResult>();

            if (object.ReferenceEquals(value, null)) return;

            foreach (var member in selector.SelectMembers(value.GetType()))
            {
                var memberValue = GetMemberValue(member, value, results);
                var memberValueType = memberValue == null ? member.GetReturnType() : memberValue.GetType();

                ValidateMember(value, member, memberValue, memberValueType, root, isCollectionElement, ref results);
            }
        }

        public virtual void ValidateMembersRecursively(object value, IMemberSelector selector, UnityEngine.Object root, ref List<ValidationResult> results)
        {
            if (results == null)
                results = new List<ValidationResult>();

            if (object.ReferenceEquals(value, null)) return;

            HashSet<object> seenReferences = new HashSet<object>(ReferenceEqualityComparer<object>.Default);
            HashSet<MemberInfo> seenMembers = new HashSet<MemberInfo>(FastMemberComparer.Instance);

            foreach (var member in selector.SelectMembers(value.GetType()))
            {
                var memberValue = GetMemberValue(member, value, results);
                var memberValueType = memberValue == null ? member.GetReturnType() : memberValue.GetType();

                ValidateMemberRecursive(value, member, memberValue, memberValueType, selector, root, new List<ValidationPathStep>() { new ValidationPathStep() {
                Member = member,
                Value = memberValue,
                StepString = member.Name
            } }, seenReferences, seenMembers, results, 0);
            }
        }

        public virtual void ValidateMember(object parentInstance, MemberInfo member, object memberValue, Type memberValueType, UnityEngine.Object root, bool isCollectionElement, ref List<ValidationResult> results)
        {
            if (results == null)
                results = new List<ValidationResult>();

            foreach (var validator in this.ValidatorLocator.GetValidators(member, memberValueType, isCollectionElement))
            {
                ValidationResult result = null;

                try
                {
                    if (validator.CanValidateMembers())
                    {
                        validator.RunMemberValidation(parentInstance, member, memberValue, root, ref result);
                    }
                    else if (validator.CanValidateValues())
                    {
                        validator.RunValueValidation(memberValue, root, ref result);
                    }
                }
                catch (Exception ex)
                {
                    while (ex is TargetInvocationException)
                    {
                        ex = ex.InnerException;
                    }

                    result = new ValidationResult()
                    {
                        Message = "Exception was thrown during validation of " + member.DeclaringType.GetNiceName() + "." + member.Name + ": " + ex.ToString(),
                        ResultType = ValidationResultType.Error,
                        ResultValue = ex,
                        Setup = new ValidationSetup()
                        {
                            Kind = validator.CanValidateMembers() ? ValidationKind.Member : ValidationKind.Value,
                            Member = member,
                            ParentInstance = parentInstance,
                            Root = root,
                            Validator = validator,
                            Value = memberValue
                        }
                    };
                }
                
                if (result != null && result.ResultType != ValidationResultType.IgnoreResult)
                    results.Add(result);
            }
        }

        public virtual void ValidateValue(object value, UnityEngine.Object root, ref List<ValidationResult> results)
        {
            if (results == null)
                results = new List<ValidationResult>();

            if (value == null) return;

            foreach (var validator in this.ValidatorLocator.GetValidators(value.GetType()))
            {
                ValidationResult result = null;

                try
                {
                    validator.RunValueValidation(value, root, ref result);
                }
                catch (Exception ex)
                {
                    while (ex is TargetInvocationException)
                    {
                        ex = ex.InnerException;
                    }

                    result = new ValidationResult()
                    {
                        Message = "Exception was thrown during validation of value '" + (value == null ? "null" : value.ToString()) + "': " + ex.ToString(),
                        ResultType = ValidationResultType.Error,
                        ResultValue = ex,
                        Setup = new ValidationSetup()
                        {
                            Kind = ValidationKind.Value,
                            Root = root,
                            Validator = validator,
                            Value = value
                        }
                    };
                }

                if (result != null && result.ResultType != ValidationResultType.IgnoreResult)
                    results.Add(result);
            }
        }

        public virtual void ValidateMemberRecursive(object parentInstance, MemberInfo member, Type memberValueType, IMemberSelector selector, UnityEngine.Object root, ref List<ValidationResult> results)
        {
            if (results == null)
                results = new List<ValidationResult>();

            if (object.ReferenceEquals(parentInstance, null)) return;

            HashSet<object> seenReferences = new HashSet<object>(ReferenceEqualityComparer<object>.Default);
            HashSet<MemberInfo> seenMembers = new HashSet<MemberInfo>(FastMemberComparer.Instance);

            var memberValue = GetMemberValue(member, parentInstance, results);
            ValidateMemberRecursive(parentInstance, member, memberValue, memberValueType, selector, root, new List<ValidationPathStep>() { new ValidationPathStep() {
            Member = member,
            Value = memberValue,
            StepString = member.Name,
        } }, seenReferences, seenMembers, results, 0);
        }

        private static List<ValidationResult> TempResults = new List<ValidationResult>();

        protected virtual void ValidateMemberRecursive(object parentValue, MemberInfo member, object memberValue, Type memberValueType, IMemberSelector selector, UnityEngine.Object root, List<ValidationPathStep> pathSoFar, HashSet<object> seenReferences, HashSet<MemberInfo> seenMembers, List<ValidationResult> results, int scanDepth, bool isInCollection = false)
        {
            if (object.ReferenceEquals(parentValue, null)) return;

            if (!isInCollection && member.IsStatic() && !seenMembers.Add(member))
                return;

            TempResults.Clear();
            {
                ValidateMember(parentValue, member, memberValue, memberValueType, root, isInCollection, ref TempResults);
                foreach (var result in TempResults)
                    result.Path = pathSoFar.ToArray();
                results.AddRange(TempResults);
            }
            TempResults.Clear();

            if (object.ReferenceEquals(memberValue, null)) return;
            if (memberValue is UnityEngine.Object && !this.RecurseThroughUnityObjectReferences) return;

            if (!(memberValue is string) && !memberValue.GetType().IsPrimitive && seenReferences.Contains(memberValue)) return;

            if (scanDepth > this.MaxScanDepth)
            {
                if (this.WarnOnScanDepthReached)
                {
                    results.Add(new ValidationResult()
                    {
                        Message = "Max scan depth reached",
                        ResultType = ValidationResultType.Warning,
                        Path = pathSoFar.ToArray()
                    });
                }

                return;
            }

            seenReferences.Add(memberValue);
            scanDepth++;

            if (!TryValidateMemberRecursivelyAsCollection(memberValue, member, selector, root, pathSoFar, seenReferences, seenMembers, results, scanDepth))
            {
                foreach (var childMember in selector.SelectMembers(memberValue.GetType()))
                {
                    var path = pathSoFar.ToList();

                    var childMemberValue = GetMemberValue(childMember, memberValue, results);
                    var childMemberValueType = childMemberValue == null ? childMember.GetReturnType() : childMemberValue.GetType();

                    path.Add(new ValidationPathStep()
                    {
                        Member = childMember,
                        Value = childMemberValue,
                        StepString = childMember.Name
                    });

                    ValidateMemberRecursive(memberValue, childMember, childMemberValue, childMemberValueType, selector, root, path, seenReferences, seenMembers, results, scanDepth);
                }
            }
        }

        protected virtual bool TryValidateMemberRecursivelyAsCollection(object collection, MemberInfo collectionMember, IMemberSelector selector, UnityEngine.Object root, List<ValidationPathStep> pathSoFar, HashSet<object> seenReferences, HashSet<MemberInfo> seenMembers, List<ValidationResult> results, int scanDepth)
        {
            scanDepth++;

            if (collection is Array)
            {
                int index = 0;
                var elementType = collection.GetType().GetElementType();

                if (elementType.IsPrimitive)
                {
                    if (!this.ValidatorLocator.PotentiallyHasValidatorsFor(collectionMember, elementType, true))
                    {
                        // Don't actually validate the items in the array
                        return true;
                    }
                }

                foreach (var item in collection as Array)
                {
                    var path = pathSoFar.ToList();

                    path.Add(new ValidationPathStep()
                    {
                        Member = collectionMember,
                        Value = collection,
                        StepString = CollectionResolverUtilities.DefaultIndexToChildName(index)
                    });

                    var itemType = item == null ? elementType : item.GetType();

                    ValidateMemberRecursive(collection, collectionMember, item, itemType, selector, root, path, seenReferences, seenMembers, results, scanDepth, true);
                    index++;
                }

                return true;
            }
            else if (collection is IList)
            {
                var elementType = collection.GetType().ImplementsOpenGenericInterface(typeof(IList<>)) ? collection.GetType().GetArgumentsOfInheritedOpenGenericInterface(typeof(IList<>))[0] : typeof(object);

                if (elementType.IsPrimitive)
                {
                    if (!this.ValidatorLocator.PotentiallyHasValidatorsFor(collectionMember, elementType, true))
                    {
                        // Don't actually validate the items in the list
                        return true;
                    }
                }

                int index = 0;
                foreach (var item in collection as IList)
                {
                    var path = pathSoFar.ToList();

                    path.Add(new ValidationPathStep()
                    {
                        Member = collectionMember,
                        Value = collection,
                        StepString = CollectionResolverUtilities.DefaultIndexToChildName(index)
                    });

                    var itemType = item == null ? elementType : item.GetType();

                    ValidateMemberRecursive(collection, collectionMember, item, itemType, selector, root, path, seenReferences, seenMembers, results, scanDepth, true);
                    index++;
                }

                return true;
            }
            else if (collection is IDictionary)
            {
                Type baseKeyType, baseValueType;

                if (collection.GetType().ImplementsOpenGenericInterface(typeof(IDictionary<,>)))
                {
                    var args = collection.GetType().GetArgumentsOfInheritedOpenGenericInterface(typeof(IDictionary<,>));
                    baseKeyType = args[0];
                    baseValueType = args[1];
                }
                else
                {
                    baseKeyType = typeof(object);
                    baseValueType = typeof(object);
                }

                foreach (DictionaryEntry entry in collection as IDictionary)
                {
                    var keyPath = pathSoFar.ToList();
                    var valuePath = pathSoFar.ToList();

                    var keyStr = DictionaryKeyUtility.GetDictionaryKeyString(entry.Key);

                    keyPath.Add(new ValidationPathStep()
                    {
                        Member = collectionMember,
                        Value = entry.Key,
                        StepString = keyStr + "#key"
                    });

                    valuePath.Add(new ValidationPathStep()
                    {
                        Member = collectionMember,
                        Value = entry.Value,
                        StepString = keyStr
                    });

                    var keyType = entry.Key == null ? baseKeyType : entry.Key.GetType();
                    var valueType = entry.Value == null ? baseValueType : entry.Value.GetType();

                    ValidateMemberRecursive(collection, collectionMember, entry.Key, keyType, selector, root, keyPath, seenReferences, seenMembers, results, scanDepth, true);
                    ValidateMemberRecursive(collection, collectionMember, entry.Value, valueType, selector, root, valuePath, seenReferences, seenMembers, results, scanDepth, true);
                }

                return true;
            }
            else if (collection is IEnumerable && collection is ISerializable && collection is IDeserializationCallback && collection.GetType().ImplementsOpenGenericClass(typeof(HashSet<>)))
            {
                var elementType = collection.GetType().GetArgumentsOfInheritedOpenGenericType(typeof(HashSet<>))[0];

                foreach (var item in collection as IEnumerable)
                {
                    var path = pathSoFar.ToList();

                    path.Add(new ValidationPathStep()
                    {
                        Member = collectionMember,
                        Value = collection,
                        StepString = DictionaryKeyUtility.GetDictionaryKeyString(item)
                    });

                    var itemType = item == null ? elementType : item.GetType();
                    ValidateMemberRecursive(collection, collectionMember, item, itemType, selector, root, path, seenReferences, seenMembers, results, scanDepth, true);
                }

                return true;
            }
            else if (collection is ICollection || collection.GetType().ImplementsOpenGenericInterface(typeof(ICollection<>)))
            {
                Type elementType;

                if (collection.GetType().ImplementsOpenGenericInterface(typeof(ICollection<>)))
                {
                    elementType = collection.GetType().GetArgumentsOfInheritedOpenGenericType(typeof(ICollection<>))[0];
                }
                else
                {
                    elementType = typeof(object);
                }

                int index = 0;
                foreach (var item in collection as IEnumerable)
                {
                    var path = pathSoFar.ToList();

                    path.Add(new ValidationPathStep()
                    {
                        Member = collectionMember,
                        Value = collection,
                        StepString = CollectionResolverUtilities.DefaultIndexToChildName(index)
                    });

                    var itemType = item == null ? elementType : item.GetType();
                    ValidateMemberRecursive(collection, collectionMember, item, itemType, selector, root, path, seenReferences, seenMembers, results, scanDepth, true);
                    index++;
                }

                return true;
            }

            return false;
        }
    }


    //[assembly: RegisterValidator(typeof(StringValidator))]

    //public class StringValidator : ValueValidator<string>
    //{
    //    protected override void Validate(string value, ValidationResult result)
    //    {
    //        if (value == null)
    //        {
    //            result.Message = "String is null you asshat";
    //            result.ResultType = ValidationResultType.Error;
    //        }
    //        else if (!value.ToLower().Contains("awesome"))
    //        {
    //            result.Message = "String is not sufficiently awesome";
    //            result.ResultType = ValidationResultType.Warning;
    //        }
    //        // Default is a valid result with no message
    //        // But to demonstrate:
    //        else
    //        {
    //            result.Message = "String is pretty awesome";
    //            result.ResultType = ValidationResultType.Valid;
    //        }
    //    }
    //}
}
#endif
#pragma warning enable