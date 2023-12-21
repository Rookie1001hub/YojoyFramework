
#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/6/5 17:48:20
//Email:         854327817@qq.com

#endregion


using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Yojoy.Tech.Common.Core.Run
{
    public static class SerializeUtility
    {
        /// <summary>
        ///序列化对象产生二进制文件
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static byte[] Serialize(object instance)
        {
            if (instance == null)
            {
                return null;
            }
            using (var ms=new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, instance);
                var bytes = new byte[ms.Length];
                Buffer.BlockCopy(src: ms.GetBuffer(), 0, bytes, 0, (int)ms.Length);
                return bytes;
            }
        }
        public static T DeSerialize<T>(byte[] value)where T : class, new()
        {
            if (value == null)
            {
                return default;
            }
            using (var ms=new MemoryStream())
            {
                var bf = new BinaryFormatter();
                var instnace = (T)bf.Deserialize(ms);
                return instnace;
            }
        }
    }
}


