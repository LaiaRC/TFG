using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Reaper : Monster
{
    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = velocity;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        //Change layer to default once the monster has been instantiated
        Invoke("changeLayer", 1f);
        spawn();
    }

    // Update is called once per frame
    private void Update()
    {
        if (miniGameManager.Instance.gameOver)
        {
            gameOver();
        }
        else
        {
            move();
        }
    }

    public override void move()
    {
        if (!checkVillagersInRange())
        {
            if (agent.speed == 0)
            {
                setOriginalSpeed();
            }
            Debug.Log("END!");
            //Mostrar dialog
        }
        else
        {
            currentTarget = getVillagerInRange();
            if (currentTarget != null)
            {
                if (targetIsInAttackRange(currentTarget))
                {
                    //scare closest villager without moving
                    agent.speed = 0;

                    //Start attacking and then wait to attack again
                    attackTime += Time.deltaTime;
                    if (attackTime >= attackRate)
                    {
                        isAttacking = true;

                        //Get anim position
                        Vector3 animPosition = transform.GetChild(0).transform.position;

                        //Cast projectile
                        GameObject proj = miniGameManager.Instance.poolParticle(miniGameManager.SCARE_PROJECTILE, animPosition);
                        proj.GetComponent<Projectile>().objective = currentTarget.gameObject;
                        proj.GetComponent<Projectile>().type = "villager";
                        proj.GetComponent<Projectile>().damage = damage;

                        miniGameManager.Instance.numScares++;
                        attackTime = 0;
                    }
                }
                else
                {
                    //keep moving to villager
                    if (agent.speed == 0)
                    {
                        setOriginalSpeed();
                    }
                    agent.SetDestination(currentTarget.gameObject.transform.position);
                }
            }
        }
    }
}