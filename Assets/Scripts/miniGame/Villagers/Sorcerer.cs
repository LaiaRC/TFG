using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorcerer : Villager
{
    public float teleportRate;
    public float detectionRange;
    public GameObject projectile;
    public GameObject teleportRange;

    private static int TELEPORT_RANGE = 4;
    private bool hasTeleported = false;
    private bool canTeleport = false;
    private float time = 0;

    public bool wasMovingRight = false;
    public bool isMovingRight = false;

    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
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

        if (miniGameManager.Instance.gameOver)
        {
            gameOver();
        }
        else
        {
            if (hasTeleported)
            {
                time += Time.deltaTime;
            }
            if (hasTeleported && time >= teleportRate)
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
            if (collision.GetComponent<Monster>())
            {
                if (!collision.GetComponent<JackOLantern>()) onlyJackOLantern = false;
            }
        }

        foreach (Collider2D collision in collisions)
        {
            if (!onlyJackOLantern)
            {
                if (collision.GetComponent<Monster>() && !collision.GetComponent<JackOLantern>())
                {
                    monsters.Add(collision);
                }
            }
            else
            {
                if (collision.GetComponent<Monster>())
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
                //Show teleport area
                teleportRange.SetActive(true);

                //play teleport before animation
                if (!wasMovingRight)
                {
                    //Play left animation
                    transform.GetChild(0).GetComponent<Animator>().Play("sorcerer_teleport_left_before");
                }
                else
                {
                    //Play right animation
                    transform.GetChild(0).GetComponent<Animator>().Play("sorcerer_teleport_right_before");
                }

                hasTeleported = true;

                //wait until animation finishes to actually teleport
                Invoke(nameof(teleport), 0.4f);
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
                            //play attack animation
                            if (!wasMovingRight)
                            {
                                //Play left animation
                                transform.GetChild(0).GetComponent<Animator>().Play("sorcerer_attack_left");
                            }
                            else
                            {
                                //Play right animation
                                transform.GetChild(0).GetComponent<Animator>().Play("sorcerer_attack_right");
                            }

                            //Get anim position
                            Vector3 animPosition = transform.GetChild(0).transform.position;

                            //Cast projectile
                            GameObject proj = miniGameManager.Instance.poolParticle(miniGameManager.SORCERER_PROJECTILE, animPosition);
                            proj.GetComponent<Projectile>().objective = currentTarget.gameObject;
                            proj.GetComponent<Projectile>().type = "monster";
                            proj.GetComponent<Projectile>().damage = damage;
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
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, detectionRange);
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

    public int RandomRangeExcept(int min, int max, int except)
    {
        int number = 0;
        do
        {
            number = Random.Range(min, max);
        } while (number == except);

        return number;
    }

    public void teleport()
    {
        teleportRange.gameObject.SetActive(false);

        List<GameObject> villagers = getVillagers();

        //Teleport all villagers in risk to another waypoint
        int newIndexPosition = RandomRangeExcept(0, miniGameManager.Instance.waypoints.Count, currentWaypointIndex);
        transform.position = (Vector2)miniGameManager.Instance.waypoints[newIndexPosition].position + (Random.insideUnitCircle * TELEPORT_RANGE);

        foreach (GameObject villager in villagers)
        {
            villager.transform.position = (Vector2)miniGameManager.Instance.waypoints[newIndexPosition].position + (Random.insideUnitCircle * TELEPORT_RANGE);
        }

        //Show teleport area
        teleportRange.gameObject.SetActive(true);

        //play teleport after animation
        if (!wasMovingRight)
        {
            //Play left animation
            transform.GetChild(0).GetComponent<Animator>().Play("sorcerer_teleport_left_after");
        }
        else
        {
            //Play right animation
            transform.GetChild(0).GetComponent<Animator>().Play("sorcerer_teleport_right_after");
        }

        //Hide teleport area
        Invoke(nameof(hideTeleportRange), 0.4f);
    }

    public void hideTeleportRange()
    {
        teleportRange.gameObject.SetActive(false);
    }
}