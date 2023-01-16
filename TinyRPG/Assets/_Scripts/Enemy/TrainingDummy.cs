using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject stunIcon;
    [SerializeField] GameObject slowIcon;
    public Vector3 startPosition;
    public float idleTime;
    public bool enemyHit;
    public bool enemyStunned;
    public bool enemySlowed;
    bool isStunned;
    bool isSlowed;
    public bool canReset;
    public float slowDuration;
    public float stunDuration;

    enum DummyState
    {
        spawn,
        idle,
        hurt,
        reset
    }

    DummyState state = DummyState.spawn;

    private void Awake()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        //Debug.Log(stunDuration);

        switch (state)
        {
            case DummyState.spawn:
                dummySpawnState();
                break;
            case DummyState.idle:
                dummyIdleState();
                break;
            case DummyState.hurt:
                dummyHurtState();
                break;
            case DummyState.reset:
                dummyResetState();
                break;
        }

        EnemyStun();
        EnemySlow();
    }

    #region Dummy States

    public void dummySpawnState()
    {
        animator.Play("Spawn");

        transform.position = startPosition;
    }

    public void dummyIdleState()
    {
        animator.Play("Idle");

        if (enemyHit)
        {
            enemyHit = false;

            state = DummyState.hurt;
        }

        if (transform.position != startPosition)
        {
            idleTime++;
        }

        if (idleTime >= 1000)
        {
            state = DummyState.reset;
        }
    }

    public void dummyHurtState()
    {
        //animator.Play("Hurt", 0, 0f);
        animator.Play("Hurt");

        idleTime = 0;
    }

    public void dummyResetState()
    {
        animator.Play("Reset");

        idleTime = 0;
    }

    #endregion

    #region Helper Methods

    public void EnemyStun()
    {
        if (enemyStunned)
        {

            // Prevents being stunned twice
            enemyStunned = false;

            isStunned = true;

            // Reset Idle Time
            idleTime = 0;

            // Reset Stun Duration
            stunDuration = 0;

            // Enable Stun
            stunIcon.SetActive(true);
        }

        if (isStunned)
        {
            stunDuration++;
        }

        if (stunDuration >= 1000)
        {
            isStunned = false;
            stunDuration = 0;
            stunIcon.SetActive(false);
            state = DummyState.idle;
        }
    }

    public void EnemySlow()
    {
        if (enemySlowed)
        {
            enemySlowed = false;

            isSlowed = true;

            idleTime = 0;

            slowDuration = 0;

            slowIcon.SetActive(true);
        }

        if (isSlowed)
        {
            slowDuration++;
        }

        if (slowDuration >= 1000)
        {
            isSlowed = false;
            slowDuration = 0;
            slowIcon.SetActive(false);
            state = DummyState.idle;
        }
    }

    #endregion


    #region Animation Events
    public void AE_AnimationEnd()
    {
        state = DummyState.idle;
    }

    public void AE_AnimationEndReset()
    {
        state = DummyState.spawn;
    }
    #endregion
}