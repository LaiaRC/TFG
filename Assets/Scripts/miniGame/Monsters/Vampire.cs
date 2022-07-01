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
    protected static int HEALING_POINTS = 2;
    protected bool isOnLink = false;
    protected float linkSpeed = 0.5f;

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

        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        move();
        heal();
        checkIsOnLink();
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
        if (agent.isOnOffMeshLink && isOnLink == false)
        {
            isOnLink = true;
            anim.GetComponent<SpriteRenderer>().sprite = batSprite;
            anim.GetComponent<Transform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);
            agent.speed = agent.speed * linkSpeed;
        }
        else if (!agent.isOnOffMeshLink && isOnLink == true)
        {
            isOnLink = false;
            anim.GetComponent<SpriteRenderer>().sprite = vampireSprite;
            anim.GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
            agent.speed = velocity;
        }
    }
}