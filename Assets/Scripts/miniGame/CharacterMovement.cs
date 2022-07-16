using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour
{
    public List<Animator> animators;
    public NavMeshAgent agent;
    public bool movingRight = false;
    public bool isMoving = false;
    public Vector3 velocity;
    public float speed;

    Vector2 movement;

    void Update()
    {
        speed = agent.speed;

        if (speed < 1f && isMoving)
        {
            isMoving = false;
            foreach (Animator animator in animators)
            {
                animator.SetFloat("Speed", 0);
            }
        }
        else if (speed >= 1f && !isMoving)
        {
            isMoving = true;
            foreach (Animator animator in animators)
            {
                animator.SetFloat("Speed", 1);
            }
        }

        if (isMoving)
        {
            velocity = agent.desiredVelocity;

            if (velocity.x < 0 && movingRight)
            {
                movingRight = false;
                changeAnimation(movingRight);
            }
            else if (velocity.x >= 0 && !movingRight)
            {
                movingRight = true;
                changeAnimation(movingRight);
            }
        }
    }

    public void changeAnimation(bool movingRight)
    {
        if (movingRight)
        {
            foreach (Animator animator in animators)
            {
                animator.SetFloat("Horizontal", 1);
            }
        }
        else
        {
            foreach (Animator animator in animators)
            {
                animator.SetFloat("Horizontal", -1);
            }
        }
    }
}
