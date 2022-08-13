using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioSource))]
public class JackOLantern : Monster
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        spawn();

        //Play animation
        gameObject.transform.GetChild(0).GetComponent<Animator>().Play("jackOLantern_idle");
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
                healthBar.setValue(health);
            }
        }
    }

    override
    public void die()
    {
        miniGameManager.Instance.numMonstersDied++;
        Instantiate(deathSound, transform.position, Quaternion.identity);
        Destroy(healthBar.gameObject);
        Destroy(this.gameObject);
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
}
