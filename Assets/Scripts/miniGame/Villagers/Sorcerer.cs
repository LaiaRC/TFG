using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorcerer : Villager
{
    public float teleportRate;

    private static int TELEPORT_RANGE = 4;
    private bool hasTeleported = false;
    private bool canTeleport = false;
    private float time = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = velocity;

        spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTeleported)
        {
            time += Time.deltaTime;
        }
        if(hasTeleported && time >= teleportRate)
        {
            hasTeleported = false;
            time = 0;
        }

        if (!isStunned)
        {
            takeAction();
            checkIsOnLink();
        }

        scareBar.setValue(currentScarePoints); //Aqui sera el take scare
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
        if (checkScaredVillagers() && !hasTeleported)
        {
            if (!hasTeleported)
            {
                //area damage monsters before teleporting
                if (checkMonstersInRange())
                {
                    List<GameObject> monsters = getMonsters();
                    if (monsters != null)
                    {
                        foreach (GameObject monster in monsters)
                        {
                            monster.GetComponent<Monster>().takeDamage(damage);
                        }
                    }
                }

                List<GameObject> villagers = getVillagers();

                //Teleport all villagers in risk to another waypoint
                int newIndexPosition = RandomRangeExcept(0, miniGameManager.Instance.waypoints.Count, currentWaypointIndex);
                transform.position = (Vector2)miniGameManager.Instance.waypoints[newIndexPosition].position + (Random.insideUnitCircle * TELEPORT_RANGE);

                foreach (GameObject villager in villagers)
                {
                    villager.transform.position = (Vector2)miniGameManager.Instance.waypoints[newIndexPosition].position + (Random.insideUnitCircle * TELEPORT_RANGE);
                }

                hasTeleported = true;

                //area damage monsters after teleporting
                if (checkMonstersInRange())
                {
                    List<GameObject> monsters = getMonsters();
                    if (monsters != null)
                    {
                        foreach (GameObject monster in monsters)
                        {
                            monster.GetComponent<Monster>().takeDamage(damage);
                        }
                    }
                }
            }
        }
        else
        {
            if (!checkMonstersInRange() || isRunning)
            {
                if (agent.speed == 0)
                {
                    setOriginalSpeed();
                }
                move();
            }
            else
            {
                Transform currentTarget = getMonsterInRange();

                if (currentTarget != null)
                {
                    //attacks closest monster without moving
                    agent.speed = 0;
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
            }
        }        
    }

    public bool checkScaredVillagers()
    {
        //check if scared villagers in range
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<Villager>() && collision.GetComponent<Villager>().isScared && !isScared)
            {
                return true;
            }
        }
        return false;
    }

    public List<GameObject> getVillagers()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, range);
        List<GameObject> villagers = new List<GameObject>();

        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<Villager>() && !isScared)
            {
                villagers.Add(collision.gameObject);
            }
        }
        return villagers;
    }

    public List<GameObject> getMonsters()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, range);
        List<GameObject> monsters = new List<GameObject>();

        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<Monster>() && !isScared)
            {
                monsters.Add(collision.gameObject);
            }
        }
        return monsters;
    }

    public int RandomRangeExcept(int min, int max, int except){
        int number = 0;
        do {
            number = Random.Range(min, max);
        } while (number == except) ;

        return number;
    }
}
