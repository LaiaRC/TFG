using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Witch : Monster
{
    public float invocationRate;
    public float invocationRange;
    
    protected float time;
    protected List<GameObject> aliveSkeletons = new List<GameObject>();

    protected static int MAX_SKELETONS = 5;

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

        //Change layer to default once the monster has been instantiated
        Invoke("changeLayer", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (miniGameManager.Instance.gameOver)
        {
            gameOver();
        }
        else
        {
            time += Time.deltaTime;
            move();
            invokeSkeletons();
        }
    }

    public void invokeSkeletons()
    {
        //Update alive skeletons        
        aliveSkeletons.RemoveAll(GameObject => GameObject == null);
        
        //Check if max alive skeletons 
        if (aliveSkeletons.Count < MAX_SKELETONS)
        {
            if (time >= invocationRate)
            {
                if (miniGameManager.Instance.MONSTERS.TryGetValue(miniGameManager.SKELETON, out GameObject skeleton))
                {
                    GameObject aux1 = Instantiate(skeleton, (Vector2)transform.position + (Random.insideUnitCircle * invocationRange), Quaternion.identity);                    
                    aux1.GetComponent<Monster>().health = 2;
                    aliveSkeletons.Add(aux1);

                    //Invoke second skeleton only if the max hasn't been reached
                    if (aliveSkeletons.Count < MAX_SKELETONS)
                    {
                        GameObject aux2 = Instantiate(skeleton, (Vector2)transform.position + (Random.insideUnitCircle * invocationRange), Quaternion.identity);
                        aux2.GetComponent<Monster>().health = 2;
                        aliveSkeletons.Add(aux2);
                    }
                    time = 0;
                }
            }
        }
    }

    override
    public void move()
    {
        if (!checkVillagersInRange())
        {
            if (agent.speed == 0)
            {
                setOriginalSpeed();
            }
            if ((transform.position - flags[currentFlag].position).magnitude <= agent.stoppingDistance + 0.2f)
            {
                //check if it's last waypoint
                if (currentFlag >= flags.Count - 1)
                {
                    //stop moving
                    currentFlag = 0;
                }
                else
                {
                    currentFlag++;
                }
            }
            else
            {
                //keep moving to waypoint
                agent.SetDestination(flags[currentFlag].position);
            }
        }
        else
        {
            //ranged attack
            currentTarget = getVillagerInRange();
            if (currentTarget != null)
            {
                agent.speed = 0;
                attackTime += Time.deltaTime;
                if (attackTime >= attackRate)
                {
                    isAttacking = true;
                    currentTarget.gameObject.GetComponent<Villager>().takeScare(damage);
                    miniGameManager.Instance.numScares++;
                    attackTime = 0;
                }
            }
        }
    }

    public void setOriginalSpeed()
    {
        agent.speed = velocity;
    }
}
