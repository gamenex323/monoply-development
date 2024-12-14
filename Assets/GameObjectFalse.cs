using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectFalse : MonoBehaviour
{
    // Start is called before the first frame update
    public float falseTime = 1f;
    void Enable()
    {
        Invoke(nameof(ObjectFalse), falseTime);
    }

    void ObjectFalse()
    {
        gameObject.SetActive(false);
    }
}
