
#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/6/1 22:07:21
//Email:         854327817@qq.com

#endregion


using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.U3d.Core.Run
{
    
    /// <summary>
    /// 加载资源时无需关心传过来的路径到底是什么
    /// 模拟Unity下的资源加载行为（Resources,StreamingAseets,沙盒）
    /// </summary>
    public static  class AssetDatabaseUtility
    {
        private static readonly MethodInfo loadAssetAtPathMethod;
        private static readonly MethodInfo loadAllAssetsAtPathMethod;
        private static readonly MethodInfo createAssetMethod;
        private static readonly MethodInfo refreshMethod;

        static AssetDatabaseUtility()
        {
            var editorAssembly = Assembly.Load("UnityEditor");
            var assetDatabaseType = editorAssembly.GetType(
                "UnityEditor.AssetDatabase");
            var methods = assetDatabaseType.GetMethods().ToList();
            loadAssetAtPathMethod = methods.Find(
                m=>m.Name=="LoadAssetAtPath");
            loadAllAssetsAtPathMethod = methods.Find(
                m => m.Name == "LoadAllAssetsAtPath");
            createAssetMethod = methods.Find(m
                =>m.Name=="CreateAsset");
            refreshMethod = methods.Find(
                m=>m.Name=="Refresh");
        }
        #region Path
        private static string GetFullPath(string path)
        {
            var fullPath = Application.dataPath.Replace("Assets", "")
                + path;
            return fullPath;
        }
        public static string GetAssetsPath(string fullPath)
        {
            if (fullPath.StartsWith("Assets/"))
            {
                return fullPath;
            }
            var path = "Assets" + fullPath.Replace(Application.dataPath, "");
            return path;
        }
        #endregion
        #region Asset Load
        public static Object LoadAssetPath(string path,Type type)
        {
            if (path.Contains(Application.dataPath))
            {
                path = GetAssetsPath(path);
            }
            var args = new object[] { path, type };
            var asset = (Object)loadAssetAtPathMethod.Invoke(null, args);
            return asset;
        }
        public static TAsset LoadAssetAtPath<TAsset>(string path)
            where TAsset : Object
        {
            var asset = (TAsset)LoadAssetPath(path, typeof(TAsset));
            return asset;
        }
        public static List<TAsset> LoadAllAssetsAtPath<TAsset>
            (string path) where TAsset : Object
        {
            var assetsPath = GetAssetsPath(path);
            var args = new object[] { assetsPath };
            var objects = (object[])loadAllAssetsAtPathMethod
                .Invoke(null, args);
            var assets = objects.OfType<TAsset>().ToList();
            return assets;
        }
        public static void CreateAsset(Object asset,string path
            ,bool isDeleteExist = false)
        {
            var fullPath = GetFullPath(path);
            if (File.Exists(fullPath) && isDeleteExist)
            {
                File.Delete(fullPath);
                Refresh();
            }
            DirectoryUtility.EnsureDirectoryExist(fullPath);
            var assetsPath = GetAssetsPath(fullPath);
            var args = new object[] { asset, assetsPath };
            createAssetMethod.Invoke(null, args);
        }
        #endregion
        #region Refresh
        private static readonly object[] refreshArgs = { null };
        private static void Refresh() =>
            refreshMethod.Invoke(null, refreshArgs);
        #endregion
    }

}


