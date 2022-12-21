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

    [Header("Basic Attack")]
    [SerializeField] GameObject basicAttackPrefab;
    [SerializeField] float basicAttackSlideVelocity;
    [SerializeField] float basicAttackCoolDown;
    [SerializeField] float basicAttackForce;
    [SerializeField] float attackRange;
    public static bool canBasicAttack;

    //[Header("Basic Attack2")]
    //[SerializeField] GameObject basicAttack2Prefab;
    //public static bool canBasicAttack2;
    //bool isBasicAttack2 = false;

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
        canDash = true;
    }

    void Update()
    {
        Debug.Log(state);
        //Debug.Log(canBasicAttack2);

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

        // Rotation
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - offset;
        rb.rotation = angle;

        
        // Animation Event trigger for Basic Attack
        if (BasicAttack.basicAttackTrigger)
        {
            BasicAttack.basicAttackTrigger = false;

            canBasicAttack = true;
            weapon.SetActive(true);
            state = PlayerState.idle;
        }
        
    }

    /////// States \\\\\\\

    void PlayerIdleState()
    {
        // Tranitions
        MoveKeyPressed();
        AttackKeyPressed();
        DashKeyPressed();
    }

    void PlayerMoveState()
    {
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
            // Prevents Attacking more than once
            canBasicAttack = false;
            weapon.SetActive(false);

            SlideForwad();

            // Instantiate Basic Attack and Add Force
            GameObject basicAttack = Instantiate(basicAttackPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D basicAttackRB = basicAttack.GetComponent<Rigidbody2D>();
            basicAttackRB.AddForce(firePoint.right * basicAttackForce, ForceMode2D.Impulse);

            /*
            // Basic Attack 2 Transition
            if (canBasicAttack2 && Input.GetKey(basicAttackKey))
            {
                canBasicAttack2 = false;
                isBasicAttack2 = true;
                state = PlayerState.attack2;
            }
            */
        }
    }

    void PlayerAttack2State()
    {

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
        }
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
