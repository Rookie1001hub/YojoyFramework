

#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/9 11:34:38
//Email:         854327817@qq.com

#endregion


using System;

namespace Yojoy.Tech.Common.Core.Run
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultInjectAttribute:Attribute
    {
        public Type TargetType { get; }

        public DefaultInjectAttribute(Type targetType) => TargetType = targetType;
      
    }
}


