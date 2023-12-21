#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="DefaultValidatorLocator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Validation
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector.Editor.TypeSearch;
    using Sirenix.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    public class DefaultValidatorLocator : IValidatorLocator
    {
        public class BrokenAttributeValidator : Validator
        {
            //private Type brokenValidatorType;
            private ValidationKind kind;
            private string message;

            public BrokenAttributeValidator(Type brokenValidatorType, ValidationKind kind, string message)
            {
                //this.brokenValidatorType = brokenValidatorType;
                this.kind = kind;
                this.message = message;
            }

            public override bool CanValidateValues()
            {
                return this.kind == ValidationKind.Value;
            }

            public override bool CanValidateMembers()
            {
                return this.kind == ValidationKind.Member;
            }

            public override void RunMemberValidation(object parentInstance, MemberInfo member, object memberValue, UnityEngine.Object root, ref ValidationResult result)
            {
                result.ResultType = ValidationResultType.Error;
                result.Message = this.message;
            }

            public override void RunValueValidation(object value, UnityEngine.Object root, ref ValidationResult result)
            {
                result.ResultType = ValidationResultType.Error;
                result.Message = this.message;
            }
        }

        public static readonly TypeSearchIndex ValueValidatorSearchIndex;
        public static readonly TypeSearchIndex AttributeValidatorSearchIndex;

        public Func<Type, bool> CustomValidatorFilter;

        static DefaultValidatorLocator()
        {
            ValueValidatorSearchIndex = new TypeSearchIndex();
            AttributeValidatorSearchIndex = new TypeSearchIndex();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var attr in assembly.GetAttributes<RegisterValidatorAttribute>())
                {
                    var type = attr.ValidatorType;

                    if (!type.InheritsFrom<Validator>())
                    {
                        Debug.LogError("The registered validator type " + attr.ValidatorType.GetNiceFullName() + " is not derived from " + typeof(Validator).GetNiceFullName());
                        continue;
                    }

                    if (type.ImplementsOpenGenericClass(typeof(AttributeValidator<,>)))
                    {
                        AttributeValidatorSearchIndex.AddIndexedType(new TypeSearchInfo()
                        {
                            MatchType = type,
                            Targets = type.GetArgumentsOfInheritedOpenGenericClass(typeof(AttributeValidator<,>)),
                            Priority = attr.Priority
                        });
                    }
                    else if (type.ImplementsOpenGenericClass(typeof(AttributeValidator<>)))
                    {
                        AttributeValidatorSearchIndex.AddIndexedType(new TypeSearchInfo()
                        {
                            MatchType = type,
                            Targets = type.GetArgumentsOfInheritedOpenGenericClass(typeof(AttributeValidator<>)),
                            Priority = attr.Priority
                        });
                    }
                    else if (type.ImplementsOpenGenericClass(typeof(ValueValidator<>)))
                    {
                        ValueValidatorSearchIndex.AddIndexedType(new TypeSearchInfo()
                        {
                            MatchType = type,
                            Targets = type.GetArgumentsOfInheritedOpenGenericClass(typeof(ValueValidator<>)),
                            Priority = attr.Priority
                        });
                    }
                    else
                    {
                        ValueValidatorSearchIndex.AddIndexedType(new TypeSearchInfo()
                        {
                            MatchType = type,
                            Targets = Type.EmptyTypes,
                            Priority = attr.Priority,
                        });

                        AttributeValidatorSearchIndex.AddIndexedType(new TypeSearchInfo()
                        {
                            MatchType = type,
                            Targets = Type.EmptyTypes,
                            Priority = attr.Priority,
                        });
                    }
                }
            }
        }

        public bool PotentiallyHasValidatorsFor(Type valueType)
        {
            return GetValidators(valueType).Count > 0;
        }

        public bool PotentiallyHasValidatorsFor(MemberInfo member, Type memberValueType, bool isCollectionElement)
        {
            return GetValidators(member, memberValueType, isCollectionElement).Count > 0;
        }

        protected readonly List<TypeSearchResult> ResultList = new List<TypeSearchResult>();
        protected readonly List<TypeSearchResult[]> SearchResultList = new List<TypeSearchResult[]>();
        protected readonly Dictionary<Type, int> AttributeNumberMap = new Dictionary<Type, int>(FastTypeComparer.Instance);

        protected readonly Dictionary<Type, Validator[]> ValueValidatorCache = new Dictionary<Type, Validator[]>(FastTypeComparer.Instance);
        protected readonly DoubleLookupDictionary<MemberInfo, Type, Validator[]> MemberValidatorCache = new DoubleLookupDictionary<MemberInfo, Type, Validator[]>(FastMemberComparer.Instance, FastTypeComparer.Instance);

        public virtual IList<Validator> GetValidators(Type valueType)
        {
            Validator[] result;
            if (!ValueValidatorCache.TryGetValue(valueType, out result))
            {
                result = CreateValidators(valueType);
                ValueValidatorCache.Add(valueType, result);
            }
            return result;
        }

        public virtual IList<Validator> GetValidators(MemberInfo member, Type memberValueType, bool isCollectionElement)
        {
            Validator[] result;
            if (!MemberValidatorCache.TryGetInnerValue(member, memberValueType, out result))
            {
                result = CreateValidators(member, memberValueType, isCollectionElement);
                MemberValidatorCache[member][memberValueType] = result;
            }
            return result;
        }

        protected virtual Validator[] CreateValidators(Type valueType)
        {
            List<Validator> validators = new List<Validator>();

            SearchResultList.Clear();

            SearchResultList.Add(ValueValidatorSearchIndex.GetMatches(Type.EmptyTypes));
            SearchResultList.Add(ValueValidatorSearchIndex.GetMatches(valueType));

            // @TODO Tor support this so it also works with attributes despite there being no member
            //var attributes = valueType.GetAttributes<Attribute>().ToList();

            //foreach (var attribute in attributes)
            //{
            //    SearchResultList.Add(AttributeValidatorSearchIndex.GetMatches(attribute.GetType()));
            //    SearchResultList.Add(AttributeValidatorSearchIndex.GetMatches(attribute.GetType(), valueType));
            //}

            ResultList.Clear();

            TypeSearchIndex.MergeQueryResultsIntoList(SearchResultList, ResultList);

            for (int i = 0; i < ResultList.Count; i++)
            {
                var result = ResultList[i];

                if (this.CustomValidatorFilter != null && !this.CustomValidatorFilter(result.MatchedType))
                    continue;

                try
                {
                    var validator = (Validator)Activator.CreateInstance(result.MatchedType);

                    if (validator.CanValidateValues() && validator.CanValidateValue(valueType))
                    {
                        validator.Initialize(valueType);
                        validators.Add(validator);
                    }
                }
                catch (Exception ex)
                {
                    validators.Add(new BrokenAttributeValidator(result.MatchedType, ValidationKind.Value, "Creating instance of validator '" + result.MatchedType.GetNiceName() + "' failed with exception: " + ex.ToString()));
                }
            }

            return validators.ToArray();
        }

        protected virtual Validator[] CreateValidators(MemberInfo member, Type memberValueType, bool isCollectionElement)
        {
            List<Validator> validators = new List<Validator>();

            SearchResultList.Clear();

            SearchResultList.Add(AttributeValidatorSearchIndex.GetMatches(Type.EmptyTypes));
            SearchResultList.Add(ValueValidatorSearchIndex.GetMatches(memberValueType));

            var attributes = member.GetAttributes<Attribute>().ToList();

            attributes.AddRange(memberValueType.GetAttributes<Attribute>());

            foreach (var attribute in attributes)
            {
                if (isCollectionElement && attribute.GetType().IsDefined<DontApplyToListElementsAttribute>(true))
                    continue;

                SearchResultList.Add(AttributeValidatorSearchIndex.GetMatches(attribute.GetType()));
                SearchResultList.Add(AttributeValidatorSearchIndex.GetMatches(attribute.GetType(), memberValueType));
            }

            ResultList.Clear();
            AttributeNumberMap.Clear();

            TypeSearchIndex.MergeQueryResultsIntoList(SearchResultList, ResultList);

            for (int i = 0; i < ResultList.Count; i++)
            {
                var result = ResultList[i];

                if (this.CustomValidatorFilter != null && !this.CustomValidatorFilter(result.MatchedType))
                    continue;

                try
                {
                    Validator validator = (Validator)Activator.CreateInstance(result.MatchedType);

                    if (validator is IAttributeValidator)
                    {
                        Attribute validatorAttribute = null;
                        Type attrType = result.MatchedTargets[0];

                        if (!attrType.InheritsFrom<Attribute>())
                            throw new NotSupportedException("Please don't manually implement the IAttributeValidator interface on any types; it's a special snowflake.");

                        int number, seen = 0;
                        AttributeNumberMap.TryGetValue(attrType, out number);

                        for (int j = 0; j < attributes.Count; j++)
                        {
                            var attr = attributes[j];

                            if (attr.GetType() == attrType)
                            {
                                if (seen == number)
                                {
                                    validatorAttribute = attr;
                                    break;
                                }
                                else
                                {
                                    seen++;
                                }
                            }
                        }

                        if (validatorAttribute == null)
                            throw new Exception("Could not find the correctly numbered attribute of type '" + attrType.GetNiceFullName() + "' on member " + member.DeclaringType.GetNiceName() + "." + member.Name + "; found " + seen + " attributes of that type, but needed number " + number + ".");

                        (validator as IAttributeValidator).SetAttributeInstance(validatorAttribute);

                        number++;
                        AttributeNumberMap[attrType] = number;
                    }

                    if (validator.CanValidateValues() && validator.CanValidateValue(memberValueType))
                    {
                        validator.Initialize(memberValueType);
                        validators.Add(validator);
                    }
                    else if (validator.CanValidateMembers() && validator.CanValidateMember(member, memberValueType))
                    {
                        validator.Initialize(member, memberValueType);
                        validators.Add(validator);
                    }
                }
                catch (Exception ex)
                {
                    validators.Add(new BrokenAttributeValidator(result.MatchedType, ValidationKind.Member, "Creating instance of validator '" + result.MatchedType.GetNiceName() + "' failed with exception: " + ex.ToString()));
                }
            }

            return validators.ToArray();
        }

        //private static Dictionary<Type, Validator> CachedValidators = new Dictionary<Type, Validator>(FastTypeComparer.Instance);

        //private static Validator GetCachedValidator(Type type)
        //{
        //    Validator result;
        //    if (!CachedValidators.TryGetValue(type, out result))
        //    {
        //        result = (Validator)Activator.CreateInstance(type);
        //        CachedValidators.Add(type, result);
        //    }
        //    return result;
        //}
    }
}
#endif
#pragma warning enable