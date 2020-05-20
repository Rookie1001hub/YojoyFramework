using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Yojoy.Tech.Common.Core.Editor
{
    public class NameSpaceBlock : IDisposable
    {
        private readonly CsharpScriptAppender appender;
        
        public NameSpaceBlock(CsharpScriptAppender appender,
            string @namespace)
        {
            this.appender = appender;
            appender.AppendLine($"namespace{@namespace}");
            appender.AppenLeftBracketAndToRight();
        }

        public void Dispose() => appender.AppendToLeftAndRightBracket();
       
    }
}

