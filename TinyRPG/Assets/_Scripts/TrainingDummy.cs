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

    enum DummyState
    {
        spawn,
        idle,
        hurt,
        stun,
        reset
    }

    DummyState state = DummyState.spawn;

    private void Awake()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        Debug.Log(state);

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
            case DummyState.stun:
                dummyStunState();
                break;
            case DummyState.reset:
                dummyResetState();
                break;
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
    }

    public void dummyHurtState()
    {
        //animator.Play("Hurt", 0, 0f);
        animator.Play("Hurt");

        idleTime = 0;
    }

    public void dummyStunState()
    {
        animator.Play("Stunned");
    }

    public void dummyResetState()
    {
        animator.Play("Reset");

        idleTime = 0;
    }

    public void AE_AnimationEnd()
    {
        state = DummyState.idle;
    }

    public void AE_AnimationEndReset()
    {
        state = DummyState.spawn;
    }
}
