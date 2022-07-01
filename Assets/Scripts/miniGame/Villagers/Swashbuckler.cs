using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Swashbuckler : Villager
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
        if (!isStunned)
        {
            takeAction();
            checkNearScares();
            checkIsOnLink();
        }
        scareBar.setValue(currentScarePoints); //Aqui sera el take scare        
    }

    override
    public void checkNearScares()
    {
        //only runs if someone near is scared by a monster above his level
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<Villager>() && !isScared)
            {
                if (collision.GetComponent<Villager>().isScared)
                {
                    foreach (Collider2D monster in collisions)
                    {
                        if (monster.GetComponent<Monster>())
                        {
                            if (monster.GetComponent<Monster>().level > level)
                            {
                                run(scareSpeed);
                            }
                        }
                    }
                }
            }
        }
    }

    public bool checkMonstersInRange()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<Monster>() && collision.GetComponent<Monster>().level <= level)
            {
                return true;
            }
        }
        return false;
    }

    public Transform getMonsterInRange()
    {
        //es pot cridar cada X segons x millorar rendiment i no a cada frame al update

        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, range);

        List<Collider2D> monsters = new List<Collider2D>();
        Transform closestMonster = null;

        bool onlyJackOLantern = true;

        //Check if there are jackOLanterns
        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<Monster>() && collision.GetComponent<Monster>().level <= level)
            {
                if (!collision.GetComponent<JackOLantern>()) onlyJackOLantern = false;
            }
        }

        foreach (Collider2D collision in collisions)
        {
            if (!onlyJackOLantern)
            {
                if (collision.GetComponent<Monster>() && collision.GetComponent<Monster>().level <= level && !collision.GetComponent<JackOLantern>())
                {
                    monsters.Add(collision);
                }
            }
            else
            {
                if (collision.GetComponent<Monster>() && collision.GetComponent<Monster>().level <= level)
                {
                    monsters.Add(collision);
                }
            }
        }


        if (monsters.Count > 0)
        {
            foreach (Collider2D monster in monsters)
            {
                if (closestMonster == null)
                {
                    closestMonster = monster.transform;
                }
                else
                {
                    if ((monster.transform.position - transform.position).magnitude < (closestMonster.transform.position - transform.position).magnitude)
                    {
                        closestMonster = monster.transform;
                    }
                }
            }
        }
        return closestMonster;
    }

    public void takeAction()
    {
        if (!checkMonstersInRange() || isRunning)
        {
            move();
        }
        else
        {
            Transform currentTarget = getMonsterInRange();
            if (currentTarget != null)
            {
                if ((transform.position - currentTarget.position).magnitude <= agent.stoppingDistance + 0.2f)
                {
                    if (canAttack)
                    {
                        attackTime += Time.deltaTime;
                        if (attackTime >= attackRate)
                        {
                            currentTarget.gameObject.GetComponent<Monster>().takeDamage(damage);
                            attackTime = 0;
                        }
                    }
                }
                else
                {
                    //keep moving to waypoint
                    agent.SetDestination(currentTarget.position);
                }
            }
        }
    }
}