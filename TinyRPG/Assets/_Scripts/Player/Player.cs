using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] float moveSpeed;
    [HideInInspector] Vector2 movement;
    [HideInInspector] Vector2 angleToMouse;
    public float health;
    public float maxHealth;

    [Header("Components")]
    [SerializeField] HealthBar healthbar;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform firePoint;
    [SerializeField] Animator animator;
    [HideInInspector] Camera cam;

    [Header("Wind Slash")]
    [SerializeField] GameObject windSlashPrefab;
    [SerializeField] float windSlashCoolDown;
    [SerializeField] float windSlashForce;
    [SerializeField] float windSlashAttackRange;
    [SerializeField] float windSlashSlideForce;
    public static float windSlashDamage = 1;
    public static float windSlashKnockBackForce = 5;
    bool canWindSlash = true;
    bool isWindSlashing = false;

    [Header("Wind Slash 2")]
    [SerializeField] GameObject windSlash2Prefab;
    bool canWindSlash2 = false;

    [Header("Wind Slash 3")]
    [SerializeField] GameObject windSlash3Prefab;
    bool canWindSlash3 = false;

    [Header("Sweeping Gust")]
    [SerializeField] GameObject sweepingGustIndicator;
    [SerializeField] Transform sweepingGustEndPosition;
    [SerializeField] GameObject sweepingGustPrefab;
    public float sweepingGustForce;
    public float sweepingGustCoolDown;
    bool canSweepingGust = true;
    bool isSweepingGustActive;
    public static float sweepingGustSlowAmount = 2;

    [Header("Tempest Charge")]
    [SerializeField] GameObject tempestChargeIndicator;
    [SerializeField] GameObject tempestChargePrefab;
    [SerializeField] GameObject tempestCharge2Prefab;
    [SerializeField] Transform tempestChargeEndPosition;
    [SerializeField] float tempestChargeCoolDown;
    [SerializeField] float tempestChargeVelocity;
    public static bool tempestChargeCollisionTrigger = false;
    bool canTempestCharge = true;
    bool canTempestCharge2 = false;

    [Header("Parry Strike")]
    [SerializeField] GameObject parryStrikePrefab;
    [SerializeField] GameObject whilrWindPrefab;
    public float parryStrikeCoolDown;
    public static bool parryStrikeTrigger = false;
    public static bool parryStrikeEnd = false;
    bool canParryStrike = true;
    bool isParryStrikeActive = false;

    [Header("Heavy Blow")]
    [SerializeField] GameObject ability3Prefab;
    [SerializeField] GameObject slamIndicator;
    public float ability3CoolDown;
    bool canAbility3 = true;
    bool isAbility3Active = false;

    [Header("Eruption")]
    [SerializeField] GameObject ultimatePrefab;
    public float ultimateCoolDown;
    bool canUltimate = true;
    bool isUltimateActive = false;

    [Header("Keys")]
    [SerializeField] KeyCode upKey;
    [SerializeField] KeyCode downKey;
    [SerializeField] KeyCode leftKey;
    [SerializeField] KeyCode rightKey;
    [SerializeField] KeyCode basicAttackKey;
    [SerializeField] KeyCode mobilityKey;
    [SerializeField] KeyCode abilityKey;
    [SerializeField] KeyCode defensiveKey;
    [SerializeField] KeyCode ability2Key;
    [SerializeField] KeyCode ultimateKey;

    enum PlayerState
    {
        idle,
        move,
        attack,
        attack2,
        attack3,
        dash,
        ability1,
        ability2,
        ability3,
        ultimate,
        hurt,
        death
    }

    PlayerState state = PlayerState.idle;

    private void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        Debug.Log(state);

        switch (state)
        {
            case PlayerState.idle:
                PlayerIdleState();
                break;
            case PlayerState.move:
                PlayerMoveState();
                break;
            case PlayerState.attack:
                PlayerAttackState();
                break;
            case PlayerState.attack2:
                PlayerAttack2State();
                break;
            case PlayerState.attack3:
                PlayerAttack3State();
                break;
            case PlayerState.dash:
                PlayerDashState();
                break;
            case PlayerState.ability1:
                PlayerAbility1State();
                break;
            case PlayerState.ability2:
                PlayerAbility2State();
                break;
            case PlayerState.ability3:
                PlayerAbility3State();
                break;
            case PlayerState.ultimate:
                PlayerUltimateState();
                break;
            case PlayerState.hurt:
                PlayerHurtState();
                break;
            case PlayerState.death:
                PlayerDeathState();
                break;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            parryStrikeTrigger = true;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            TakeDamage(1);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RestoreHealth(1);
        }
    }

    #region States
    void PlayerIdleState()
    {
        // Enables collision of Player and Enemy
        Physics2D.IgnoreLayerCollision(3, 6, false);

        // Animation
        animator.Play("Idle");

        rb.velocity = new Vector2(0, 0);

        // Tranitions
        MoveKeyPressed();
        AttackKeyPressed();
        DashKeyPressed();
        Ability1KeyPressed();
        Ability2KeyPressed();
        Ability3KeyPressed();
        UltimateKeyPressed();
    }

    void PlayerMoveState()
    {
        // Aniamtion
        animator.Play("Run");

        // Set idle Animation after move
        if (movement != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Input
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movement = moveInput.normalized * moveSpeed;

        // Movement
        Movement();

        // Transitions
        NoMoveKeyPressed();
        AttackKeyPressed();
        DashKeyPressed();
        Ability1KeyPressed();
        Ability2KeyPressed();
        Ability3KeyPressed();
        UltimateKeyPressed();
    }

    void PlayerAttackState()
    {
        // Animation
        animator.Play("Attack");

        // Calculate the difference between mouse position and player position
        AngleToMouse();
        AnimationDirection();

        // Prevents Attacking more than once
        canWindSlash = false;

        if (isWindSlashing)
        {
            // Prevents attacking more than once
            isWindSlashing = false;

            // Instantiate Basic Attack and Add Force
            GameObject basicAttack = Instantiate(windSlashPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D basicAttackRB = basicAttack.GetComponent<Rigidbody2D>();
            basicAttackRB.AddForce(firePoint.right * windSlashForce, ForceMode2D.Impulse);

            SlideForwad();
        }

        // Transition
        if (canWindSlash2 && Input.GetKey(basicAttackKey))
        {
            state = PlayerState.attack2;
        }
    }

    void PlayerAttack2State()
    {
        // Animation
        animator.Play("Attack2");

        // Calculate the difference between mouse position and player position
        AngleToMouse();
        AnimationDirection();

        // Prevents attacking more than once
        canWindSlash2 = false;

        if (isWindSlashing)
        {
            // Prevents attacking more than once
            isWindSlashing = false;

            // Instantiate Basic Attack and Add Force
            GameObject basicAttack2 = Instantiate(windSlash2Prefab, firePoint.position, firePoint.rotation);
            Rigidbody2D basicAttack2RB = basicAttack2.GetComponent<Rigidbody2D>();
            basicAttack2RB.AddForce(firePoint.right * windSlashForce, ForceMode2D.Impulse);

            SlideForwad();
        }

        // Transition
        if (canWindSlash3 && Input.GetKey(basicAttackKey))
        {
            state = PlayerState.attack3;
        }
    }

    void PlayerAttack3State()
    {
        // Animation
        animator.Play("Attack");

        // Calculate the difference between mouse position and player position
        AngleToMouse();
        AnimationDirection();

        // Prevents attacking more than once
        canWindSlash3 = false;

        if (isWindSlashing)
        {
            // Prevents attacking more than once
            isWindSlashing = false;

            // Instantiate Basic Attack and Add Force
            GameObject basicAttack3 = Instantiate(windSlash3Prefab, firePoint.position, firePoint.rotation);
            Rigidbody2D basicAttack3RB = basicAttack3.GetComponent<Rigidbody2D>();
            basicAttack3RB.AddForce(firePoint.right * windSlashForce, ForceMode2D.Impulse);

            SlideForwad();
        }
    }

    void PlayerDashState()
    {
        // This if check makes the following code run only once
        if (canTempestCharge)
        {
            // Prevents Dashing more than once
            canTempestCharge = false;

            // Animation
            animator.Play("Run");

            // Get the ablge of the indicator end position and player position
            Vector3 angle = tempestChargeEndPosition.position - transform.position;

            // Set Attack Animation Depending on Mouse Position
            animator.SetFloat("Aim Horizontal", angle.x);
            animator.SetFloat("Aim Vertical", angle.y);
            // Set Idle to last attack position
            animator.SetFloat("Horizontal", angle.x);
            animator.SetFloat("Vertical", angle.y);

            // Add Velocity Based on Angle
            rb.velocity = angle.normalized * tempestChargeVelocity;

            // Instantiate Dash Telegraph and Destroy It
            GameObject _dashTelegraph = Instantiate(tempestChargePrefab, firePoint.position, firePoint.rotation);
            Destroy(_dashTelegraph, .5f);

            // Transition
            StartCoroutine(DashDelay());
            StartCoroutine(DashCoolDown());
        }
        
        if (tempestChargeCollisionTrigger)
        {
            tempestChargeCollisionTrigger = false;
            rb.velocity = new Vector2(0, 0);

            if (!canTempestCharge2)
            {
                // Prevents code from running more than once
                canTempestCharge2 = true;

                // Animate
                animator.Play("Quick Attack");

                GameObject _dashEndTelegraph = Instantiate(tempestCharge2Prefab, firePoint.position, firePoint.rotation);
                Destroy(_dashEndTelegraph, .3f);
            }
            canTempestCharge2 = false;
        }
    }

    void PlayerAbility1State()
    {
        // Prevents attacking more than once
        canSweepingGust = false;

        // Animation
        animator.Play("Attack");

        // Get the ablge of the indicator end position and player position
        Vector3 angle = sweepingGustEndPosition.position - transform.position;

        //Quaternion rotation = Quaternion.FromToRotation(firePoint.position, angle);

        // Set Attack Animation Depending on Mouse Position
        animator.SetFloat("Aim Horizontal", angle.x);
        animator.SetFloat("Aim Vertical", angle.y);
        // Set Idle to last attack position
        animator.SetFloat("Horizontal", angle.x);
        animator.SetFloat("Vertical", angle.y);

        if (isSweepingGustActive)
        {
            isSweepingGustActive = false;

            // Instantiate Basic Attack and Add Force
            GameObject tornado = Instantiate(sweepingGustPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D ability1RB = tornado.GetComponent<Rigidbody2D>();
            ability1RB.AddForce(firePoint.right * sweepingGustForce, ForceMode2D.Impulse);

            StartCoroutine(Ability1CoolDown());
        }
    }

    void PlayerAbility2State()
    {
        if (canParryStrike)
        {
            canParryStrike = false;
            animator.Play("Counter");

            Instantiate(parryStrikePrefab, transform.position, Quaternion.identity);
        }

        // If player is attacked while this state is active - Play "Attack" animation
        if (parryStrikeTrigger)
        {
            parryStrikeTrigger = false;
            animator.Play("Attack");
        }

        if (isParryStrikeActive)
        {
            isParryStrikeActive = false;

            // Spawn prefab that will spin in a circle around the player and stun and damage all enemies hit
            GameObject _whirlWind = Instantiate(whilrWindPrefab, firePoint.position, firePoint.rotation);
            Destroy(_whirlWind, .3f);
        }

        if (parryStrikeEnd)
        {
            parryStrikeEnd = false;

            state = PlayerState.idle;
        }
    }

    void PlayerAbility3State()
    {
        if (canAbility3)
        {
            canAbility3 = false;

            animator.Play("Slow Attack");

            AngleToMouse();
            AnimationDirection();

            StartCoroutine(Ability3CoolDown());
        }

        if (isAbility3Active)
        {
            isAbility3Active = false;

            Instantiate(ability3Prefab, firePoint.position, firePoint.rotation);
        }
    }

    void PlayerUltimateState()
    {
        animator.Play("PowerUp");

        if (isUltimateActive)
        {
            isUltimateActive = false;
            Instantiate(ultimatePrefab, firePoint.position, firePoint.rotation);
        }
    }

    public void PlayerHurtState()
    {
        Debug.Log("Hurt");

        state = PlayerState.hurt;

        // Animate
        animator.Play("Hurt");
    }

    void PlayerDeathState()
    {

    }

    #endregion

    #region Input
    public void MoveKeyPressed()
    {
        //  Movement Key Pressed
        if (Input.GetKey(upKey) || Input.GetKey(leftKey) || Input.GetKey(downKey) || Input.GetKey(rightKey))
        {
            state = PlayerState.move;
        }
    }

    public void NoMoveKeyPressed()
    {
        // No Movement Key Pressed
        if (!Input.GetKey(upKey) && !Input.GetKey(leftKey) && !Input.GetKey(downKey) && !Input.GetKey(rightKey))
        {
            state = PlayerState.idle;
        }
    }

    public void AttackKeyPressed()
    {
        // Basic Attack Key Pressed
        if (Input.GetKey(basicAttackKey) && canWindSlash)
        {
            canWindSlash = false;
            state = PlayerState.attack;
            StartCoroutine(BasicAttackCoolDown());
        }
    }

    public void DashKeyPressed()
    {
        bool held = Input.GetKeyUp(mobilityKey);

        // Dash Key Pressed
        if (Input.GetKey(mobilityKey) && canTempestCharge)
        {
            tempestChargeIndicator.SetActive(true);
        } else
        {
            tempestChargeIndicator.SetActive(false);
        }

        // Dash Key Held
        if (held && canTempestCharge)
        {
            tempestChargeIndicator.SetActive(false);
            state = PlayerState.dash;
        }
    }

    public void Ability1KeyPressed()
    {
        bool held = Input.GetKeyUp(abilityKey);

        if (Input.GetKey(abilityKey) && canSweepingGust)
        {
            sweepingGustIndicator.SetActive(true);
        }
        else
        {
            sweepingGustIndicator.SetActive(false);
        }

        if (held && canSweepingGust)
        {
            sweepingGustIndicator.SetActive(false);
            state = PlayerState.ability1;
        }
    }

    public void Ability2KeyPressed()
    {
        if (Input.GetKey(defensiveKey) && canParryStrike)
        {
            state = PlayerState.ability2;
            StartCoroutine(Ability2CoolDown());
        }
    }

    public void Ability3KeyPressed()
    {
        bool held = Input.GetKeyUp(ability2Key);

        if (Input.GetKey(ability2Key) && canAbility3)
        {
            slamIndicator.SetActive(true);
        }
        else
        {
            slamIndicator.SetActive(false);
        }

        if (held && canAbility3)
        {
            slamIndicator.SetActive(false);
            state = PlayerState.ability3;
        }
    }

    public void UltimateKeyPressed()
    {
        if (Input.GetKey(ultimateKey) && canUltimate)
        {
            canUltimate = false;
            state = PlayerState.ultimate;
            StartCoroutine(UltimateCoolDown());
        }
    }

    #endregion

    #region Coroutines
    IEnumerator DashDelay()
    {
        yield return new WaitForSeconds(.5f);
        if (!tempestChargeCollisionTrigger && state == PlayerState.dash)
        {
            rb.velocity = new Vector2(0, 0);
            state = PlayerState.idle;
        }
    }

    IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(tempestChargeCoolDown);

        canTempestCharge = true;
    }

    IEnumerator BasicAttackCoolDown()
    {
        yield return new WaitForSeconds(windSlashCoolDown);

        canWindSlash = true;
    }

    IEnumerator Ability1CoolDown()
    {
        yield return new WaitForSeconds(sweepingGustCoolDown);
        canSweepingGust = true;
    }

    IEnumerator Ability2CoolDown()
    {
        yield return new WaitForSeconds(parryStrikeCoolDown);
        canParryStrike = true;
    }

    IEnumerator Ability3CoolDown()
    {
        yield return new WaitForSeconds(ability3CoolDown);
        canAbility3 = true;
    }

    IEnumerator UltimateCoolDown()
    {
        yield return new WaitForSeconds(ultimateCoolDown);
        canUltimate = true;
    }

    #endregion

    #region Helper Methods
    public void AngleToMouse()
    {
        // Calculates the difference between the mouse position and player position
        // Not Normalized
        angleToMouse = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    }

    public void AnimationDirection()
    {
        // Set Attack Animation Depending on Mouse Position
        animator.SetFloat("Aim Horizontal", angleToMouse.x);
        animator.SetFloat("Aim Vertical", angleToMouse.y);
        // Set Idle to last attack position
        animator.SetFloat("Horizontal", angleToMouse.x);
        animator.SetFloat("Vertical", angleToMouse.y);
    }

    public void Blink()
    {
        // Instantly Teleport in direction of movement keys
        // Does not teleport if no movment keys are being pressed
        rb.MovePosition(rb.position + movement * tempestChargeVelocity);
    }

    public void Movement()
    {
        // Move in direction of movement keys
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }

    public void SlideForwad()
    {
        // Calculate the difference between mouse position and player position
        AngleToMouse();

        // If Mouse 
        if (Vector3.Distance(rb.position, cam.ScreenToWorldPoint(Input.mousePosition)) > windSlashAttackRange)
        {
            // Normalize movement vector and times it by attack move distance
            angleToMouse = angleToMouse.normalized * windSlashSlideForce;

            // Disables collision of Player and Enemy
            Physics2D.IgnoreLayerCollision(3, 6, true);

            // Slide in Attack Direction
            rb.MovePosition(rb.position + angleToMouse);
        }

        if (Input.GetKey(upKey) || Input.GetKey(leftKey) || Input.GetKey(downKey) || Input.GetKey(rightKey))
        {
            // Normalize movement vector and times it by attack move distance
            angleToMouse = angleToMouse.normalized * windSlashSlideForce;

            // Disables collision of Player and Enemy
            Physics2D.IgnoreLayerCollision(3, 6, true);

            // Slide in Attack Direction
            rb.MovePosition(rb.position + angleToMouse);
        }
    }

    #endregion

    #region Animation Events
    public void AE_BasicAttack()
    {
        isWindSlashing = true;
    }

    public void AE_BasicAttack2()
    {
        canWindSlash2 = true;
    }

    public void AE_BasicAttack3()
    {
        canWindSlash3 = true;
    }

    public void AE_BasicAttackEnd()
    {
        canWindSlash2 = false;
        canWindSlash3 = false;
        state = PlayerState.idle;
    }

    public void AE_Ability1()
    {
        // Prevents shooting 2 projectiles - This is because it's a shared animation with basic attack
        if (state == PlayerState.ability1)
        {
            isSweepingGustActive = true;
        }
    }

    public void AE_Ability2()
    {
        if (state == PlayerState.ability2)
        {
            isParryStrikeActive = true;
        }
    }

    public void AE_Ability3()
    {
        isAbility3Active = true;
    }

    public void AE_Ultimate()
    {
        isUltimateActive = true;
    }

    public void AE_EndOfAnimation()
    {
        state = PlayerState.idle;
    }

    #endregion

    void TakeDamage(float damage)
    {
        health -= damage;
        healthbar.lerpTimer = 0f;
    }

    void RestoreHealth(float healAmount)
    {
        health += healAmount;
        healthbar.lerpTimer = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            // Components
            var enemy = collision.gameObject.GetComponent<Enemy>();

            // HitSpark
            Instantiate(enemy.hitSparkPrefab, collision.transform.position, collision.transform.rotation);

            // Deal Damage
            PlayerHurtState();

            // KnockBack
            Vector2 direction = (transform.position - collision.transform.position).normalized;
            rb.velocity = direction * 5;
        }

        if (collision.tag == "EnemyAttack")
        {
            // Components
            var enemy = collision.gameObject.GetComponent<Enemy>();

            // HitSpark
            Instantiate(enemy.hitSparkPrefab, collision.transform.position, collision.transform.rotation);

            // Deal Damage
            PlayerHurtState();

            // KnockBack
            Vector2 direction = (transform.position - collision.transform.position).normalized;
            rb.velocity = direction * 5 ;
        }
    }
}
