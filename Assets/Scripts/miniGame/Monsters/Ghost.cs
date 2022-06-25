using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : Monster
{
    // Start is called before the first frame update
    void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = velocity;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        //agent.areaMask = NavMesh.GetAreaFromName("Fly");

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        spawn();
    }

    // Update is called once per frame
    void Update()
    {
        move();
        healthBar.setValue(health); //Aqui sera el take damage        
    }
}
