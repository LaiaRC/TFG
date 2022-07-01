using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Witch : Monster
{
    public float invocationRate;
    
    protected float time;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = velocity;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        spawn();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        move();
        invokeSkeletons();
    }

    public void invokeSkeletons()
    {
        if (time >= invocationRate)
        {
            if (miniGameManager.Instance.MONSTERS.TryGetValue(miniGameManager.SKELETON, out GameObject skeleton))
            {
                GameObject aux1 = Instantiate(skeleton, (Vector2)transform.position + (Random.insideUnitCircle * range), Quaternion.identity);
                GameObject aux2 = Instantiate(skeleton, (Vector2)transform.position + (Random.insideUnitCircle * range), Quaternion.identity);

                aux1.GetComponent<Monster>().health = 2;
                aux2.GetComponent<Monster>().health = 2;
                time = 0;
            }
        }
    }
}
