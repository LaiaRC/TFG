using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    private void Update()
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
                if (currentVillagersActive < maxVillagers)
                {
                    GameObject temp = pool.GetPooledObject();

                    Vector3 pos = ((Vector2)transform.position + (Random.insideUnitCircle * radious));

                    temp.transform.position = RandomNavmeshLocation(radious);
                    temp.SetActive(true);
                }
                else
                {
                    if (miniGameManager.Instance.hasReaper)
                    {
                        Destroy(this);
                    }
                }
            }
            time = 0;
        }
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    private bool checkDestination(Vector3 targetDestination)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetDestination, out hit, 1f, NavMesh.GetAreaFromName("Walkable")))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radious);
    }
}