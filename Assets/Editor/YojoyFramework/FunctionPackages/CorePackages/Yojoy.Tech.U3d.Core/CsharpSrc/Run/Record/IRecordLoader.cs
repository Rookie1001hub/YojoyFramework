#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/6/8 12:01:14
//Email:         854327817@qq.com

#endregion

using System;

namespace Yojoy.Tech.U3d.Core.Run
{
    public interface IRecordLoader
    {
        object LoadRecord(Type recordType, string recordName);

        void SaveRecord(IRecord record, bool deleteExist = false);
    }
}
