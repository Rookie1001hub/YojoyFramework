using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Yojoy.Tech.Common.Core.Run
{
    public static class FileUtility 
    {
        /// <summary>
        /// 获取指定路径所代表的无拓展名的文件名
        /// </summary>
        /// <param name="path">指定路径</param>
        /// <returns></returns>
        public static string GetFileIdWithoutExtension(string path)
        {
            var id = Path.GetFileNameWithoutExtension(path);
            if (id == null)
            {
                throw new Exception(message:
                    $"An error occurred while getting the target file name!");
            }
            return id;
        }

        public static void WriteAllText(string path,string content)
        {
            DirectoryUtility.EnsureDirectoryExist(path);
            File.WriteAllText(path, content);
        }
    }
}

