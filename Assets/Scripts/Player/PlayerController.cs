using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [Header("Movement")]
    public float moveSpeed = 5f;
    private float horizontalMovement;

    [Header("Dash")]
    public float dashingPower = 20f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;
    [SerializeField] private TrailRenderer tr;

    [Header("Jump")]
    public float jumpForce = 10f;
    public int maxJumps = 2;
    private int jumpsLeft;
    private bool isGrounded;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float fallMultiplier = 2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public Vector2 groundCheckSize = new (0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private const int ANIM_STATE_IDLE = 0;
    private const int ANIM_STATE_RUN = 2;

    private Rigidbody2D rb;
    private bool canCheckGround = true;
    [SerializeField] private float groundCheckDisableDuration = 0.2f;
    private MovingPlatformScript currentPlatform;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
            return; 
        }
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
            animator.SetTrigger("Recover");
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.stompSound);
            AudioManager.instance.PlaySFX(AudioManager.instance.respawn);
            AudioManager.instance.PlaySFX(AudioManager.instance.clothesSound);
        }

        jumpsLeft = maxJumps;
    }

    private void Update()
    {
        if (isDashing) return;
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            bool canGroundJump = isGrounded;
            bool canAirJump = !isGrounded && jumpsLeft > 0;
            
            if (canGroundJump)
            {
                DoJump();
                jumpsLeft = maxJumps - 1; // 1 jump left
            }
            else if (canAirJump) // double jump
            {
                DoJump();
                jumpsLeft--; 
            }
        }

        // dash logic
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }
    
    private void DoJump() // jump logic
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        if (animator != null)
        {
            animator.SetBool("Grounded", false);
            animator.SetTrigger("Jump");
        }
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.jump);

        if (isGrounded)
        {
            StartCoroutine(DisableGroundCheckForJump(groundCheckDisableDuration)); // bug fix for ground checking
        }
    }

    private void FixedUpdate() // used for horizontal movement
    {
        if (isDashing) return;

        horizontalMovement = Input.GetAxis("Horizontal");
        GroundCheck();
        ApplyPlatformVelocity();

        float platformVelocityX = currentPlatform != null && isGrounded ? currentPlatform.CurrentVelocity.x : 0f;
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed + platformVelocityX, rb.linearVelocity.y);

        Gravity();

        if (animator != null)
        {
            if (horizontalMovement > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (horizontalMovement < 0)
                transform.localScale = new Vector3(-1, 1, 1);

            animator.SetFloat("AirSpeed", rb.linearVelocity.y);
            animator.SetBool("Grounded", isGrounded);
            animator.SetInteger("AnimState", Mathf.Abs(horizontalMovement) > 0.01f ? ANIM_STATE_RUN : ANIM_STATE_IDLE);
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.dash);
        }
        
        // trail effect
        if (tr != null) tr.emitting = true; 

        yield return new WaitForSeconds(dashingTime);

        if (tr != null) tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void GroundCheck() // player can only jump when on ground
    {
        if (!canCheckGround)
        {
            isGrounded = false;
            return;
        }

        bool currentlyGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer); // helper for checking the current state

        if (currentlyGrounded && !isGrounded)
            jumpsLeft = maxJumps;

        // if player walks off a ledge, strip all air jumps until they land.
        if (isGrounded && !currentlyGrounded)
        {
             jumpsLeft = 0; 
        }

        isGrounded = currentlyGrounded; // save the current state
    }

    private IEnumerator DisableGroundCheckForJump(float duration)
    {
        canCheckGround = false;
        yield return new WaitForSeconds(duration);
        canCheckGround = true;
    }

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
            rb.gravityScale = baseGravity * fallMultiplier;
        else
            rb.gravityScale = baseGravity;
    }

    private void ApplyPlatformVelocity()
    {
        if (currentPlatform == null || !isGrounded)
            return;

        Vector2 platformVelocity = currentPlatform.CurrentVelocity;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, platformVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        UpdateCurrentPlatform(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        UpdateCurrentPlatform(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        MovingPlatformScript platform = collision.collider.GetComponent<MovingPlatformScript>();
        if (platform == null)
            platform = collision.collider.GetComponentInParent<MovingPlatformScript>();

        if (platform != null && platform == currentPlatform)
            currentPlatform = null;
    }

    private void UpdateCurrentPlatform(Collision2D collision)
    {
        MovingPlatformScript platform = collision.collider.GetComponent<MovingPlatformScript>();
        if (platform == null)
            platform = collision.collider.GetComponentInParent<MovingPlatformScript>();

        if (platform == null || !IsStandingOnTopOfPlatform(collision))
            return;

        currentPlatform = platform;
    }

    private bool IsStandingOnTopOfPlatform(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.5f)
                return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected() // visualization of the ground-check box
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
