using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] float moveSpeed;
    [HideInInspector] Vector2 movement;
    [HideInInspector] Vector2 angleToMouse;
    [HideInInspector] Vector2 mousePos;
    [HideInInspector] float offset;

    [Header("Components")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform firePoint;
    [HideInInspector] Camera cam;
    [SerializeField] Animator animator;

    [Header("Basic Attack")]
    [SerializeField] GameObject basicAttackPrefab;
    [SerializeField] float basicAttackCoolDown;
    [SerializeField] float basicAttackForce;
    [SerializeField] float attackRange;
    [SerializeField] float basicAttackSlideForce;
    public static float knockBackForce = 5;
    bool canBasicAttack = true;
    bool isBasicAttacking = false;
    bool dashEndTrigger = false;

    [Header("Basic Attack2")]
    [SerializeField] GameObject basicAttack2Prefab;
    bool canBasicAttack2 = false;

    [Header("Basic Attack3")]
    [SerializeField] GameObject basicAttack3Prefab;
    bool canBasicAttack3 = false;

    [Header("Dash")]
    [SerializeField] GameObject dashIndicator;
    [SerializeField] GameObject dashTelegraph;
    [SerializeField] GameObject dashEndTelegraph;
    [SerializeField] Transform dashEndPosition;
    [SerializeField] float dashCoolDown;
    [SerializeField] float dashVelocity;
    public static float dashKnockBackForce = 2;
    public static bool dashCollide = false;
    bool canDash = true;

    [Header("Ability1")]
    [SerializeField] GameObject ability1Indicator;
    [SerializeField] Transform ability1EndPosition;
    [SerializeField] GameObject ability1Prefab;
    public static float pullForce = -15f;

    public float ability1Force;
    public float ability1CoolDown;
    bool canAbility1 = true;
    bool isAbility1Active;


    [Header("Keys")]
    [SerializeField] KeyCode upKey;
    [SerializeField] KeyCode downKey;
    [SerializeField] KeyCode leftKey;
    [SerializeField] KeyCode rightKey;
    [SerializeField] KeyCode basicAttackKey;
    [SerializeField] KeyCode dashKey;
    [SerializeField] KeyCode ability1Key;

    enum PlayerState
    {
        idle,
        move,
        attack,
        attack2,
        attack3,
        dash,
        ability1,
        hurt,
        death
    }

    PlayerState state = PlayerState.idle;

    private void Awake()
    {
        cam = Camera.main;
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
            case PlayerState.hurt:
                PlayerHurtState();
                break;
            case PlayerState.death:
                PlayerDeathState();
                break;
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
    }

    void PlayerAttackState()
    {
        // Animation
        animator.Play("Attack");

        // Calculate the difference between mouse position and player position
        AngleToMouse();
        AnimationDirection();

        // Prevents Attacking more than once
        canBasicAttack = false;

        if (isBasicAttacking)
        {
            // Prevents attacking more than once
            isBasicAttacking = false;

            // Instantiate Basic Attack and Add Force
            GameObject basicAttack = Instantiate(basicAttackPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D basicAttackRB = basicAttack.GetComponent<Rigidbody2D>();
            basicAttackRB.AddForce(firePoint.right * basicAttackForce, ForceMode2D.Impulse);

            SlideForwad();
        }

        // Transition
        if (canBasicAttack2 && Input.GetKey(basicAttackKey))
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
        canBasicAttack2 = false;

        if (isBasicAttacking)
        {
            // Prevents attacking more than once
            isBasicAttacking = false;

            // Instantiate Basic Attack and Add Force
            GameObject basicAttack2 = Instantiate(basicAttack2Prefab, firePoint.position, firePoint.rotation);
            Rigidbody2D basicAttack2RB = basicAttack2.GetComponent<Rigidbody2D>();
            basicAttack2RB.AddForce(firePoint.right * basicAttackForce, ForceMode2D.Impulse);

            SlideForwad();
        }

        // Transition
        if (canBasicAttack3 && Input.GetKey(basicAttackKey))
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
        canBasicAttack3 = false;

        if (isBasicAttacking)
        {
            // Prevents attacking more than once
            isBasicAttacking = false;

            // Instantiate Basic Attack and Add Force
            GameObject basicAttack3 = Instantiate(basicAttack3Prefab, firePoint.position, firePoint.rotation);
            Rigidbody2D basicAttack3RB = basicAttack3.GetComponent<Rigidbody2D>();
            basicAttack3RB.AddForce(firePoint.right * basicAttackForce, ForceMode2D.Impulse);

            SlideForwad();
        }
    }

    void PlayerDashState()
    {
        // This if check makes the following code run only once
        if (canDash)
        {
            // Prevents Dashing more than once
            canDash = false;

            // Animation
            animator.Play("Run");

            // Get the ablge of the indicator end position and player position
            Vector3 angle = dashEndPosition.position - transform.position;

            // Set Attack Animation Depending on Mouse Position
            animator.SetFloat("Aim Horizontal", angle.x);
            animator.SetFloat("Aim Vertical", angle.y);
            // Set Idle to last attack position
            animator.SetFloat("Horizontal", angle.x);
            animator.SetFloat("Vertical", angle.y);

            // Logic
            //rb.velocity = angleToMouse.normalized * dashVelocity;
            rb.velocity = angle.normalized * dashVelocity;

            GameObject _dashTelegraph = Instantiate(dashTelegraph, firePoint.position, firePoint.rotation);
            Destroy(_dashTelegraph, .5f);

            // Transition
            StartCoroutine(DashDelay());
            StartCoroutine(DashCoolDown());
        }
        
        if (dashCollide)
        {
            dashCollide = false;
            rb.velocity = new Vector2(0, 0);

            if (!dashEndTrigger)
            {
                // Prevents code from running more than once
                dashEndTrigger = true;

                // Animate
                animator.Play("Quick Attack");

                GameObject _dashEndTelegraph = Instantiate(dashEndTelegraph, firePoint.position, firePoint.rotation);
                Destroy(_dashEndTelegraph, .3f);
            }
            dashEndTrigger = false;
        }
    }

    void PlayerAbility1State()
    {
        // Prevents attacking more than once
        canAbility1 = false;

        // Animation
        animator.Play("Attack");

        // Get the ablge of the indicator end position and player position
        Vector3 angle = ability1EndPosition.position - transform.position;

        //Quaternion rotation = Quaternion.FromToRotation(firePoint.position, angle);

        // Set Attack Animation Depending on Mouse Position
        animator.SetFloat("Aim Horizontal", angle.x);
        animator.SetFloat("Aim Vertical", angle.y);
        // Set Idle to last attack position
        animator.SetFloat("Horizontal", angle.x);
        animator.SetFloat("Vertical", angle.y);

        if (isAbility1Active)
        {
            isAbility1Active = false;

            // Instantiate Basic Attack and Add Force
            GameObject tornado = Instantiate(ability1Prefab, firePoint.position, firePoint.rotation);
            Rigidbody2D ability1RB = tornado.GetComponent<Rigidbody2D>();
            ability1RB.AddForce(firePoint.right * ability1Force, ForceMode2D.Impulse);

            StartCoroutine(Ability1CoolDown());
        }
    }

    IEnumerator DashDelay()
    {
        yield return new WaitForSeconds(.5f);
        if (!dashCollide && state == PlayerState.dash)
        {
            rb.velocity = new Vector2(0, 0);
            state = PlayerState.idle;
        }
    }

    IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(dashCoolDown);

        canDash = true;
    }

    IEnumerator BasicAttackCoolDown()
    {
        yield return new WaitForSeconds(basicAttackCoolDown);

        canBasicAttack = true;
    }

    IEnumerator Ability1CoolDown()
    {
        yield return new WaitForSeconds(ability1CoolDown);
        canAbility1 = true;
    }

    void PlayerHurtState()
    {

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
        if (Input.GetKey(basicAttackKey) && canBasicAttack)
        {
            canBasicAttack = false;
            state = PlayerState.attack;
            StartCoroutine(BasicAttackCoolDown());
        }
    }

    public void DashKeyPressed()
    {
        bool held = Input.GetKeyUp(dashKey);

        // Dash Key Pressed
        if (Input.GetKey(dashKey) && canDash)
        {
            dashIndicator.SetActive(true);
        }

        // Dash Key Held
        if (held && canDash)
        {
            dashIndicator.SetActive(false);
            state = PlayerState.dash;
        }
    }

    public void Ability1KeyPressed()
    {
        bool held = Input.GetKeyUp(ability1Key);

        if (Input.GetKey(ability1Key) && canAbility1)
        {
            ability1Indicator.SetActive(true);
        }

        if (held && canAbility1)
        {
            ability1Indicator.SetActive(false);
            state = PlayerState.ability1;
        }
    }

    #endregion

    #region Helper Methods
    public void AngleToMouse()
    {
        // Calculates the difference between the mouse position and player position
        // Not Normalized
        angleToMouse = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    }

    public void Blink()
    {
        // Instantly Teleport in direction of movement keys
        // Does not teleport if no movment keys are being pressed
        rb.MovePosition(rb.position + movement * dashVelocity);
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
        if (Vector3.Distance(rb.position, cam.ScreenToWorldPoint(Input.mousePosition)) > attackRange)
        {
            // Normalize movement vector and times it by attack move distance
            angleToMouse = angleToMouse.normalized * basicAttackSlideForce;

            // Disables collision of Player and Enemy
            Physics2D.IgnoreLayerCollision(3, 6, true);

            // Slide in Attack Direction
            rb.MovePosition(rb.position + angleToMouse);
        }

        if (Input.GetKey(upKey) || Input.GetKey(leftKey) || Input.GetKey(downKey) || Input.GetKey(rightKey))
        {
            // Normalize movement vector and times it by attack move distance
            angleToMouse = angleToMouse.normalized * basicAttackSlideForce;

            // Disables collision of Player and Enemy
            Physics2D.IgnoreLayerCollision(3, 6, true);

            // Slide in Attack Direction
            rb.MovePosition(rb.position + angleToMouse);
        }
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

    #endregion

    #region Animation Events
    public void AE_BasicAttack()
    {
        isBasicAttacking = true;
    }

    public void AE_BasicAttack2()
    {
        canBasicAttack2 = true;
    }

    public void AE_BasicAttack3()
    {
        canBasicAttack3 = true;
    }

    public void AE_BasicAttackEnd()
    {
        canBasicAttack2 = false;
        canBasicAttack3 = false;
        state = PlayerState.idle;
    }

    public void AE_Ability1()
    {
        isAbility1Active = true;
    }

    #endregion
}
