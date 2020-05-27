using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Yojoy.Tech.Common.Core.Editor
{
    public class IfPreCompileBlock : IDisposable
    {
        private readonly CsharpScriptAppender appender;
        private readonly List<string> instructions;
        public IfPreCompileBlock(CsharpScriptAppender appender,
            List<string> instructions)
        {
            this.appender = appender;
            this.instructions = instructions;
            AppendIfStart();
        }
        /// <summary>
        /// 注意方法里的stringBuilder添加的实际内容
        /// 空格也会严重影响这输出的内容
        /// </summary>
        private void AppendIfStart()
        {
            for (int index = 0; index < instructions.Count; index++)
            {
                var instruct = instructions[index];
                appender.Append(index == 0
                    ? $"#if {instruct}"
                    : $" && {instruct}");
            }
            appender.AppendLine();
        }

        public void Dispose()
        {
            if (instructions == null || instructions.Count == 0)
                return;
            appender.AppendLine();
            appender.AppendLine("#endif");
        }
    }
}


