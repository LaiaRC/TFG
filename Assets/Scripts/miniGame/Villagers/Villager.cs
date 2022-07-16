using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioSource))]
public class Villager : MonoBehaviour
{
    public float velocity;
    public int maxScarePoints; //"health"
    public int currentScarePoints; 
    public int damage;
    public int level;
    public float range;
    public string currentState;
    public bool canAttack;
    public bool canMove = true;
    public bool isAttacking;
    public bool isScared;
    public string type; //type of villager
    public float runningTime;
    public float runningSpeed;
    public float scareSpeed; //speed when someone near is scared
    public float attackRate;
    public bool isStunned = false;
    public GameObject drop; //prefab
    public int dropQuantity; //In case there are upgrades

    public Animator animator;
    public AudioSource audioSource;
    public AudioSource audioSourceAux;
    public AudioClip[] sounds;
    public HealthBar scareBar;
    public GameObject deathParticles;
    public GameObject spawnParticles;
    public Color fillColor;
    public Color backgroundColor;

    public RectTransform canvas;

    public NavMeshAgent agent;
    protected float attackTime = 0;
    protected bool isRunning = false;
    protected bool isOnLink = false;
    protected float linkSpeed = 0.1f;

    protected List<Transform> route = new List<Transform>();
    protected int currentWaypointIndex;

    //Variables x els sons de l'array sounds
    protected static int ATTACK = 0;
    protected static int DEFAULT = 1;
    protected static int SPAWN = 2;
    protected static int TAKE_SCARE = 3;
    protected static int DIE = 4;
    protected static int WALK = 5;

    //Variables x les animacions
    protected static string DIE_ANIM = "die";

    //Waypoints variables
    protected static int NUM_WAYPOINTS = 10;

    //Time Scared
    protected static float TIME_SCARED = 1f;

    public void gameOver()
    {
        gameObject.SetActive(false);
        scareBar.gameObject.SetActive(false);
    }

    public void die()
    {
        //Add drop to dictionary
        if (miniGameManager.Instance.DROPS.TryGetValue(drop.name, out Drop dropClass))
        {
            Drop newDrop = new Drop();
            newDrop.quantity = dropClass.quantity + dropQuantity;
            newDrop.icon = dropClass.icon;
            newDrop.level = level;
            miniGameManager.Instance.DROPS[drop.name] =  newDrop;
        }
        else
        {
            //Add for the first time
            Drop newDrop = new Drop();
            newDrop.quantity = dropQuantity;
            newDrop.icon = drop.GetComponent<SpriteRenderer>().sprite;
            newDrop.level = level;
            miniGameManager.Instance.DROPS.Add(drop.name, newDrop);
        }
        
        audioSource.clip = sounds[DIE];
        audioSource.Play();
        //Instantiate(deathParticles, transform.position, Quaternion.identity);
        GameObject deathParticles = miniGameManager.Instance.poolParticle(miniGameManager.VILLAGER_DEATH_PARTICLES, transform.position);
        changeAnimationState(DIE_ANIM);
        scareBar.setValue(0);
        currentScarePoints = 0;
        gameObject.SetActive(false); //S'haura d'esperar a que acabi la anim de die
        scareBar.gameObject.SetActive(false);                                 
    }

    public void spawn()
    {
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        audioSource.clip = sounds[SPAWN];
        audioSource.Play();
        Instantiate(spawnParticles, transform.position, Quaternion.identity);
        canMove = true;

        //Create villager route 
        if (miniGameManager.Instance.waypoints != null)
        {
            for (int i = 0; i < NUM_WAYPOINTS; i++)
            {
                route.Add(miniGameManager.Instance.waypoints[Random.Range(0, miniGameManager.Instance.waypoints.Count)]);
            }
        }

        currentWaypointIndex = 0;

        agent.stoppingDistance = 3;

        scareBar = Instantiate(scareBar, canvas);
        scareBar.setTarget(transform);
        scareBar.setMaxValue(maxScarePoints);
        scareBar.setMinValue(0);
        scareBar.setValue(currentScarePoints);
        scareBar.targetCanvas = canvas;
        scareBar.setColor(fillColor, backgroundColor);
        scareBar.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        scareBar.gameObject.SetActive(true);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public void changeAnimationState(string state)
    {
        if (currentState == state) return;

        animator.Play(state);

        currentState = state;
    }

    public void takeScare(int scareValue)
    {
        currentScarePoints += scareValue;
        scareBar.setValue(currentScarePoints);
        isScared = true;
        Invoke("resetIsScared", TIME_SCARED);

        //not run when scared, only nearby villagers run
        /*if (canMove)
        {
            run(runningSpeed);
        }*/
        if (currentScarePoints >= maxScarePoints)
        {
            die();
        }
        else
        {
            //Play hit animation
            animator.Play("scare_projectile_hit");
        }
    }

    public void resetIsScared()
    {
        isScared = false;
    }

    public virtual void run(float speed)
    {
        if (canMove)
        {
            isRunning = true;
            agent.speed = speed;
            Invoke("setOriginalSpeed", runningTime);
        }
    }
    public void setOriginalSpeed()
    {
        agent.speed = velocity;
        isRunning = false;
    }

    public virtual void checkNearScares()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<Villager>() && !isScared)
            {
                if (collision.GetComponent<Villager>().isScared)
                {
                    if (canMove)
                    {
                        run(scareSpeed);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public virtual void move()
    {
        if (miniGameManager.Instance.waypoints != null && canMove)
        {
            if ((transform.position - route[currentWaypointIndex].position).magnitude <= agent.stoppingDistance + 0.2f)
            {
                //check if it's last waypoint
                if (currentWaypointIndex >= route.Count - 1)
                {
                    //set new villager route
                    route.Clear();
                    for (int i = 0; i < NUM_WAYPOINTS; i++)
                    {
                        route.Add(miniGameManager.Instance.waypoints[Random.Range(0, miniGameManager.Instance.waypoints.Count)]);
                    }
                    currentWaypointIndex = 0;
                }
                else
                {
                    currentWaypointIndex++;
                }
            }
            else
            {
                //keep moving to waypoint
                agent.SetDestination(route[currentWaypointIndex].position);
            }
        }
    }

    public void checkIsOnLink()
    {
        //check if it's on offmesh link
        if (agent.isOnOffMeshLink && isOnLink == false)
        {
            isOnLink = true;
            agent.speed = agent.speed * linkSpeed;
        }
        else if (agent.isOnNavMesh && isOnLink == true)
        {
            isOnLink = false;
            agent.velocity = Vector3.zero;
            agent.speed = velocity;
        }
    }
}
