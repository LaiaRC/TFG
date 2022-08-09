using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mom : Villager
{
    public float childDetectionRange;

    protected bool isProtecting = false;
    protected Vector3 childPosition = Vector3.zero; //avoid that mom never gets to target because child is dead

    public bool wasMovingRight = false;
    public bool isMovingRight = false;

    public bool isGoingToChild = false;
    public Transform currentTarget;
    public Vector3 currentTargetDEBUG;

    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = velocity;

        spawn();
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

        if (currentTarget != null && (transform.position - currentTarget.position).magnitude <= agent.stoppingDistance + 0.5f)
        {
            currentTarget = null;
            isGoingToChild = false;
        }

        if (miniGameManager.Instance.gameOver)
        {
            gameOver();
        }
        else
        {
            if (!isStunned)
            {
                takeAction();
                checkNearScares();
                checkIsOnLink();
            }
            scareBar.setValue(currentScarePoints); //Aqui sera el take scare
        }
    }

    public override void checkNearScares()
    {
        //only runs if someone near is scared by a monster above level 1
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

    public bool checkScaredChild()
    {
        //check if scared child in range
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, childDetectionRange);
        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<PasiveVillager>() && collision.GetComponent<PasiveVillager>().type.Equals("child") && collision.GetComponent<PasiveVillager>().isScared)
            {
                isProtecting = true;
                return true;
            }
        }
        return false;
    }

    public Transform getScaredChild()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, childDetectionRange);

        List<Collider2D> childs = new List<Collider2D>();
        Transform closestChild = null;

        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<PasiveVillager>() && collision.GetComponent<PasiveVillager>().type.Equals("child") && collision.GetComponent<PasiveVillager>().isScared)
            {
                childs.Add(collision);
            }
        }

        if (childs.Count > 0)
        {
            foreach (Collider2D child in childs)
            {
                if (closestChild == null)
                {
                    closestChild = child.transform;
                }
                else
                {
                    if ((child.transform.position - transform.position).magnitude < (closestChild.transform.position - transform.position).magnitude)
                    {
                        closestChild = child.transform;
                    }
                }
            }
        }
        return closestChild;
    }

    public bool checkMonstersInRange()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<Monster>()) //&& collision.GetComponent<Monster>().level <= level
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
                if (collision.GetComponent<Monster>() && collision.GetComponent<Monster>().level <= level)
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
        if (checkScaredChild() && !isGoingToChild)
        {
            currentTarget = getScaredChild();
            runToChild(runningSpeed);
            isProtecting = true;
            isGoingToChild = true;
            currentTargetDEBUG = currentTarget.position;
            Invoke("cancelTarget", 15f);
            if (currentTarget != null) agent.SetDestination(currentTarget.position);
        }
        else
        {
            if (isProtecting)
            {
                setOriginalSpeed();
                isProtecting = false;
            }
            if (!checkMonstersInRange() || isRunning)
            {
                move();
            }
            else
            {
                Transform currentTarget = getMonsterInRange();
                if (currentTarget != null)
                {
                    if ((transform.position - currentTarget.position).magnitude <= range)
                    {
                        if (canAttack)
                        {
                            attackTime += Time.deltaTime;
                            if (attackTime >= attackRate)
                            {
                                //play attack animation
                                if (!wasMovingRight)
                                {
                                    //Play left animation
                                    transform.GetChild(0).GetComponent<Animator>().Play("mom_attack_left");
                                }
                                else
                                {
                                    //Play right animation
                                    transform.GetChild(0).GetComponent<Animator>().Play("mom_attack_right");
                                }
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

    private void cancelTarget()
    {
        currentTarget = null;
        isGoingToChild = false;
    }

    public void runToChild(float speed)
    {
        if (canMove)
        {
            isRunning = true;
            agent.speed = speed;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, childDetectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}