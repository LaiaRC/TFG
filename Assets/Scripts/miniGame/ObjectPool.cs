using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public List<GameObject> pooledObjects = new List<GameObject>();
    public GameObject objectToPool;
    public int amountToPool;

    public void setup()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject tmp = Instantiate(objectToPool, miniGameManager.Instance.transform);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }

    public GameObject GetPooledObject()
    {
        GameObject obj = null;
        for (int i = 0; i < amountToPool; i++)
        {
            if (pooledObjects.Count > 0 && !pooledObjects[i].activeInHierarchy)
            {
                obj = pooledObjects[i];
            }
        }
        if (obj == null)
        {
            obj = Instantiate(objectToPool, miniGameManager.Instance.transform);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
        return obj;
    }
}
