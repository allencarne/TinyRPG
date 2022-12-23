using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] int moveSpeed;
    [HideInInspector] Vector2 movement;

    [HideInInspector] Vector2 mousePos;
    [HideInInspector] float offset;

    [Header("Components")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject weapon;
    [HideInInspector] Camera cam;
    [SerializeField] Animator animator;

    [Header("Basic Attack")]
    [SerializeField] GameObject basicAttackPrefab;
    [SerializeField] float basicAttackSlideVelocity;
    [SerializeField] float basicAttackCoolDown;
    [SerializeField] float basicAttackForce;
    [SerializeField] float attackRange;
    public static bool canBasicAttack;

    [Header("Basic Attack2")]
    [SerializeField] GameObject basicAttack2Prefab;
    public static bool canBasicAttack2;

    [Header("Basic Attack3")]
    [SerializeField] GameObject basicAttack3Prefab;
    public static bool canBasicAttack3;

    [Header("Dash")]
    [SerializeField] float dashCoolDown;
    [SerializeField] float dashVelocity;
    [HideInInspector] bool canDash;

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

    void Start()
    {
        canBasicAttack = true;
        canBasicAttack2 = false;
        canDash = true;
    }

    void Update()
    {
        Debug.Log(state);
        Debug.Log(canBasicAttack2);

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

        // Animation Event trigger for Basic Attack
        if (BasicAttack.basicAttackTrigger && state == PlayerState.attack)
        {
            BasicAttack.basicAttackTrigger = false;
            weapon.SetActive(true);
            state = PlayerState.idle;
        }

        if (BasicAttack.basicAttack2Trigger && state == PlayerState.attack2)
        {
            BasicAttack.basicAttack2Trigger = false;
            weapon.SetActive(true);
            state = PlayerState.idle;
        }

        if (BasicAttack.basicAttack3Trigger && state == PlayerState.attack3)
        {
            BasicAttack.basicAttack3Trigger = false;
            weapon.SetActive(true);
            state = PlayerState.idle;
        }

        if (canBasicAttack2)
        {
            if (Input.GetMouseButtonDown(0))
            {
                state = PlayerState.attack2;
            }
        }

        if (canBasicAttack3)
        {
            if (Input.GetMouseButtonDown(0))
            {
                state = PlayerState.attack3;
            }

        }

    }

    /////// States \\\\\\\

    void PlayerIdleState()
    {
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
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);

        // Transitions
        NoMoveKeyPressed();
        AttackKeyPressed();
        DashKeyPressed();
    }

    void PlayerAttackState()
    {
        if (canBasicAttack)
        {
            // Animation
            animator.Play("Attack");

            // Calculate the difference between mouse position and player position
            Vector2 difference = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

            // Set Attack Animation Depending on Mouse Position
            animator.SetFloat("Aim Horizontal", difference.x);
            animator.SetFloat("Aim Vertical", difference.y);
            // Set Idle to last attack position
            animator.SetFloat("Horizontal", difference.x);
            animator.SetFloat("Vertical", difference.y);

            // Prevents Attacking more than once
            canBasicAttack = false;
            weapon.SetActive(false);

            SlideForwad();

            // Instantiate Basic Attack and Add Force
            GameObject basicAttack = Instantiate(basicAttackPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D basicAttackRB = basicAttack.GetComponent<Rigidbody2D>();
            basicAttackRB.AddForce(firePoint.right * basicAttackForce, ForceMode2D.Impulse);
        }
    }

    void PlayerAttack2State()
    {
        if (canBasicAttack2)
        {
            // Animation
            animator.Play("Attack");

            // Calculate the difference between mouse position and player position
            Vector2 difference = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

            // Set Attack Animation Depending on Mouse Position
            animator.SetFloat("Aim Horizontal", difference.x);
            animator.SetFloat("Aim Vertical", difference.y);
            // Set Idle to last attack position
            animator.SetFloat("Horizontal", difference.x);
            animator.SetFloat("Vertical", difference.y);

            // Prevents attacking more than once
            canBasicAttack2 = false;
            weapon.SetActive(false);

            SlideForwad();

            // Instantiate Basic Attack and Add Force
            GameObject basicAttack2 = Instantiate(basicAttack2Prefab, firePoint.position, firePoint.rotation);
            Rigidbody2D basicAttack2RB = basicAttack2.GetComponent<Rigidbody2D>();
            basicAttack2RB.AddForce(firePoint.right * basicAttackForce, ForceMode2D.Impulse);
        }
    }

    void PlayerAttack3State()
    {
        if (canBasicAttack3)
        {
            // Animation
            animator.Play("Attack");

            // Calculate the difference between mouse position and player position
            Vector2 difference = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

            // Set Attack Animation Depending on Mouse Position
            animator.SetFloat("Aim Horizontal", difference.x);
            animator.SetFloat("Aim Vertical", difference.y);
            // Set Idle to last attack position
            animator.SetFloat("Horizontal", difference.x);
            animator.SetFloat("Vertical", difference.y);

            // Prevents attacking more than once
            canBasicAttack3 = false;
            weapon.SetActive(false);

            SlideForwad();

            // Instantiate Basic Attack and Add Force
            GameObject basicAttack3 = Instantiate(basicAttack3Prefab, firePoint.position, firePoint.rotation);
            Rigidbody2D basicAttack3RB = basicAttack3.GetComponent<Rigidbody2D>();
            basicAttack3RB.AddForce(firePoint.right * basicAttackForce, ForceMode2D.Impulse);
        }
    }

    void PlayerDashState()
    {
        if (canDash)
        {
            // Prevents Dashing more than once
            canDash = false;

            // Input
            Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            movement = moveInput.normalized * moveSpeed;

            rb.MovePosition(rb.position + movement * dashVelocity);
            //rb.MovePosition(transform.position + moveDir * dashVelocity);

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
        // Dash Key Pressed
        if (Input.GetKey(dashKey) && canDash)
        {
            state = PlayerState.dash;
        }
    }

    // Helper Methods

    public void SlideForwad()
    {
        // Calculate the difference between mouse position and player position
        Vector2 difference = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        if (Vector3.Distance(rb.position, cam.ScreenToWorldPoint(Input.mousePosition)) > attackRange)
        {
            // Normalize movement vector and times it by attack move distance
            difference = difference.normalized * basicAttackSlideVelocity;
            // Slide in Attack Direction
            rb.AddForce(difference, ForceMode2D.Impulse);
        }
    }
}
