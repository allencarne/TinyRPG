using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] int maxHealth;
    [SerializeField] int health;
    [SerializeField] int moveSpeed;
    [HideInInspector] Vector2 movement;
    public bool isPlayerAlive;
    public bool canMove;
    public float offset;
    Vector3 moveDir;
    Vector2 mousePos;

    [SerializeField] float basicAttackCoolDown;
    [SerializeField] float basicAttackForce;
    bool isBasicAttackKeyDown;
    bool canBasicAttack;

    [SerializeField] float dashCoolDown;
    [SerializeField] float dashVelocity;
    bool isDashKeyDown;
    bool canDash;

    [Header("Components")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform firePoint;
    Camera cam;

    [SerializeField] GameObject basicAttackPrefab;

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

        //Quaternion rot = Quaternion.FromToRotation(Vector3.up, moveDir);
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // Dash Key Pressed
        if (Input.GetKeyDown(dashKey) && canDash && isPlayerAlive)
        {
            isDashKeyDown = true;
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            // Input
            Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            movement = moveInput.normalized * moveSpeed;

            // Movement
            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
        }

        /*
        // Movement
        if (canMove)
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

            rb.velocity = moveDir * moveSpeed;
        }
        */
        // Rotation
        if (isPlayerAlive)
        {
            Vector2 lookDir = mousePos - rb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - offset;
            rb.rotation = angle;
        }

        // Dash
        if (isDashKeyDown)
        {
            StartCoroutine(DashCoolDown());
        }

        IEnumerator DashCoolDown()
        {
            isDashKeyDown = false;
            canDash = false;
            //Instantiate(dashPrefab, transform.position, rot);
            rb.MovePosition(transform.position + moveDir * dashVelocity);

            yield return new WaitForSeconds(dashCoolDown);

            canDash = true;
        }
    }

    public void PlayerIdleState()
    {
        canMove = false;
        MoveKeyPressed();
        AttackKeyPressed();
    }

    public void PlayerMoveState()
    {
        canMove = true;
        NoMoveKeyPressed();
        AttackKeyPressed();
    }

    public void PlayerAttackState()
    {
        canMove = false;
        if (canBasicAttack)
        {
            StartCoroutine(BasicAttackCoolDown());
        }
    }

    IEnumerator BasicAttackCoolDown()
    {
        canBasicAttack = false;
        GameObject basicAttack = Instantiate(basicAttackPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = basicAttack.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.right * basicAttackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(basicAttackCoolDown);

        canBasicAttack = true;
        state = PlayerState.idle;
    }

    public void PlayerDashState()
    {

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
        if (Input.GetKey(upKey) || Input.GetKey(leftKey) || Input.GetKey(downKey) || Input.GetKey(rightKey))
        {
            state = PlayerState.move;
        }
    }

    public void NoMoveKeyPressed()
    {
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

    }
}
