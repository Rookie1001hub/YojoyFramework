using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomAttr;
using Yojoy.Tech.Common.Core.Run;

public class MyTestClasee
{
    
}
public class Test : MonoBehaviour
{
    string Abc = "哎Abc";
    // Start is called before the first frame update
    void Start()
    {

        Debug.Log(Abc.EveryToBig());
       //MyAttributeAttribute myAttributeAttribute= typeof(MyTestClasee).GetAttribute<MyAttributeAttribute>();
   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
