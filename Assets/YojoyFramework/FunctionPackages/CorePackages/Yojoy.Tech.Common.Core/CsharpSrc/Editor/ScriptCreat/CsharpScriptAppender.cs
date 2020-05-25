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
        public void AppendMultiComment(params string[] comments)
        {
            AppendLine("/*");
            foreach (var item in comments)
            {
                AppendLine("*0" + item);
            }
            AppendLine("*/");
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
            AppendLine($"// Author:         {coderName}");
            AppendLine(
                $"// CreateDate:       { CommonGlobalUtility.NowDateString()}");
            AppendLine($"//Email:       {coderEmail}");
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
