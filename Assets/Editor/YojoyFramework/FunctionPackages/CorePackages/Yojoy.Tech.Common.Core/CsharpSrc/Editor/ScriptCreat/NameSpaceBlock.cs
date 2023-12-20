using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Yojoy.Tech.Common.Core.Editor
{
    public class NameSpaceBlock : IDisposable
    {
        private readonly CsharpScriptAppender appender;
        private string nameSpcace;
        public NameSpaceBlock(CsharpScriptAppender appender,
            string @namespace)
        {
            nameSpcace = @namespace;
            if (!string.IsNullOrEmpty(nameSpcace))
            {
                this.appender = appender;
                appender.AppendLine($"namespace {@namespace}");
                appender.AppenLeftBracketAndToRight();
            }
        }

        public void Dispose()
        {
            if (!string.IsNullOrEmpty(nameSpcace))
                appender.AppendToLeftAndRightBracket();
        }

    }
}

