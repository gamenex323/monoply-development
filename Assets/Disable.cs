using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disable : MonoBehaviour
{
    // Start is called before the first frame update
    public float falseTime = 2f;
    public GameObject objectToFalse;
    void Start()
    {
        Invoke(nameof(DisableThis), falseTime);
    }

    // Update is called once per frame
    void DisableThis()
    {
        if (objectToFalse)
            objectToFalse.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}
