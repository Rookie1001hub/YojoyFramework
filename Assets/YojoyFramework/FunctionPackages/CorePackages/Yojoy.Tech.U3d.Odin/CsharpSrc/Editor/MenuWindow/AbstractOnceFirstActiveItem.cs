#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/8 10:44:12
//Email:         854327817@qq.com

#endregion

using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.U3d.Odin.Editor
{
    public abstract class AbstractOnceFirstActiveItem : IOnActive
    {
        private  bool isActivated;
        public void OnActive()
        {
            if (isActivated)
            {
                return;
            }
            DoActive();
            isActivated = true;
        }
        protected abstract void DoActive();
    }
}
