#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/6/8 12:01:14
//Email:         854327817@qq.com

#endregion

using System;

namespace Yojoy.Tech.U3d.Core.Run
{
    [AttributeUsage(validOn:AttributeTargets.Class)]
    public class RecordAttribute:Attribute
    {
        public RecordNumberType RecordNumberType { get; }
        public RecordScopeType RecordScopeType { get; }

        public RecordAttribute(RecordNumberType recordNumberType,RecordScopeType recordScopeType)
        {
            RecordNumberType = recordNumberType;
            RecordScopeType = recordScopeType;
        }
    }
}
