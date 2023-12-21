#region Comment Head



#endregion

using System;

namespace Yojoy.Tech.Common.Core.Run
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultInjectAttribute : Attribute
    {
        public Type TargetType { get; }

        public DefaultInjectAttribute(Type targetType) => TargetType = targetType;
    }
}
