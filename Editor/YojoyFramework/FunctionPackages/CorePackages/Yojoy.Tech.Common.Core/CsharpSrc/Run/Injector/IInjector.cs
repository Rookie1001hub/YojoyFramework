#region Comment Head


#endregion

namespace Yojoy.Tech.Common.Core.Run
{
    public interface IInjector
    {
        TTargetType Get<TTargetType>(bool useReflection)
            where TTargetType : class;
    }
}
