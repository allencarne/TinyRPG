using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] int moveSpeed;
    [HideInInspector] Vector2 movement;
    [HideInInspector] Vector2 difference;
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

    [Header("Basic Attack2")]
    [SerializeField] GameObject basicAttack2Prefab;
    bool canBasicAttack2 = false;

    [Header("Basic Attack3")]
    [SerializeField] GameObject basicAttack3Prefab;
    bool canBasicAttack3 = false;

    [Header("Dash")]
    [SerializeField] float dashCoolDown;
    [SerializeField] float dashVelocity;
    [SerializeField] GameObject dashIndicator;
    bool canDash = true;

    [Header("Keys")]
    [SerializeField] KeyCode basicAttackKey;
    [SerializeField] KeyCode dashKey;
    [SerializeField] KeyCode upKey;
    [SerializeField] KeyCode downKey;
    [SerializeField] KeyCode leftKey;
    [SerializeField] KeyCode rightKey;

    enum PlayerState
    {
        idle,
        move,
        attack,
        attack2,
        attack3,
        dash,
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
            case PlayerState.hurt:
                PlayerHurtState();
                break;
            case PlayerState.death:
                PlayerDeathState();
                break;
        }
    }

    /////// States \\\\\\\

    void PlayerIdleState()
    {
        // Enables collision of Player and Enemy
        Physics2D.IgnoreLayerCollision(3, 6, false);

        // Animation
        animator.Play("Idle");

        // Tranitions
        MoveKeyPressed();
        AttackKeyPressed();
        DashKeyPressed();
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
    }

    void PlayerAttackState()
    {
        // Animation
        animator.Play("Attack");

        // Calculate the difference between mouse position and player position
        Difference();
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
        Difference();
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
        Difference();
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
        if (canDash)
        {
            // Prevents Dashing more than once
            canDash = false;

            Difference();
            difference = difference.normalized * dashVelocity;

            // Logic
            rb.MovePosition(rb.position + difference);
            //rb.velocity = difference * moveSpeed;

            // Transition
            StartCoroutine(DashDelay());
            StartCoroutine(DashCoolDown());
        }
    }

    IEnumerator DashDelay()
    {
        yield return new WaitForSeconds(.1f);

        state = PlayerState.idle;
    }

    IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(dashCoolDown);

        canDash = true;
    }

    void PlayerHurtState()
    {

    }

    void PlayerDeathState()
    {

    }

    /////// Input \\\\\\\
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

    IEnumerator BasicAttackCoolDown()
    {
        yield return new WaitForSeconds(basicAttackCoolDown);

        canBasicAttack = true;
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
        if (held)
        {
            dashIndicator.SetActive(false);
            state = PlayerState.dash;
        }
    }

    /////// Helper Methods \\\\\\\
    public void Difference()
    {
        // Calculates the difference between the mouse position and player position
        // Not Normalized
        difference = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
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
        Difference();

        if (Vector3.Distance(rb.position, cam.ScreenToWorldPoint(Input.mousePosition)) > attackRange)
        {
            // Normalize movement vector and times it by attack move distance
            difference = difference.normalized * basicAttackSlideForce;

            // Disables collision of Player and Enemy
            Physics2D.IgnoreLayerCollision(3, 6, true);

            // Slide in Attack Direction
            rb.MovePosition(rb.position + difference);
        }
    }

    public void AnimationDirection()
    {
        // Set Attack Animation Depending on Mouse Position
        animator.SetFloat("Aim Horizontal", difference.x);
        animator.SetFloat("Aim Vertical", difference.y);
        // Set Idle to last attack position
        animator.SetFloat("Horizontal", difference.x);
        animator.SetFloat("Vertical", difference.y);
    }

    // Animation Events

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
}
