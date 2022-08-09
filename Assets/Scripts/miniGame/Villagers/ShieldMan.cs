using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShieldMan : Villager
{
    public NavMeshObstacle navMeshObstacle;
    public float blockingTime;
    public bool isBlocking = false;
    public bool isPermanentBlocking = false;
    public List<OffMeshLink> links;
    public CircleCollider2D collider;

    protected List<Transform> flags = new List<Transform>();
    protected Vector3 destination = Vector3.zero;
    protected float radius = 5f;
    protected int destinationFlag = 0;
    protected float time = 0;
    protected float timeStopped = 0;

    public bool wasMovingRight = false;
    public bool isMovingRight = false;

    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = velocity;
        //agent.agentTypeID = AgentTypeID.GetAgenTypeIDByName(element.ElementName);

        spawn();

        for (int i = 0; i < miniGameManager.Instance.activeFlags.Count; i++)
        {
            flags.Add(miniGameManager.Instance.activeFlags[i].GetComponent<Transform>());
        }

        destinationFlag = Random.Range(0, flags.Count);
        destination = ((Vector2)flags[destinationFlag].position + (Random.insideUnitCircle * radius));
        destination.z = 0;

        agent.stoppingDistance = 4;
        collider = transform.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        //Update direction
        isMovingRight = transform.GetComponent<CharacterMovement>().movingRight;
        if (agent.desiredVelocity.sqrMagnitude > 0)
        {
            //is moving
            wasMovingRight = isMovingRight;
        }

        if (miniGameManager.Instance.gameOver)
        {
            gameOver();
        }
        else
        {
            time += Time.deltaTime;
            if (!isBlocking && !isPermanentBlocking && !isStunned)
            {
                takeAction();
            }
            else if (!isPermanentBlocking && !isStunned)
            {
                //attacks while it's blocking
                attack();
            }

            if (!isStunned)
            {
                //Is not moving
                if (agent.velocity.magnitude < 0.15f)
                {
                    timeStopped += Time.deltaTime;
                }
                else
                {
                    timeStopped = 0;
                }

                //check that permanentBlock applies
                if (!isPermanentBlocking && !isBlocking && timeStopped > 15)
                {
                    permanentBlock();
                }
            }
            else
            {
                //If is stunned reset timeStopeed (it doesn't count)
                timeStopped = 0;
            }

            //Play block animation if it's blocking
            if (isBlocking || isPermanentBlocking)
            {
                playBlockAnimation();
            }

            scareBar.setValue(currentScarePoints); //Aqui sera el take
        }
    }

    public bool checkMonstersInRange()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<Monster>())
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

    public void goToFlag()
    {
        if ((transform.position - destination).magnitude <= range)
        {
            //Once arrived to the chosen flag block permanently
            permanentBlock();
        }
        else
        {
            //keep moving to waypoint
            agent.SetDestination(flags[destinationFlag].position);
        }
    }

    public void takeAction()
    {
        if (!checkMonstersInRange())
        {
            if (time <= 30)
            {
                move();
            }
            else
            {
                goToFlag();
            }
        }
        else
        {
            //start running to monster
            run();
            Transform currentTarget = getMonsterInRange();
            if (currentTarget != null)
            {
                if ((transform.position - currentTarget.position).magnitude <= agent.stoppingDistance + 0.2f)
                {
                    block();
                }
                else
                {
                    //keep moving to waypoint
                    agent.SetDestination(currentTarget.position);
                }
            }
        }
    }

    //Only attacks while it's blocking
    public void attack()
    {
        if (canAttack)
        {
            if (checkMonstersInRange())
            {
                Transform currentTarget = getMonsterInRange();
                if (currentTarget != null)
                {
                    if ((transform.position - currentTarget.position).magnitude <= 5 + 0.2f)
                    {
                        attackTime += Time.deltaTime;
                        if (attackTime >= attackRate)
                        {
                            //play attack animation
                            if (!wasMovingRight)
                            {
                                //Play left animation
                                for (int i = 0; i < transform.GetChild(0).childCount; i++)
                                {
                                    transform.GetChild(0).GetChild(i).GetComponent<Animator>().Play("shieldMan_attack_left");
                                }
                            }
                            else
                            {
                                //Play right animation
                                for (int i = 0; i < transform.GetChild(0).childCount; i++)
                                {
                                    transform.GetChild(0).GetChild(i).GetComponent<Animator>().Play("shieldMan_attack_right");
                                }
                            }

                            currentTarget.gameObject.GetComponent<Monster>().takeDamage(damage);
                            attackTime = 0;
                        }
                    }
                }
            }
        }
    }

    public void run()
    {
        isRunning = true;
        agent.speed = runningSpeed;
        //Invoke("setOriginalSpeed", runningTime);
    }

    public void block()
    {
        isBlocking = true;
        agent.speed = 0;
        isRunning = false;
        agent.enabled = false;
        navMeshObstacle.enabled = true;
        for (int i = 0; i < links.Count; i++)
        {
            links[i].enabled = true;
        }
        collider.radius = 2.5f;
        Invoke("stopBlocking", blockingTime);
    }

    public void permanentBlock()
    {
        isPermanentBlocking = true;
        agent.speed = 0;
        isRunning = false;
        agent.enabled = false;
        navMeshObstacle.enabled = true;
        for (int i = 0; i < links.Count; i++)
        {
            links[i].enabled = true;
        }
        collider.radius = 2.5f;
    }

    public void stopBlocking()
    {
        navMeshObstacle.enabled = false;
        for (int i = 0; i < links.Count; i++)
        {
            links[i].enabled = false;
        }
        collider.radius = 0.5f;
        Invoke("enableAgent", 0.5f);
    }

    public void enableAgent()
    {
        agent.enabled = true;
        agent.speed = velocity;
        isBlocking = false;
    }

    public void playBlockAnimation()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            float nTime = transform.GetChild(0).GetChild(i).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (nTime > 1.0f)
            {
                //animation has finished
                //play block animation
                if (!wasMovingRight)
                {
                    //Play left animation
                    transform.GetChild(0).GetChild(i).GetComponent<Animator>().Play("shieldMan_block_left");
                }
                else
                {
                    //Play right animation
                    transform.GetChild(0).GetChild(i).GetComponent<Animator>().Play("shieldMan_block_right");
                }
            }
        }
    }
}