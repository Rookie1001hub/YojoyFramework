#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/9 11:47:45
//Email:         854327817@qq.com

#endregion

namespace Yojoy.Tech.Common.Core.Run
{
    public interface IInjector
    {
        TTargetType Get<TTargetType>(bool useReflection)
            where TTargetType : class;
    }
}
