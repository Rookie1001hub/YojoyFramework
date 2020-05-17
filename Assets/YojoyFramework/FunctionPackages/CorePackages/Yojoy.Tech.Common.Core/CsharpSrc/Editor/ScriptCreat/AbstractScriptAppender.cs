using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Yojoy.Tech.Common.Core.Editor
{
    public class AbstractScriptAppender
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();

        private int indentationWidth;

        #region BaseAPI
        public void Clean() => stringBuilder.Clear();

        public override string ToString()
        {
            var content = stringBuilder.ToString();
            var bytes = Encoding.Default.GetBytes(content);
            var utf8String = Encoding.UTF8.GetString(Encoding.
                Convert(Encoding.Default, Encoding.UTF8, bytes));
            return utf8String;
        }
        public void AppendIndentation(int indentationWidth = -1)
        {
            var finalIndentation = indentationWidth == -1
                ? this.indentationWidth : indentationWidth;
            for (int i = 0; i < finalIndentation; i++)
            {
                stringBuilder.Append(" ");
            }
        }
        public void Append(string content) => stringBuilder.Append(content);

        public void AppendLine(string content)
        {
            AppendIndentation();
            stringBuilder.Append(content);
        }
        public void AppendLine()
        {
            AppendIndentation();
            stringBuilder.AppendLine();
        }
        public void AppendFormatLine(string content,params object[] args)
        {
            AppendIndentation();
            stringBuilder.AppendFormat(content, args);
            stringBuilder.AppendLine();
        }
        public void AppendPrecompileInstruction(params string[] instructions)
        {
            for (int index = 0; index < instructions.Length; index++)
            {
                var args = instructions[index];
                Append(index == 0 ? $"#if{args}" : $"&&{args}");
            }
            AppendLine();
        }
        #endregion

        #region Layout
        public void ToRight() => indentationWidth += 4;
        public void ToLeft()
        {
            var afterMoveIndex = indentationWidth - 4;
            if (afterMoveIndex < 0)
                return;
            indentationWidth = afterMoveIndex;
        }

        public void AppenLeftBracketAndToRight()
        {
            AppendLine("{");
            ToRight();
        }
        public void AppendToLeftAndRightBracket()
        {
            ToLeft();
            AppendLine("}");
        }
        #endregion
    }
}
