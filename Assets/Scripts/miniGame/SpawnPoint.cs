using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject villager;
    public int maxVillagers;
    public float radious;
    public float spawnRate;
    public int initialVillagers;

    private int currentVillagersActive = 0;
    private float time;

    private void Start()
    {
        ObjectPool pool = new ObjectPool();
        pool.objectToPool = villager;
        pool.amountToPool = initialVillagers;
        pool.setup();
        miniGameManager.Instance.POOLS.Add(villager.GetComponent<Villager>().type, pool);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= spawnRate)
        {
            if (miniGameManager.Instance.POOLS.TryGetValue(villager.GetComponent<Villager>().type, out ObjectPool pool))
            {
                currentVillagersActive = 0;
                foreach (GameObject go in pool.pooledObjects)
                {
                    if (go.activeInHierarchy)
                    {
                        currentVillagersActive++;
                    }
                }
                if(currentVillagersActive < maxVillagers)
                {
                    GameObject temp = pool.GetPooledObject();
                    temp.SetActive(true);
                    temp.transform.position = ((Vector2)transform.position + (Random.insideUnitCircle * radious));
                }
            }
            time = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radious);
    }
}
