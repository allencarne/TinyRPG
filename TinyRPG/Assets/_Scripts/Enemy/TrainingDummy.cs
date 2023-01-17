using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject stunIcon;
    [SerializeField] GameObject slowIcon;
    public Vector3 startPosition;
    public float dummyIdleTime;
    public bool dummyHit;
    public bool dummyStunned;
    public bool dummySlowed;
    bool isDummyStunned;
    bool isDummySlowed;
    public bool canDummyReset;
    public float dummySlowDuration;
    public float dummyStunDuration;

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

        if (dummyHit)
        {
            dummyHit = false;

            state = DummyState.hurt;
        }

        if (transform.position != startPosition)
        {
            dummyIdleTime++;
        }

        if (dummyIdleTime >= 1000)
        {
            state = DummyState.reset;
        }
    }

    public void dummyHurtState()
    {
        //animator.Play("Hurt", 0, 0f);
        animator.Play("Hurt");

        dummyIdleTime = 0;
    }

    public void dummyResetState()
    {
        animator.Play("Reset");

        dummyIdleTime = 0;
    }

    #endregion

    #region Helper Methods

    public void EnemyStun()
    {
        if (dummyStunned)
        {

            // Prevents being stunned twice
            dummyStunned = false;

            isDummyStunned = true;

            // Reset Idle Time
            dummyIdleTime = 0;

            // Reset Stun Duration
            dummyStunDuration = 0;

            // Enable Stun
            stunIcon.SetActive(true);
        }

        if (isDummyStunned)
        {
            dummyStunDuration++;
        }

        if (dummyStunDuration >= 1000)
        {
            isDummyStunned = false;
            dummyStunDuration = 0;
            stunIcon.SetActive(false);
            state = DummyState.idle;
        }
    }

    public void EnemySlow()
    {
        if (dummySlowed)
        {
            dummySlowed = false;

            isDummySlowed = true;

            dummyIdleTime = 0;

            dummySlowDuration = 0;

            slowIcon.SetActive(true);
        }

        if (isDummySlowed)
        {
            dummySlowDuration++;
        }

        if (dummySlowDuration >= 1000)
        {
            isDummySlowed = false;
            dummySlowDuration = 0;
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
