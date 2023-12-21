#region Comment Head

// Author:        LiuXiYuan
// CreateDate:    2020/5/31 23:19:35
//Email:         854327817@qq.com

#endregion

namespace Yojoy.Tech.U3d.Core.Editor
{
    public interface IProjectExpander:IEditorExpander
    {
        /// <summary>
        /// 保存当前的资源id和路径信息
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="path"></param>
        void SaveContext(string guid, string path);
        /// <summary>
        /// 检测当前资源或者路径是否是扩展器自身的拓展目标
        /// </summary>
        /// <returns></returns>
        bool CheckContext();
    }
}
