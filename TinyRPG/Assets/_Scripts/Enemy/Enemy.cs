using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] EnemyHealthBar enemyHealthbar;
    [SerializeField] Animator enemyAnimator;
    [SerializeField] Rigidbody2D enemyRB;
    [SerializeField] Transform enemyFirePoint;
    Transform target;

    [Header("Variables")]
    public float enemyHealth;
    public float enemyMaxHealth;
    public float enemySpeed;
    public float enemyCurrentSpeed;
    float damage;

    [Header("Combat")]
    public GameObject hitSparkPrefab;
    [SerializeField] float aggroRange;
    float idleTime;
    bool canWander = true;
    Vector2 enemyStartingPosition;
    Vector2 newMoveDirection;

    [Header("DeBuffs")]
    [SerializeField] GameObject enemySlowIcon;
    public bool enemySlowedTrigger = false;
    public bool isEnemySlowed = false;
    float enemySlowDuration;

    [SerializeField] GameObject enemyStunIcon;
    public bool enemyStunnedTrigger = false;
    public bool isEnemyStunned = false;
    float enemyStunDuration;

    [SerializeField] GameObject enemyKnockBackIcon;
    public bool enemyKnockBackTrigger = false;
    bool isEnemyKnockedBack = false;
    float enemyKnockBackDuration;


    [Header("MeleeAttack")]
    public static float basicAttackDamage;
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
        enemyHealthbar = GetComponent<EnemyHealthBar>();
    }

    void Start()
    {
        enemyHealth = enemyMaxHealth;
        enemyCurrentSpeed = enemySpeed;
        enemyStartingPosition = transform.position;
    }

    void Update()
    {
        //Debug.Log(state);

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

        EnemySlowed();
        EnemyStunned();
        EnemyKnockBack();
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
        Vector2 newWanderPos = Vector2.MoveTowards(enemyRB.position, newMoveDirection, enemyCurrentSpeed * Time.deltaTime);
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
        Vector2 newPos = Vector2.MoveTowards(enemyRB.position, newTarget, enemyCurrentSpeed * Time.deltaTime);
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
        state = EnemyState.hurt;

        // Animate
        enemyAnimator.Play("Hurt");
        enemyAnimator.SetFloat("Horizontal", enemyRB.position.x - target.position.x);
        enemyAnimator.SetFloat("Vertical", enemyRB.position.y - target.position.y);

        // Behaviour
        TakeDamage(damage);

        // Transition
        // if hurt animation is done playing and player cc'd - wait until cc is done, then idle

        // Transition
        if (enemyHealth <= 0)
        {
            state = EnemyState.death;

            SpawnManager.enemyCount--;
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

    #region Helper Methods
    void EnemySlowed()
    {
        if (enemySlowedTrigger)
        {
            enemySlowedTrigger = false;

            isEnemySlowed = true;

            enemySlowDuration = 0;

            enemySlowIcon.SetActive(true);

            // Slow
            enemyCurrentSpeed = Player.sweepingGustSlowAmount;
        }

        if (isEnemySlowed)
        {
            enemySlowDuration++;
        }


        if (enemySlowDuration >= 1000)
        {
            // Return to normal speed
            enemyCurrentSpeed = enemySpeed;

            isEnemySlowed = false;
            enemySlowDuration = 0;
            enemySlowIcon.SetActive(false);
        }
    }

    void EnemyStunned()
    {
        if (enemyStunnedTrigger)
        {
            enemyStunnedTrigger = false;

            isEnemyStunned = true;

            enemyStunDuration = 0;

            enemyStunIcon.SetActive(true);
        }

        if (isEnemyStunned)
        {
            enemyStunDuration++;
        }


        if (enemyStunDuration >= 1000)
        {
            isEnemyStunned = false;
            enemyStunDuration = 0;
            enemyStunIcon.SetActive(false);
        }
    }

    public void EnemyKnockBack()
    {
        if (enemyKnockBackTrigger)
        {
            enemyKnockBackTrigger = false;

            isEnemyKnockedBack = true;

            enemyKnockBackDuration = 0;

            enemyKnockBackIcon.SetActive(true);
        }

        if (isEnemyKnockedBack)
        {
            enemyKnockBackDuration++;
        }

        if (enemyKnockBackDuration >= 100)
        {
            isEnemyKnockedBack = false;
            enemyKnockBackDuration = 0;
            enemyKnockBackIcon.SetActive(false);
        }
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

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        enemyHealthbar.lerpTimer = 0f;
    }

    void RestoreHealth(float healAmount)
    {
        enemyHealth += healAmount;
        enemyHealthbar.lerpTimer = 0f;
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
