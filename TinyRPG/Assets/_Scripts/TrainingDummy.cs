using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    [SerializeField] Animator animator;
    public bool enemyHit;
    public Vector3 startPosition;
    public float idleTime;
    public bool canReset;

    private void Awake()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (enemyHit)
        {
            idleTime = 0;
            enemyHit = false;
            animator.Play("Hit", 0, 0f);
        }

        if (transform.position != startPosition)
        {
            idleTime++;
        }

        if (idleTime >= 1000)
        {
            ResetPosition();
        }
    }

    public void AE_AnimationEnd()
    {
        animator.Play("Idle");
    }

    public void ResetPosition()
    {
        idleTime = 0;
        transform.position = startPosition;
    }
}
