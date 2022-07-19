using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Requirement : MonoBehaviour
{
    public string resourceNameKey;
    public int quantity;

    public Requirement(string resourceNameKey, int quantity)
    {
        this.resourceNameKey = resourceNameKey;
        this.quantity = quantity;
    }

    public Requirement() {}
}

