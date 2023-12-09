using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Yojoy.Tech.Common.Core.Editor;
using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.Common.Core.Editor
{
    public class CsharpScriptAppender : AbstractScriptAppender
    {
        public void AppendSingleComment(string comment)
            => AppendLine($"//{comment}");
        /// <summary>
        /// 添加多行文字内容
        /// </summary>
        /// <param name="comments"></param>
        public void AppendMultiComment(params string[] comments)
        {
            AppendLine("#region Copyright");
            foreach (var item in comments)
            {
                AppendLine(item);
            }
            AppendLine("#endregion");
            AppendLine();
        }

        public void AppendUsingNameSpace(params string[] nameSpaces)
        {
            foreach (var item in nameSpaces)
            {
                AppendLine($"using{item};");
            }
            AppendLine();
        }
        public void AppendComment(
            string bodyComment,
            List<string> paramNames = null,
            List<string> paramComments = null
            )
        {
            AppendLine("/// <summary>");
            AppendLine("///" + bodyComment);
            AppendLine("/// </summary>");

            if (paramNames == null)
            {
                return;
            }
            if (paramComments == null)
            {
                foreach (var item in paramNames)
                {
                    AppendLine("/// <param name=\"" +
                        item + "\"></param>");
                }
            }
            else
            {
                for (int index = 0; index < paramNames.Count; index++)
                {
                    var name = paramNames[index];
                    var comment = paramComments[index];
                    AppendFormatLine("/// <param name=\"{0}\">{1}</param>",
                        name,comment);
                }
            }
        }
        /// <summary>
        /// 添加头文件 作者信息
        /// </summary>
        /// <param name="coderName"></param>
        /// <param name="coderEmail"></param>
        public void AppendCommentHeader(string coderName, string coderEmail)
        {
            AppendLine("#region Comment Head");
            AppendLine();
            AppendLine($"// Author:{coderName}");
            AppendLine(
                $"// Date:{ CommonGlobalUtility.NowDateString()}");
            AppendLine($"// Email:{coderEmail}");
            AppendLine();
            AppendLine("#endregion");
            AppendLine();
        }
        public void AppendFooter()
        {
            ToLeft();
            AppendLine("}");
            ToLeft();
            AppendLine("}");
            AppendLine();
        }
    }
   
}
