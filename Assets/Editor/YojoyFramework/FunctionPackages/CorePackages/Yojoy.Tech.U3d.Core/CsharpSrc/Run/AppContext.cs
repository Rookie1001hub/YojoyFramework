#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/6/9 21:32:30
//Email:         854327817@qq.com

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.U3d.Core.Run
{
    public class AppContext : AbstractInjector
    {
        public AppContext(Action<object>debugAction,
            InjectorBuilder injectorBuilder):base(debugAction,injectorBuilder)
        {

        }
        private void Start(Action callback)
        {
            AllTypes = Assembly.GetExecutingAssembly().GetTypes().ToList();
        }

        protected override List<Type> AllTypes { get ; set ; }
    }
}
