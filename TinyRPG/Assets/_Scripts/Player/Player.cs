using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] float moveSpeed;
    [SerializeField] float currentMoveSpeed;
    [HideInInspector] Vector2 movement;
    [HideInInspector] Vector2 angleToMouse;
    public float health;
    public float maxHealth;
    public bool isPlayerHurt;
    public bool isPlayerSliding;
    float damage;

    [Header("Components")]
    [SerializeField] HealthBar healthbar;
    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform firePoint;
    [SerializeField] Animator animator;
    //[SerializeField] GameObject hitSpark;
    [HideInInspector] Camera cam;

    [Header("Buffs")]
    [SerializeField] GameObject playerAlactrityIcon;
    [SerializeField] GameObject playerHasteIcon;
    [SerializeField] GameObject playerMightIcon;
    [SerializeField] GameObject playerProtectionIcon;
    [SerializeField] GameObject playerRegenerationIcon;

    [Header("DeBuffs")]
    [SerializeField] GameObject playerBleedIcon;
    [SerializeField] GameObject playerImpedeIcon;
    [SerializeField] GameObject playerSlowIcon;
    public bool playerSlowTrigger;
    public bool isPlayerSlowed;
    float playerSlowDuration;


    [SerializeField] GameObject playerVulnerabilityIcon;
    [SerializeField] GameObject playerWeaknessIcon;

    [Header("CC")]
    [SerializeField] GameObject playerImmobilizeIcon;
    [SerializeField] GameObject playerIncapacitateIcon;
    [SerializeField] GameObject playerKnockBackIcon;
    [SerializeField] GameObject playerSilenceIcon;
    [SerializeField] GameObject playerStunIcon;

    [Header("Wind Slash")]
    //[SerializeField] GameObject windSlashHitSpark;
    [SerializeField] GameObject windSlashPrefab;
    [SerializeField] float windSlashCoolDown;
    [SerializeField] float windSlashForce;
    [SerializeField] float windSlashAttackRange;
    [SerializeField] float windSlashSlideForce;
    public static float windSlashDamage = 1;
    public static float windSlashKnockBackForce = 6;
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
    public static float sweepingGustSlowAmount = .5f;
    public static float sweepingGustSlowDuration = 4;

    [Header("Tempest Charge")]
    [SerializeField] GameObject tempestChargeIndicator;
    [SerializeField] GameObject tempestChargePrefab;
    [SerializeField] GameObject tempestCharge2Prefab;
    [SerializeField] Transform tempestChargeEndPosition;
    [SerializeField] float tempestChargeCoolDown;
    [SerializeField] float tempestChargeVelocity;
    public static float tempestChargeStunDuration = 2;
    public static bool tempestChargeCollisionTrigger = false;
    bool canTempestCharge = true;
    bool canTempestCharge2 = false;
    Vector3 tempestChargeAngle;
    bool isTempestChargeActive;

    [Header("Parry Strike")]
    [SerializeField] GameObject parryStrikePrefab;
    [SerializeField] GameObject whilrWindPrefab;
    public float parryStrikeCoolDown;
    public static bool parryStrikeTrigger = false;
    public static bool parryStrikeEnd = false;
    float parryStrikeTimer;
    bool canParryStrike = true;
    bool isParryStrikeActive = false;

    [Header("Heavy Blow")]
    [SerializeField] GameObject heavyBlowPrefab;
    [SerializeField] GameObject heavyBlowIndicator;
    public float heavyBlowCoolDown;
    bool canHeavyBlow = true;
    bool isHeavyBlowActive = false;

    [Header("Eruption")]
    [SerializeField] GameObject ultimatePrefab;
    public static float eruptionKnockBackForce = 10f;
    public float eruptionCoolDown;
    bool canEruption = true;
    bool isEruptionActive = false;

    [Header("Keys")]
    [SerializeField] KeyCode upKey;
    [SerializeField] KeyCode downKey;
    [SerializeField] KeyCode leftKey;
    [SerializeField] KeyCode rightKey;
    [SerializeField] KeyCode basicAttackKey;
    [SerializeField] KeyCode abilityKey;
    [SerializeField] KeyCode mobilityKey;
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
        ability,
        mobility,
        defensive,
        ability2,
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
            case PlayerState.ability:
                PlayerAbilityState();
                break;
            case PlayerState.mobility:
                PlayerMobilityState();
                break;
            case PlayerState.defensive:
                PlayerDefensiveState();
                break;
            case PlayerState.ability2:
                PlayerAbility2State();
                break;
            case PlayerState.ultimate:
                PlayerUltimateState();
                break;
            case PlayerState.hurt:
                PlayerHurtState(damage);
                break;
            case PlayerState.death:
                PlayerDeathState();
                break;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            TakeDamage(1);
            playerSlowTrigger = true;
            PlayerSlowed();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RestoreHealth(1);
        }
    }

    private void FixedUpdate()
    {
        if (state == PlayerState.move)
        {
            Movement();
        }
        
        if (state == PlayerState.mobility)
        {
            if (isTempestChargeActive)
            {
                isTempestChargeActive = false;
                rb.velocity = tempestChargeAngle.normalized * tempestChargeVelocity;
            }
        }
        
        if (state == PlayerState.attack || state == PlayerState.attack2 || state == PlayerState.attack3)
        {
            if (isPlayerSliding)
            {
                isPlayerSliding = false;
                SlideForwad();
            }
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
        AbilityKeyPressed();
        DefensiveKeyPressed();
        Ability2KeyPressed();
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
        //Movement();

        // Transitions
        NoMoveKeyPressed();
        AttackKeyPressed();
        DashKeyPressed();
        AbilityKeyPressed();
        DefensiveKeyPressed();
        Ability2KeyPressed();
        UltimateKeyPressed();
    }

    void PlayerAttackState()
    {
        if (!Aim.pauseDirection)
        {
            // Animation
            animator.Play("Attack");

            // Calculate the difference between mouse position and player position
            AngleToMouse();
            AnimationDirection();

            Aim.pauseDirection = true;
        }

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

            //SlideForwad();
            isPlayerSliding = true;
        }

        // Transition
        if (canWindSlash2 && Input.GetKey(basicAttackKey))
        {
            Aim.pauseDirection = false;
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

            //SlideForwad();
            isPlayerSliding = true;
        }

        // Transition
        if (canWindSlash3 && Input.GetKey(basicAttackKey))
        {
            Aim.pauseDirection = false;
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

            //SlideForwad();
            isPlayerSliding = true;
        }
    }

    void PlayerAbilityState()
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

            StartCoroutine(SweepingGustCoolDown());
        }
    }

    void PlayerMobilityState()
    {
        // This if check makes the following code run only once
        if (canTempestCharge)
        {
            // Prevents Dashing more than once
            canTempestCharge = false;

            // Animation
            animator.Play("Run");

            // Get the ablge of the indicator end position and player position
            tempestChargeAngle = tempestChargeEndPosition.position - transform.position;

            // Set Attack Animation Depending on Mouse Position
            animator.SetFloat("Aim Horizontal", tempestChargeAngle.x);
            animator.SetFloat("Aim Vertical", tempestChargeAngle.y);
            // Set Idle to last attack position
            animator.SetFloat("Horizontal", tempestChargeAngle.x);
            animator.SetFloat("Vertical", tempestChargeAngle.y);

            // Add Velocity Based on Angle
            //rb.velocity = angle.normalized * tempestChargeVelocity;
            isTempestChargeActive = true;

            // Instantiate Dash Telegraph and Destroy It
            GameObject _dashTelegraph = Instantiate(tempestChargePrefab, firePoint.position, firePoint.rotation);
            Destroy(_dashTelegraph, .5f);

            // Transition
            StartCoroutine(TempestChargeDelay());
            StartCoroutine(TempestChargeCoolDown());
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

    void PlayerDefensiveState()
    {
        if (canParryStrike)
        {
            // Prevents Attacking more than once
            canParryStrike = false;

            // Disable Player Collider
            circleCollider.enabled = false;

            // Animate - No animation event
            animator.Play("Counter");

            // Instantiate Prefab - Defensive
            Instantiate(parryStrikePrefab, transform.position, Quaternion.identity);
        }

        // Transition - If Parry Strike is Triggered, Attack Animation
        if (parryStrikeTrigger)
        {
            // Prevents Attacking More than Once
            parryStrikeTrigger = false;

            // Turn Collider On
            circleCollider.enabled = true;

            // Animate
            animator.Play("Attack");
        }

        // If Animation Event is Triggered, Instantiate Prefab - Offensive
        if (isParryStrikeActive)
        {
            isParryStrikeActive = false;

            GameObject _whirlWind = Instantiate(whilrWindPrefab, firePoint.position, firePoint.rotation);
            Destroy(_whirlWind, .3f);
        }

        // Transition - If Nothing is hit, Idle
        if (parryStrikeEnd)
        {
            parryStrikeEnd = false;

            //
            circleCollider.enabled = true;

            state = PlayerState.idle;
        }
    }

    void PlayerAbility2State()
    {
        if (canHeavyBlow)
        {
            canHeavyBlow = false;

            animator.Play("Slow Attack");

            AngleToMouse();
            AnimationDirection();

            StartCoroutine(HeavyBlowCoolDown());
        }

        if (isHeavyBlowActive)
        {
            isHeavyBlowActive = false;

            Instantiate(heavyBlowPrefab, firePoint.position, firePoint.rotation);
        }
    }

    void PlayerUltimateState()
    {
        animator.Play("PowerUp");

        if (isEruptionActive)
        {
            isEruptionActive = false;
            Instantiate(ultimatePrefab, firePoint.position, firePoint.rotation);
        }
    }

    public void PlayerHurtState(float damage)
    {
        // State
        state = PlayerState.hurt;

        // Animate
        animator.Play("Hurt");
        animator.SetFloat("Horizontal", rb.position.x);
        animator.SetFloat("Vertical", rb.position.y);

        // Behaviour
        TakeDamage(damage);

        // Transition
        if (health <= 0)
        {
            state = PlayerState.death;
        }
    }

    void PlayerDeathState()
    {
        Destroy(gameObject);
    }

    #endregion

    public void PlayerSlowed()
    {
        if (playerSlowTrigger)
        {
            playerSlowTrigger = false;

            isPlayerSlowed = true;

            playerSlowDuration = 0;

            playerSlowIcon.SetActive(true);

            // Slow
            currentMoveSpeed = 3;
        }

        if (isPlayerSlowed)
        {
            playerSlowDuration++;
        }


        if (playerSlowDuration >= 300)
        {
            // Return to normal speed
            currentMoveSpeed = moveSpeed;

            isPlayerSlowed = false;
            playerSlowDuration = 0;
            playerSlowIcon.SetActive(false);
        }
    }

    public void PlayerKnockedBack()
    {

    }

    public void PlayerStunned()
    {

    }

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

    public void AbilityKeyPressed()
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
            state = PlayerState.ability;
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
            state = PlayerState.mobility;
        }
    }

    public void DefensiveKeyPressed()
    {
        if (Input.GetKey(defensiveKey) && canParryStrike)
        {
            state = PlayerState.defensive;
            StartCoroutine(ParryStrikeCoolDown());
        }
    }

    public void Ability2KeyPressed()
    {
        bool held = Input.GetKeyUp(ability2Key);

        if (Input.GetKey(ability2Key) && canHeavyBlow)
        {
            heavyBlowIndicator.SetActive(true);
        }
        else
        {
            heavyBlowIndicator.SetActive(false);
        }

        if (held && canHeavyBlow)
        {
            heavyBlowIndicator.SetActive(false);
            state = PlayerState.ability2;
        }
    }

    public void UltimateKeyPressed()
    {
        if (Input.GetKey(ultimateKey) && canEruption)
        {
            canEruption = false;
            state = PlayerState.ultimate;
            StartCoroutine(UltimateCoolDown());
        }
    }

    #endregion

    #region Coroutines
    IEnumerator BasicAttackCoolDown()
    {
        yield return new WaitForSeconds(windSlashCoolDown);

        canWindSlash = true;
    }

    IEnumerator TempestChargeDelay()
    {
        yield return new WaitForSeconds(.5f);
        if (!tempestChargeCollisionTrigger && state == PlayerState.mobility)
        {
            rb.velocity = new Vector2(0, 0);
            state = PlayerState.idle;
        }
    }

    IEnumerator TempestChargeCoolDown()
    {
        yield return new WaitForSeconds(tempestChargeCoolDown);

        canTempestCharge = true;
    }

    IEnumerator SweepingGustCoolDown()
    {
        yield return new WaitForSeconds(sweepingGustCoolDown);
        canSweepingGust = true;
    }

    IEnumerator ParryStrikeCoolDown()
    {
        yield return new WaitForSeconds(parryStrikeCoolDown);
        canParryStrike = true;
    }

    IEnumerator HeavyBlowCoolDown()
    {
        yield return new WaitForSeconds(heavyBlowCoolDown);
        canHeavyBlow = true;
    }

    IEnumerator UltimateCoolDown()
    {
        yield return new WaitForSeconds(eruptionCoolDown);
        canEruption = true;
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

    public void Movement()
    {
        // Move in direction of movement keys
        rb.MovePosition(rb.position + movement * Time.deltaTime);
    }

    public void SlideForwad()
    {
        // Calculate the difference between mouse position and player position
        AngleToMouse();

        // If Mouse is outside attack range - Slide
        if (Vector3.Distance(rb.position, cam.ScreenToWorldPoint(Input.mousePosition)) > windSlashAttackRange)
        {
            // Normalize movement vector and times it by attack move distance
            angleToMouse = angleToMouse.normalized * windSlashSlideForce;

            // Disables collision of Player and Enemy
            Physics2D.IgnoreLayerCollision(3, 6, true);

            // Slide in Attack Direction
            rb.MovePosition(rb.position + angleToMouse * moveSpeed * Time.deltaTime);
        }

        // If Movement key is held while attacking - Slide
        if (Input.GetKey(upKey) || Input.GetKey(leftKey) || Input.GetKey(downKey) || Input.GetKey(rightKey))
        {
            // Normalize movement vector and times it by attack move distance
            angleToMouse = angleToMouse.normalized * windSlashSlideForce;

            // Disables collision of Player and Enemy
            Physics2D.IgnoreLayerCollision(3, 6, true);

            // Slide in Attack Direction
            rb.MovePosition(rb.position + angleToMouse * moveSpeed * Time.deltaTime);
        }
    }

    #endregion

    #region Animation Events
    public void AE_WindSlash()
    {
        isWindSlashing = true;
    }

    public void AE_WindSlash2()
    {
        canWindSlash2 = true;
    }

    public void AE_WindSlash3()
    {
        canWindSlash3 = true;
    }

    public void AE_WindSlashEnd()
    {
        Aim.pauseDirection = false;
        canWindSlash2 = false;
        canWindSlash3 = false;
        state = PlayerState.idle;
    }

    public void AE_SweepingGust()
    {
        // Prevents shooting 2 projectiles - This is because it's a shared animation with basic attack
        if (state == PlayerState.ability)
        {
            isSweepingGustActive = true;
        }
    }

    public void AE_ParryStrike()
    {
        if (state == PlayerState.defensive)
        {
            isParryStrikeActive = true;
        } else
        {
            isParryStrikeActive = false;
        }
    }

    public void AE_HeavyBlow()
    {
        isHeavyBlowActive = true;
    }

    public void AE_Eruption()
    {
        isEruptionActive = true;
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
}
