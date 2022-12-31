using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    [SerializeField] Animator animator;

    void Update()
    {
        if (BasicAttack.enemyHit)
        {
            BasicAttack.enemyHit = false;
            animator.Play("Hit", 0, 0f);
        }
    }

    public void AE_AnimationEnd()
    {
        animator.Play("Idle");
    }
}
