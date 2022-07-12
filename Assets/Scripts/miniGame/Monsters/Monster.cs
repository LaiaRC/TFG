using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioSource))]
public abstract class Monster : MonoBehaviour
{
    public float velocity;
    public int health;
    public int damage;
    public float range;
    public float attackRate;
    public bool canBeInvokedInside;
    public bool canFly;
    public string currentState;
    public string favVillager;
    public int level;

    protected Transform currentTarget;
    protected List<Transform> flags = new List<Transform>();
    protected int currentFlag = 0;
    public Animator animator;
    public GameObject tomb;
    public GameObject deathParticles;
    public GameObject spawnParticles;
    public GameObject deathSound;
    public AudioSource audioSource;
    public AudioSource audioSourceAux; //Perque sonin 2 a la vegada i no es talli si ha començat
    public AudioClip[] sounds;
    public HealthBar healthBar;
    protected float attackTime = 0;
    public Color fillColor;
    public Color backgroundColor;

    protected RectTransform canvas;

    protected NavMeshAgent agent;
    public bool isAttacking = false;

    //Variables x els sons de l'array sounds
    protected static int ATTACK = 0;
    protected static int DEFAULT = 1;
    protected static int SPAWN = 2;
    protected static int TAKE_DAMAGE = 3;
    protected static int WALK = 4;

    public void gameOver()
    {
        gameObject.SetActive(false);
        healthBar.gameObject.SetActive(false);
    }
    
    public virtual void die()
    {
        //Instantiate tomb
        /*Instantiate(deathParticles, transform.position, Quaternion.identity);
        Instantiate(deathSound, transform.position, Quaternion.identity);*/
        Destroy(healthBar.gameObject);
        Instantiate(tomb,transform.position, Quaternion.identity);
        Destroy(this.gameObject);        
    }

    protected virtual void spawn()
    {
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        audioSource.clip = sounds[SPAWN];
        audioSource.Play();
        Instantiate(spawnParticles, transform.position, Quaternion.identity);

        for (int i = 0; i < miniGameManager.Instance.activeFlags.Count; i++)
        {            
            flags.Add(miniGameManager.Instance.activeFlags[i].GetComponent<Transform>());            
        }

        healthBar = Instantiate(healthBar, canvas);
        healthBar.setTarget(transform);
        healthBar.setMaxValue(health);
        healthBar.setMinValue(0);
        healthBar.setValue(health);
        healthBar.setColor(fillColor, backgroundColor);
        healthBar.targetCanvas = canvas;

        agent.stoppingDistance = 3;
    }

    public void updateFlags()
    {
        flags.Clear();
        for (int i = 0; i < miniGameManager.Instance.activeFlags.Count; i++)
        {
            flags.Add(miniGameManager.Instance.activeFlags[i].GetComponent<Transform>());
        }
    }

    public void takeDamage(int damage)
    {
        health -= damage;
        healthBar.setValue(health);

        if (health <= 0)
        {
            die();
        }
    }

    public void changeAnimationState(string state)
    {
        if (currentState == state) return;

        animator.Play(state);

        currentState = state;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public virtual void move()
    {
        
        if (!checkVillagersInRange())
        {
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
            currentTarget = getVillagerInRange();
            if ((transform.position - currentTarget.position).magnitude <= agent.stoppingDistance + 0.2f)
            {
                attackTime += Time.deltaTime;
                if (attackTime >= attackRate)
                {
                    isAttacking = true;
                    currentTarget.gameObject.GetComponent<Villager>().takeScare(damage);
                    miniGameManager.Instance.numScares++;
                    attackTime = 0;
                }
            }
            else
            {
                //keep moving to waypoint
                agent.SetDestination(currentTarget.position);
            }
        }
    }

    public Transform getVillagerInRange()
    {
        //es pot cridar cada X segons x millorar rendiment i no a cada frame al update

        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, range);

        List<Collider2D> villagers = new List<Collider2D>(); //not favourite villagers
        List<Collider2D> favVillagers = new List<Collider2D>();
        Transform closestVillager = null;

        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<Villager>() && collision.GetComponent<Villager>().level <= level)
            {
                //Check if favourite villager
                if(collision.GetComponent<Villager>().type == favVillager)
                {
                    favVillagers.Add(collision);
                }
                else
                {
                    villagers.Add(collision);
                }
            }
        }

        if(favVillagers.Count > 0){

            foreach (Collider2D favVillager in favVillagers)
            {
                if (closestVillager == null)
                {
                    closestVillager = favVillager.transform;
                }
                else
                {
                    if ((favVillager.transform.position - transform.position).magnitude < (closestVillager.transform.position - transform.position).magnitude)
                    {
                        closestVillager = favVillager.transform;
                    }
                }
            }
        }
        else
        {
            //There aren't fav villagers in range

            foreach (Collider2D villager in villagers)
            {
                if (closestVillager == null)
                {
                    closestVillager = villager.transform;
                }
                else
                {
                    if ((villager.transform.position - transform.position).magnitude < (closestVillager.transform.position - transform.position).magnitude)
                    {
                        closestVillager = villager.transform;
                    }
                }
            }
        }
        return closestVillager;
    }

    public virtual bool checkVillagersInRange()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<Villager>() && collision.GetComponent<Villager>().level <= level)
            {
                return true;
            }
        }
        return false;
    }

    public void changeLayer()
    {
        gameObject.layer = 0;
    }
}
