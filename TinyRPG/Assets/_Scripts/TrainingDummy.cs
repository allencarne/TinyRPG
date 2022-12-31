using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    [SerializeField] Animator animator;
    BasicAttack basicAttack;

    private void Awake()
    {
        basicAttack = GetComponent<BasicAttack>();
    }

    void Update()
    {
        if (basicAttack.enemyHit)
        {
            basicAttack.enemyHit = false;
            animator.Play("Hit", 0, 0f);
        }
    }

    public void AE_AnimationEnd()
    {
        animator.Play("Idle");
    }
}
