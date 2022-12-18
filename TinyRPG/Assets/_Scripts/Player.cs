using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] int maxHealth;
    [HideInInspector] int health;
    [SerializeField] int moveSpeed;
    [HideInInspector] Vector2 movement;
    [HideInInspector] bool isPlayerAlive;
    [HideInInspector] bool canMove;
    [HideInInspector] float offset;
    [HideInInspector] Vector3 moveDir;
    [HideInInspector] Vector2 mousePos;

    [Header("Basic Attack")]
    [SerializeField] GameObject basicAttackPrefab;
    [SerializeField] float basicAttackSlideVelocity;
    [SerializeField] float basicAttackCoolDown;
    [SerializeField] float basicAttackForce;
    [SerializeField] float attackRange;
    [HideInInspector] bool canBasicAttack;

    [Header("Basic Attack2")]
    [SerializeField] GameObject basicAttack2Prefab;
    public static bool canBasicAttack2;
    bool isBasicAttack2;

    [Header("Dash")]
    [SerializeField] float dashCoolDown;
    [SerializeField] float dashVelocity;
    [HideInInspector] bool canDash;

    [Header("Components")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject weapon;
    [HideInInspector] Camera cam;

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

    // Start is called before the first frame update
    void Start()
    {
        canDash = true;
        canBasicAttack = true;
        canBasicAttack2 = false;
        isPlayerAlive = true;
    }

    // Update is called once per frame
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

        if (canBasicAttack2 && Input.GetKeyDown(basicAttackKey))
        {
            isBasicAttack2 = true;
            //state = PlayerState.attack2;
        }

        //Quaternion rot = Quaternion.FromToRotation(Vector3.up, moveDir);
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        if (isPlayerAlive)
        {
            float moveX = 0f;
            float moveY = 0f;

            if (Input.GetKey(upKey))
            {
                moveY = +1f;
            }
            if (Input.GetKey(downKey))
            {
                moveY = -1f;
            }
            if (Input.GetKey(leftKey))
            {
                moveX = -1f;
            }
            if (Input.GetKey(rightKey))
            {
                moveX = +1f;
            }

            moveDir = new Vector3(moveX, moveY).normalized;

            Vector2 lookDir = mousePos - rb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - offset;
            rb.rotation = angle;
        }
    }

    public void PlayerIdleState()
    {
        // Tranitions
        MoveKeyPressed();
        AttackKeyPressed();
        DashKeyPressed();
    }

    public void PlayerMoveState()
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

    public void PlayerAttackState()
    {
        //weapon.SetActive(false);
        if (canBasicAttack)
        {
            weapon.SetActive(false);

            canBasicAttack = false;

            SlideForwad();

            // Instantiate Basic Attack and Add Force
            GameObject basicAttack = Instantiate(basicAttackPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D basicAttackRB = basicAttack.GetComponent<Rigidbody2D>();
            basicAttackRB.AddForce(firePoint.right * basicAttackForce, ForceMode2D.Impulse);

            if (isBasicAttack2)
            {
                isBasicAttack2 = false;
                state = PlayerState.attack2;
            } else
            {
                StartCoroutine(BasicAttackCoolDown());
            }
        }
    }

    IEnumerator BasicAttackCoolDown()
    {
        yield return new WaitForSeconds(basicAttackCoolDown);

        canBasicAttack = true;
        canBasicAttack2 = false;
        weapon.SetActive(true);
        state = PlayerState.idle;
    }

    public void PlayerAttack2State()
    {
        weapon.SetActive(false);
        if (canBasicAttack2)
        {
            StartCoroutine(BasicAttack2CoolDown());
        }
    }

    IEnumerator BasicAttack2CoolDown()
    {
        canBasicAttack2 = false;

        SlideForwad();

        // Instantiate Basic Attack and Add Force
        GameObject basicAttack2 = Instantiate(basicAttack2Prefab, firePoint.position, firePoint.rotation);
        Rigidbody2D basicAttackRB = basicAttack2.GetComponent<Rigidbody2D>();
        basicAttackRB.AddForce(firePoint.right * basicAttackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(basicAttackCoolDown);

        canBasicAttack = true;
        weapon.SetActive(true);
        state = PlayerState.idle;
    }

    public void PlayerDashState()
    {
        if (canDash)
        {
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
        canDash = false;
        //Instantiate(dashPrefab, transform.position, rot);
        rb.MovePosition(transform.position + moveDir * dashVelocity);

        yield return new WaitForSeconds(dashCoolDown);

        canDash = true;
    }

    public void PlayerHurtState()
    {

    }

    public void PlayerDeathState()
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
        if (Input.GetKey(basicAttackKey) && isPlayerAlive && canBasicAttack)
        {
            state = PlayerState.attack;
        }
    }

    public void DashKeyPressed()
    {
        // Dash Key Pressed
        if (Input.GetKey(dashKey) && canDash && isPlayerAlive)
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
