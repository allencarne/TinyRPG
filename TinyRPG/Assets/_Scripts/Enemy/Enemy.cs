using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Animator enemyAnimator;
    [SerializeField] Rigidbody2D enemyRB;
    Transform target;

    float enemyHealth;
    public float enemyMaxHealth;
    public float enemySpeed;



    float idleTime;
    [SerializeField] float aggroRange;
    [SerializeField] float meleeAttackRange;

    bool canMeleeAttack;
    bool canWander = true;

    Vector2 enemyStartingPosition;
    Vector2 newMoveDirection;

    enum EnemyState
    {
        spawn,
        idle,
        wander,
        chase,
        meleeAttack,
        hurt,
        reset,
        death
    }

    EnemyState state = EnemyState.spawn;

    private void Awake()
    {
        target = GameObject.Find("Player").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyHealth = enemyMaxHealth;
        enemyStartingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(state);

        switch (state)
        {
            case EnemyState.spawn:
                EnemySpawnState();
                break;
            case EnemyState.idle:
                EnemyIdleState();
                break;
            case EnemyState.wander:
                EnemyWanderState();
                break;
            case EnemyState.chase:
                EnemyChaseState();
                break;
            case EnemyState.meleeAttack:
                EnemyMeleeAttackState();
                break;
            case EnemyState.hurt:
                EnemyHurtState();
                break;
            case EnemyState.reset:
                EnemyResetState();
                break;
            case EnemyState.death:
                EnemyDeathState();
                break;
        }
    }

    #region Enemy States

    void EnemySpawnState()
    {
        enemyAnimator.Play("Spawn");
    }

    void EnemyIdleState()
    {
        // Animation
        enemyAnimator.Play("Idle");
        enemyAnimator.SetFloat("Horizontal", enemyRB.position.x);
        enemyAnimator.SetFloat("Vertical", enemyRB.position.y);

        // Behaviour
        enemyRB.velocity = new Vector2(0, 0);

        idleTime++;

        if (idleTime >= 500)
        {
            int change = Random.Range(0, 2);
            switch (change)
            {
                case 0:
                    state = EnemyState.wander;
                    idleTime = 0;
                    break;
                case 1:
                    idleTime = 0;
                    break;
            }
        }

        // Transition
        if (Vector2.Distance(target.position, enemyRB.position) <= aggroRange)
        {
            state = EnemyState.chase;
        }

        if (Vector2.Distance(target.position, enemyRB.position) <= meleeAttackRange)
        {
            if (canMeleeAttack)
            {
                state = EnemyState.meleeAttack;
            }
        }
    }

    void EnemyWanderState()
    {
        // Animation
        enemyAnimator.Play("Move");
        enemyAnimator.SetFloat("Horizontal", newMoveDirection.x - enemyRB.position.x);
        enemyAnimator.SetFloat("Vertical", newMoveDirection.y - enemyRB.position.y);

        // Behaviour
        if (canWander)
        {
            canWander = false;

            newMoveDirection = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));

            StartCoroutine(Wandering());
        }
        Vector2 newWanderPos = Vector2.MoveTowards(enemyRB.position, newMoveDirection, enemySpeed * Time.deltaTime);
        enemyRB.MovePosition(newWanderPos);
        //enemyRB.velocity = newMoveDirection * enemySpeed * Time.deltaTime;
    }

    void EnemyChaseState()
    {
        // Animation
        enemyAnimator.Play("Move");

        // Behaviour
        Vector2 newTarget = new Vector2(target.position.x, target.position.y);
        Vector2 newPos = Vector2.MoveTowards(enemyRB.position, newTarget, enemySpeed * Time.deltaTime);
        enemyRB.MovePosition(newPos);

        enemyAnimator.SetFloat("Horizontal", target.position.x - enemyRB.position.x);
        enemyAnimator.SetFloat("Vertical", target.position.y - enemyRB.position.y);
    }

    void EnemyMeleeAttackState()
    {

    }

    void EnemyHurtState()
    {

    }

    void EnemyResetState()
    {

    }

    void EnemyDeathState()
    {

    }

    #endregion

    public void AE_Idle()
    {
        state = EnemyState.idle;
    }

    IEnumerator Wandering()
    {
        yield return new WaitForSeconds(1f);
        canWander = true;
        state = EnemyState.idle;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);
    }
}
