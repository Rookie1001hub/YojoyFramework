#pragma warning disable
#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="DictionaryExamples.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using UnityEngine;
    using System.Collections.Generic;

    [ShowOdinSerializedPropertiesInInspector]
    [AttributeExample(typeof(DictionaryDrawerSettings))]
    internal class DictionaryExamples
    {
        [InfoBox("In order to serialize dictionaries, all we need to do is to inherit our class from SerializedMonoBehaviour.")]
        public Dictionary<int, Material> IntMaterialLookup = new Dictionary<int, Material>()
        {
            { 1, ExampleHelper.GetMaterial() },
            { 7, ExampleHelper.GetMaterial() },
        };

        public Dictionary<string, string> StringStringDictionary = new Dictionary<string, string>()
        {
            { "One", ExampleHelper.GetString() },
            { "Syv", ExampleHelper.GetString() },
        };

        [DictionaryDrawerSettings(KeyLabel = "Custom Key Name", ValueLabel = "Custom Value Label")]
        public Dictionary<SomeEnum, MyCustomType> CustomLabels = new Dictionary<SomeEnum, MyCustomType>()
        {
            { SomeEnum.First, new MyCustomType() },
            { SomeEnum.Second, new MyCustomType() },
        };

        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        public Dictionary<string, List<int>> StringListDictionary = new Dictionary<string, List<int>>()
        {
            { "Numbers", new List<int>(){ 1, 2, 3, 4, } },
        };

        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
        public Dictionary<SomeEnum, MyCustomType> EnumObjectLookup = new Dictionary<SomeEnum, MyCustomType>()
        {
            { SomeEnum.Third, new MyCustomType() },
            { SomeEnum.Fourth, new MyCustomType() },
        };

        [InlineProperty(LabelWidth = 90)]
        public struct MyCustomType
        {
            public int SomeMember;
            public GameObject SomePrefab;
        }

        public enum SomeEnum
        {
            First, Second, Third, Fourth, AndSoOn
        }
    }
}
#endif
#pragma warning enable