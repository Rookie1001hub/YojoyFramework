using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yojoy.Tech.Common.Core.Run;
public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DirectoryUtility.EnsureDirectoryExist("Assets/Res/Sprites");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
