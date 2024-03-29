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
    public float wanderRadius;
    float damage;
    bool isEnemyWandering;
    bool isEnemyChasing;
    bool canChase = true;
    float randomWanderDirection;
    bool canWander = true;

    [Header("Combat")]
    [SerializeField] GameObject hitSparkPrefab;
    [SerializeField] float aggroRange;
    [SerializeField] float deAggroRange;
    public static bool isEnemyHurt;
    Vector2 enemyStartingPosition;
    Vector2 newMoveDirection;
    float idleTime;

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
    [SerializeField] GameObject meleeAttackTelegraph;
    [SerializeField] GameObject maleeAttackPrefab;
    [SerializeField] float meleeAttackCoolDown;
    [SerializeField] float meleeAttackRange;
    public static float basicAttackDamage;
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
        //Debug.Log(idleTime);

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

    private void FixedUpdate()
    {
        if (isEnemyWandering)
        {
            // Prevents from running more than once
            isEnemyWandering = false;

            // Get Move Direction
            newMoveDirection = Random.insideUnitCircle * wanderRadius;

            // Move
            enemyRB.velocity = newMoveDirection * enemyCurrentSpeed;
        }

        if (state == EnemyState.chase)
        {
            isEnemyChasing = true;
        } else
        {
            isEnemyChasing = false;
        }

        if (isEnemyChasing)
        {
            // Get Direction
            Vector3 direction = target.position - transform.position;
            newMoveDirection = direction.normalized;

            // Animate
            enemyAnimator.SetFloat("Horizontal", target.position.x - enemyRB.position.x);
            enemyAnimator.SetFloat("Vertical", target.position.y - enemyRB.position.y);


            // Move
            enemyRB.MovePosition(enemyRB.position + newMoveDirection * enemyCurrentSpeed * Time.deltaTime);
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

        idleTime += 1 * Time.deltaTime;

        if (idleTime >= 5)
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

        // Reset chase variable
        canChase = true;

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
        enemyAnimator.SetFloat("Horizontal", newMoveDirection.x);
        enemyAnimator.SetFloat("Vertical", newMoveDirection.y);

        // Behaviour
        if (canWander)
        {
            canWander = false;

            isEnemyWandering = true;

            StartCoroutine(Wandering());
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

    void EnemyChaseState()
    {
        // Animation
        enemyAnimator.Play("Move");

        if (canChase)
        {
            canChase = false;
        }

        // If Target is inside attack range - Attack
        if (Vector2.Distance(target.position, enemyRB.position) <= meleeAttackRange)
        {
            if (canMeleeAttack)
            {
                state = EnemyState.meleeAttack;
            }
        }

        // If Target is outside of deAggro Range - Return to Spawn Position
        if (Vector2.Distance(target.position, enemyRB.position) >= deAggroRange)
        {
            // Return to spawn position
            state = EnemyState.idle;
        }
    }

    void EnemyMeleeAttackState()
    {
        // If Pause Direction is False, Pause Direction and face the player direction
        if (!EnemyAim.pauseDirection)
        {
            // Set Animation based on player location
            enemyAnimator.Play("MeleeAttack");
            enemyAnimator.SetFloat("Horizontal", target.position.x - enemyRB.position.x);
            enemyAnimator.SetFloat("Vertical", target.position.y - enemyRB.position.y);

            EnemyAim.pauseDirection = true;
        }

        if (canMeleeAttack)
        {
            canMeleeAttack = false;

            Instantiate(meleeAttackTelegraph, enemyFirePoint.position, enemyFirePoint.rotation);

            StartCoroutine(MeleeAttackCoolDown());
        }

        if (isMeleeAttacking)
        {
            isMeleeAttacking = false;

            Instantiate(maleeAttackPrefab, enemyFirePoint.position, enemyFirePoint.rotation);
        }
    }

    public void EnemyHurtState(float damage)
    {
        state = EnemyState.hurt;

        // If enemy is hurt - Destroy Attack Telegraph
        isEnemyHurt = true;

        // Animate
        enemyAnimator.Play("Hurt");
        enemyAnimator.SetFloat("Horizontal", enemyRB.position.x - target.position.x);
        enemyAnimator.SetFloat("Vertical", enemyRB.position.y - target.position.y);

        // Behaviour
        TakeDamage(damage);

        // Die
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

    #region Debuffs
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


        if (enemySlowDuration >= 300)
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
            enemyAnimator.StartPlayback();

        } else
        {
            enemyAnimator.StopPlayback();
        }


        if (enemyStunDuration >= 200)
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

    #region Animation Events

    public void AE_Idle()
    {
        EnemyAim.pauseDirection = false;
        isEnemyHurt = false;
        state = EnemyState.idle;
    }

    public void AE_MeleeAttack()
    {
        isMeleeAttacking = true;
    }

    public void AE_EnemyHurt()
    {
        EnemyAim.pauseDirection = false;
        isEnemyHurt = false;
        state = EnemyState.idle;
    }

    #endregion

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
        yield return new WaitForSeconds(.5f);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, deAggroRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}
