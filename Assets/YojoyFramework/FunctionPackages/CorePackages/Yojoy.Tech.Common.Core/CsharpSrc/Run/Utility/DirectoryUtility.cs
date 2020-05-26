using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Yojoy.Tech.Common.Core.Run
{
    public static class DirectoryUtility
    {
        public static List<string> GetAllFirstSonDirectory(string directory,
            Func<string, bool> func = null)
        {
            if (string.IsNullOrEmpty(directory))
                return null;
            var directories = new List<string>();
            var sonDirectories = Directory.GetDirectories(directory).ToList();

            foreach (var item in sonDirectories)
            {
                var tempDirectory = item.EnsureDirectoryFormat();
                tempDirectory = tempDirectory.Replace(oldValue: "\\", newValue: "/");
                directories.Add(tempDirectory);
            }
            if (func != null)
            {
                directories = directories.Where(func).ToList();
            }
            return directories;
        }

        public static string GetDirectoryName(string directory)
        {
            if (directory.EndsWith("/"))
            {
                directory = directory.Substring(startIndex: 0, length: directory.Length - 1);
            }
            var array = directory.Split('/');
            var directoryName = array.Last();
            return directoryName;
        }
        public static void TryCreateDirectory(string targetDirectory, bool isHide = false)
        {
            if (Directory.Exists(targetDirectory))
            {
                return;
            }
            Directory.CreateDirectory(targetDirectory);
            if (isHide)
            {
                File.SetAttributes(targetDirectory, FileAttributes.Hidden);
            }
        }
        /// <summary>
        /// 确保目标目录存在
        /// </summary>
        /// <param name="targetDiorectory"></param>
        public static void EnsureDirectoryExist(string targetDiorectory)
        {
            var lastIndex = targetDiorectory.LastIndexOf(value: "/", StringComparison.Ordinal);
            var lastDirectory = targetDiorectory.Substring(startIndex: 0, lastIndex);
            TryCreateDirectory(lastDirectory);
        }
        public static void GetAllDiretories(string directory, List<string> directories)
        {
            if (string.IsNullOrEmpty(directory) || !File.Exists(directory))
                return;
            if (Directory.GetDirectories(directory).Length == 0)
                return;
            var sonDirectories = Directory.GetDirectories(directory);
            foreach (var item in sonDirectories)
            {
                var tempDirectory = item.EnsureDirectoryFormat();
                tempDirectory = tempDirectory.Replace("\\", "/");
                directories.Add(tempDirectory);
                GetAllDiretories(tempDirectory, directories);
            }
        }
        public static List<string> GetAllDirectoryContainSelf(string directory,
            Func<string, bool> func = null)
        {
            var directories = new List<string>();
            directories.Add(directory);
            GetAllDiretories(directory, directories);
            if (func != null)
            {
                directories = directories.Where(func).ToList();
            }
            return directories;
        }
        public static List<string> GetPathsContainSonDirectory(string directory,
            Func<string, bool> func = null, Func<string, bool> fileFilter = null)
        {
            EnsureDirectoryExist(directory);
            var directoires = GetAllDirectoryContainSelf(directory);
            var paths = new List<string>();
            foreach (var item in directoires)
            {
                var files = Directory.GetFiles(item).ToList();
                files = files.Select(p => p.Replace(oldValue: "\\", newValue: "/")).ToList();
                paths.AddRange(files);
            }
            return paths;
        }
        public static string GetFileLocDirectory(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception(message: "There are no files on the destionation path!");
            }
            var lastIndex = path.LastIndexOf('/');
            var directory = path.Substring(startIndex: 0, length: lastIndex);
            return directory.EnsureDirectoryFormat();
        }
    }
}

