using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    [SerializeField] Animator animator;
    public bool enemyHit;

    void Update()
    {
        if (enemyHit)
        {
            enemyHit = false;
            animator.Play("Hit", 0, 0f);
        }
    }

    public void AE_AnimationEnd()
    {
        animator.Play("Idle");
    }
}
