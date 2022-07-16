using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float smoothTime = 0.3f;
    public GameObject objective;
    public string type; //monster / villager
    public int damage = 0;

    private Vector3 velocity = Vector3.zero;
    private float time = 0;
    private bool hasHit = false;

    public static string MONSTER = "monster";
    public static string VILLAGER = "villager";

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            time += Time.deltaTime;
            if (objective != null && type != null && damage != 0)
            {
                //Rotate sprite
                Vector3 moveDirection = objective.transform.position - transform.position;
                if (moveDirection != Vector3.zero)
                {
                    float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }

                transform.position = Vector3.SmoothDamp(transform.position, objective.transform.position, ref velocity, smoothTime);
                if ((transform.position - objective.transform.position).magnitude <= 2f)
                {
                    if (type.Equals(MONSTER))
                    {
                        //Damage monster
                        objective.GetComponent<Monster>().takeDamage(damage);
                    }
                    else if (type.Equals(VILLAGER) && !hasHit)
                    {
                        //Scare villager
                        objective.GetComponent<Villager>().takeScare(damage);
                    }
                    gameObject.SetActive(false);
                }
            }

            if (objective == null && time > 0.5f)
            {
                //monster is already dead
                gameObject.SetActive(false);
            }
        }
        else
        {
            time = 0;
        }
    }
}
