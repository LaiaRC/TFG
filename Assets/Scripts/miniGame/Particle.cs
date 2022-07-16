using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public Animator animator;    
    
    // Update is called once per frame
    void Update()
    {
        float nTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (nTime > 1.0f)
        {
            //animation has finished
            gameObject.SetActive(false);
        }
    }
}
