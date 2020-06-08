#region Comment Head

// Author:        LiuQian1992
// CreateDate:    2020/6/8 18:03:02
//Email:         854327817@qq.com

#endregion

using Newtonsoft.Json;
using System.IO;

namespace Yojoy.Tech.U3d.Core.Editor
{
    public static  class JsonNetUtility
    {
        public static string GetBeautifiedJson(string jsonContent)
        {
            var serializer = new JsonSerializer();
            var textReader = new StringReader(jsonContent);
            var jsonReader = new JsonTextReader(textReader);
            var obj = serializer.Deserialize(jsonReader);
            if (obj == null)
            {
                return jsonContent;
            }
            var textWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(textWriter);
            jsonWriter.Formatting = Formatting.Indented;
            jsonWriter.Indentation = 4;
            jsonWriter.IndentChar = ' ';
            serializer.Serialize(jsonWriter, obj);
            return textWriter.ToString();
        }
    }
}
