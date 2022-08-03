using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clown : Monster
{
    public int lifeTime; //stun Time
    public int numAffectedVillagers = 0;

    public bool isStunning = false;
    protected float time;
    protected List<GameObject> affectedVillagers = new List<GameObject>();
    protected bool isOnList = false;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        spawn();
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
                if (isStunning)
                {
                    time += Time.deltaTime;
                    health = (int)((int)lifeTime - time);
                }

                if (!isStunning || isStunning && time < lifeTime)
                {
                    stun();
                }
                else if (isStunning && time >= lifeTime)
                {
                    die();
                }
                healthBar.setValue(health);
            }
        }
    }

    override
    protected void spawn()
    {
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        audioSource.clip = sounds[SPAWN];
        audioSource.Play();
        //Instantiate(spawnParticles, transform.position, Quaternion.identity);

        //spawn particles
        animator.Play("monster_spawn");

        healthBar = Instantiate(healthBar, canvas);
        healthBar.setTarget(transform);
        healthBar.setMaxValue(health);
        healthBar.setMinValue(0);
        healthBar.setValue(health);
        healthBar.setColor(fillColor, backgroundColor);
        healthBar.targetCanvas = canvas;
    }

    override
    public void die()
    {
        miniGameManager.Instance.numMonstersDied++;
        //Turn back orginal conditions to villagers
        foreach (GameObject villager in affectedVillagers)
        {
            villager.GetComponent<Villager>().agent.speed = villager.GetComponent<Villager>().velocity;
            villager.GetComponent<Villager>().canMove = true;
            villager.GetComponent<Villager>().isStunned = false;
            if (villager.GetComponent<Villager>().type.Equals("mom") || villager.GetComponent<Villager>().type.Equals("swashbuckler") || villager.GetComponent<Villager>().type.Equals("shieldMan") || villager.GetComponent<Villager>().type.Equals("sorcerer"))
            {
                villager.GetComponent<Villager>().canAttack = true;
            }
        }
        affectedVillagers.Clear();
        /*Instantiate(deathParticles, transform.position, Quaternion.identity);
        Instantiate(deathSound, transform.position, Quaternion.identity);*/
        Destroy(healthBar.gameObject);
        Destroy(this.gameObject);
    }

    public void stun()
    {
        if (checkVillagersInRange())
        {
            isStunning = true;
            List<GameObject> villagers = getVillagersInRange();
            numAffectedVillagers = villagers.Count;
            foreach (GameObject villager in villagers)
            {
                isOnList = false;
                foreach (GameObject affectedVillager in affectedVillagers)
                {
                    if(villager == affectedVillager)
                    {
                        isOnList = true;
                    }
                }
                if (!isOnList || !villager.GetComponent<Villager>().isStunned)
                {
                    villager.GetComponent<Villager>().agent.speed = 0;
                    villager.GetComponent<Villager>().canAttack = false;
                    villager.GetComponent<Villager>().canMove = false;
                    villager.GetComponent<Villager>().isStunned = true;
                    affectedVillagers.Add(villager);
                }
            }
        }
    }

    public List<GameObject> getVillagersInRange()
    {
        //es pot cridar cada X segons x millorar rendiment i no a cada frame al update

        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, detectionRange);

        List<GameObject> villagers = new List<GameObject>();

        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<Villager>() && collision.GetComponent<Villager>().level <= level)
            {
                villagers.Add(collision.gameObject);
            }
        }
        return villagers;
    }   
}
