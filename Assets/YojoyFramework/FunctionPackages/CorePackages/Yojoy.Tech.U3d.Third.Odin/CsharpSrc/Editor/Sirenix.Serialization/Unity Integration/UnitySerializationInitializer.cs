#pragma warning disable
//-----------------------------------------------------------------------\n// <copyright file="UnitySerializationInitializer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Serialization
{
    using UnityEngine;

    /// <summary>
    /// Utility class which initializes the Sirenix serialization system to be compatible with Unity.
    /// </summary>
    public static class UnitySerializationInitializer
    {
        private static readonly object LOCK = new object();
        private static bool initialized = false;

        /// <summary>
        /// Initializes the Sirenix serialization system to be compatible with Unity.
        /// </summary>
        private static void Initialize()
        {
            if (!initialized)
            {
                lock (LOCK)
                {
                    if (!initialized)
                    {
                        // Ensure that the config instance is loaded before deserialization of anything occurs.
                        // If we try to load it during deserialization, Unity will throw exceptions, as a lot of
                        // the Unity API is disallowed during serialization and deserialization.
                        GlobalSerializationConfig.LoadInstanceIfAssetExists();
                        initialized = true;
                    }
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeRuntime()
        {
            Initialize();
        }

#if UNITY_EDITOR

        [UnityEditor.InitializeOnLoadMethod]
#endif
        private static void InitializeEditor()
        {
            Initialize();
        }
    }
}
#pragma warning enable