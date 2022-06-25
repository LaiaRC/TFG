using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PasiveVillager : Villager
{
    // Start is called before the first frame update
    void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = velocity;

        spawn();
    }

    // Update is called once per frame
    void Update()
    {
        move();
        scareBar.setValue(currentScarePoints); //Aqui sera el take scare
        checkNearScares();
    }
}
