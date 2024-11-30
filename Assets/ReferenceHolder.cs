using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceHolder : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject mainBoardEnv;
    public GameObject buildEnv;
    public static ReferenceHolder instance;
    void Start()
    {
        instance = this;
        
    }

}
