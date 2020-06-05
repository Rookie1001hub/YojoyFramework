#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/5 9:35:34
//Email:         854327817@qq.com

#endregion

namespace Yojoy.Tech.U3d.Core.Run
{
    public interface IPrefsValueGeneric<TValue>
    {
        string Key { get; }
        TValue Value { get; }

        string Description { get; }

        void Init(string key, TValue value, string description);
    }
}
