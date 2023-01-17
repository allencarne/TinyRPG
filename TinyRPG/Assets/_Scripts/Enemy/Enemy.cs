using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Animator enemyAnimator;
    [SerializeField] Rigidbody2D enemyRB;
    [SerializeField] Transform enemyFirePoint;
    Transform target;

    [Header("Variables")]
    public float enemyHealth;
    public float enemyMaxHealth;
    public float enemySpeed;
    float damage;

    [Header("Combat")]
    [SerializeField] float aggroRange;
    float idleTime;
    bool canWander = true;
    Vector2 enemyStartingPosition;
    Vector2 newMoveDirection;
    public bool enemyHit = false;
    public bool enemyStunned = false;
    public bool enemySlowed = false;

    [Header("MeleeAttack")]
    [SerializeField] GameObject meleeAttackTelegraph;
    [SerializeField] float meleeAttackCoolDown;
    [SerializeField] float meleeAttackRange;
    bool canMeleeAttack = true;
    bool isMeleeAttacking = false;


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
        target = GameObject.Find("Aim").transform;
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
                EnemyHurtState(damage);
                break;
            case EnemyState.reset:
                EnemyResetState();
                break;
            case EnemyState.death:
                EnemyDeathState();
                break;
        }

        if (enemyHit && state != EnemyState.death)
        {
            enemyHit = false;

            state = EnemyState.hurt;
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

        // Transition
        if (Vector2.Distance(target.position, enemyRB.position) <= meleeAttackRange)
        {
            if (canMeleeAttack)
            {
                state = EnemyState.meleeAttack;
            }
        }

        if (Vector2.Distance(target.position, enemyRB.position) >= aggroRange)
        {
            state = EnemyState.idle;
        }
    }

    void EnemyMeleeAttackState()
    {
        // Set Animation based on player location
        enemyAnimator.Play("MeleeAttack");
        enemyAnimator.SetFloat("Horizontal", target.position.x - enemyRB.position.x);
        enemyAnimator.SetFloat("Vertical", target.position.y - enemyRB.position.y);

        if (canMeleeAttack)
        {
            canMeleeAttack = false;

            StartCoroutine(MeleeAttackCoolDown());
        }

        if (isMeleeAttacking)
        {
            isMeleeAttacking = false;

            Instantiate(meleeAttackTelegraph, enemyFirePoint.position, enemyFirePoint.rotation);
        }
    }

    public void EnemyHurtState(float damage)
    {
        // Animate
        enemyAnimator.Play("Hurt");
        enemyAnimator.SetFloat("Horizontal", enemyRB.position.x - target.position.x);
        enemyAnimator.SetFloat("Vertical", enemyRB.position.y - target.position.y);

        // Behaviour
        enemyHealth -= damage;

        // Transition
        if (enemyHealth <= 0)
        {
            state = EnemyState.death;
        }
    }

    void EnemyResetState()
    {

    }

    void EnemyDeathState()
    {
        Destroy(gameObject);
    }

    #endregion

    public void AE_Idle()
    {
        state = EnemyState.idle;
    }

    public void AE_MeleeAttack()
    {
        isMeleeAttacking = true;
    }

    IEnumerator Wandering()
    {
        yield return new WaitForSeconds(1f);
        canWander = true;
        state = EnemyState.idle;
    }

    IEnumerator MeleeAttackCoolDown()
    {
        yield return new WaitForSeconds(meleeAttackCoolDown);
        canMeleeAttack = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);
    }
}
