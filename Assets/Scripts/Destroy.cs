using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public float destroyTime = 3;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this, destroyTime);
    }
}
