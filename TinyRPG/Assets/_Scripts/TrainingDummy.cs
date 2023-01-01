using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    [SerializeField] Animator animator;
    public bool enemyHit;
    public Vector2 startPosition;
    public float idleTime;

    private void Awake()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (enemyHit)
        {
            enemyHit = false;
            animator.Play("Hit", 0, 0f);

            idleTime = 0;
        }

        if (!enemyHit)
        {
            idleTime++;
        }

        if (idleTime >= 1000)
        {
            transform.position = startPosition;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.position = startPosition;
        }
    }

    public void AE_AnimationEnd()
    {
        animator.Play("Idle");
    }
}
