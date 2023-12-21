#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/6/8 22:04:42
//Email:         854327817@qq.com

#endregion



using Sirenix.OdinInspector;

namespace Yojoy.Tech.U3d.Core.Run
{
    public abstract class AbstractRecord : IRecord
    {
        public abstract string RecordName { get; protected set; }
        
        [Button("Save anytime","保存到最新",ButtonSizes.Medium)]
        protected virtual void SaveAnyTime()
        {
            UnityRecordLoader.Instance.SaveRecord(this, true);
        }

        [Button("Tay save","尝试保存",ButtonSizes.Medium)]
        protected virtual void TrySave()
        {
            UnityRecordLoader.Instance.SaveRecord(this);
        }
    }
}
