#region Copyright
//------------------------------------------------------------
// Copyright © 2018-2024 LiuXiYuan. All rights reserved.
// Homepage: 
// Feedback: 
//------------------------------------------------------------
#endregion

#region Comment Head

// Author:LiuXiYuan
// Date:2024/7/13 14:37:39
// Email:854327817@qq.com

#endregion


using UnityEngine;

[CreateAssetMenu(fileName = "YojoyFolderSettins", menuName = "YojoySampleScriptableObject/Create YojoyFolderSettins")]
public class YojoyFolderSettins : ScriptableObject
{
    [Header("Yojoy工具所在文件夹,Yojoyu作为编辑器工具一般在Editor目录下")]
    public string YojoyToolsFolder ;
    [Header("Yojoy工具的目录名称")]
    public string YojoyParentDirectoyId ;
    
}
