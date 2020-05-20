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
            List<string> instruction)
        {
            this.appender = appender;
            this.instructions = instruction;
            AppendIfStart();
        }
        private void AppendIfStart()
        {
            for (int index = 0; index < instructions.Count; index++)
            {
                var instruct = instructions[index];
                appender.Append(index == 0
                    ? $"#if{instruct}"
                    : $"&&{instruct}");
            }
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


