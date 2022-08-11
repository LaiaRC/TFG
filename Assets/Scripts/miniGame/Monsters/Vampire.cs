using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Vampire : Monster
{
    public float healRate;
    public Sprite vampireSprite;
    public Sprite batSprite;
    public GameObject anim;

    protected int maxHealth;
    protected float time;
    protected static int HEALING_POINTS = 1;
    protected bool isOnLink = false;
    protected float linkSpeed = 0.5f;

    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = velocity;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        spawn();

        maxHealth = health;

        //Change layer to default once the monster has been instantiated
        Invoke("changeLayer", 1f);
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
            if (hasDied)
            {
                float nTime = transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
                if (nTime > 1.0f)
                {
                    //death animation has finished
                    die();
                }
            }
            else
            {
                time += Time.deltaTime;
                move();
                heal();
                checkIsOnLink();
            }
        }
    }

    public void heal()
    {
        if (health < maxHealth && time >= healRate)
        {
            time = 0;
            health += HEALING_POINTS;
            healthBar.setValue(health);
        }
    }

    public void checkIsOnLink()
    {
        //check if it's on offmesh link
        if ((agent.isOnOffMeshLink && isOnLink == false))
        {
            isOnLink = true;
            anim.GetComponent<SpriteRenderer>().sprite = batSprite;
            //anim.GetComponent<Transform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);
            agent.speed = agent.speed * linkSpeed;
        }
        else if (!agent.isOnOffMeshLink && isOnLink == true)
        {
            isOnLink = false;
            anim.GetComponent<SpriteRenderer>().sprite = vampireSprite;
            //anim.GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
            agent.speed = velocity;
        }

        NavMeshHit hit;
        if (agent.SamplePathPosition(NavMesh.GetAreaFromName("Fly"), 0.2f, out hit))
        {
            Debug.Log("Flying");
            anim.GetComponent<Animator>().Play("MovementBat");
            anim.GetComponent<SpriteRenderer>().sprite = batSprite;
        }
        else
        {
            Debug.Log("Walking");
            anim.GetComponent<Animator>().Play("Movement");
            anim.GetComponent<SpriteRenderer>().sprite = vampireSprite;
        }
    }
}