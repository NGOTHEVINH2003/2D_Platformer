using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static event EventHandler OnAnyPlayerDeath;

    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D cd;
    private SpriteRenderer spriteRenderer;


    private bool canBeControlled = false;
    private float DefaultGravityScale;

    [Header("CharacterDetails")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float jumpForce = 15f;
    
    [Header("Jumping Details")]
    [SerializeField] private float doubleJumpForce = 12f;
    [SerializeField] private int numberOfExtraJump = 1;

    private float bufferJumpWindow = .25f;
    private float bufferJumpPressed = -1;
    private float coyoteJumpWindow = .5f;
    private float coyoteJumpPressed = -1;
    [Header("Wall Interaction")]
    [SerializeField]private float wallJumpDuration = .3f;
    [SerializeField] private Vector2 wallJumpForce;
    private bool isWallJumping;

    [Header("Get Hit Details")]
    [SerializeField] private float knockBackDuration = 1f;
    [SerializeField] private Vector2 knockBackForce;
    private bool isHit = false;
    [Header("Character Status Details")]
    [SerializeField] private bool isAirBorne;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isWallDetected;
    [SerializeField] private int extraJump = 1;
    [SerializeField] private int facingDirection = 1;
    private float xInput;
    private float yInput;

    [Header("Collision")]
    [SerializeField]private float wallDetectDistance = 0.5f;
    [SerializeField] private float groundCheckDistance = 0.8f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 Size;

    [Header("Death Handle")]
    [SerializeField] private GameObject DeathVFX;

    
    private bool facingRight = true;
    private bool jumpAvailable = true;
   
    


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        cd = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        DefaultGravityScale = rb.gravityScale;
        FinishedRespawn(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAirBorneStatus();
        if (!canBeControlled)
        {
            HandleCollision();
            HandleAnimation();
            return;
        }
        if (isHit) return;
        HandleCollision();
        HandleInput();
        HandleAnimation();
        HandleMovement();
    }

    

    private void UpdateAirBorneStatus()
    {
        if(isAirBorne && isGrounded)
        {
            HandleLanding();
        }
        if(!isGrounded && !isAirBorne)
        {
            isAirBorne = true;
            if (rb.velocity.y < 0) ActivateCoyoteJump();
        }
    }

    private void HandleLanding()
    {
        isAirBorne = false;
        extraJump = numberOfExtraJump;
        jumpAvailable = true;

        AttemptBufferJump();
    }

    private void HandleCollision()
    {
        isGrounded = Physics2D.BoxCast(transform.position, Size, 0f, Vector2.down , groundCheckDistance, groundLayer) && (rb.velocity.y < 1 && rb.velocity.y > -1);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, wallDetectDistance, groundLayer);
    }

    public void FinishedRespawn(bool finished)
    {
        if (finished)
            rb.gravityScale = DefaultGravityScale;
        else rb.gravityScale = 0;

        cd.enabled = finished;
        canBeControlled = finished;
    }
   
    private void HandleInput() 
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PressJump();
            RequestBufferJump();
        }
    }

    private void HandleAnimation()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }

    private void HandleMovement()
    {
        if (isWallJumping) return;

        if (facingRight && xInput < 0 || !facingRight && xInput > 0) 
        {
            Flip();        
        }

        HandleWallSlide();

        if (isWallDetected) return;

        Run();

    }

    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.velocity.y < 0;
        float yModifier = yInput < 0 ? 1f : 0.1f; 
        if(!canWallSlide) { return; }

        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * yModifier);
    } 
    private void PressJump()
    {
        bool coyoteJumpAvailable = Time.time < coyoteJumpPressed + coyoteJumpWindow;

        if (isGrounded || coyoteJumpAvailable)
        {
            Jump();
        }else if (isWallDetected && !isGrounded)
        {
            WallJump();
        }
        else if(extraJump > 0)
        {
            DoubleJump();
        }

        CancelCoyoteJump();
    }
    #region Jumping Logic.
    private void DoubleJump()
    {
        isWallJumping = false;
        anim.SetTrigger("DoubleJump");
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
        extraJump -= 1;
    }
    private void WallJump()
    {
        rb.velocity = new Vector2(wallJumpForce.x * -facingDirection, wallJumpForce.y);
        //Allow 1 extra jump per landing
        if (jumpAvailable)
        {
            extraJump += 1;
            jumpAvailable = false;
        }
        Flip();
        StopAllCoroutines();
        StartCoroutine(WallJumpCoroutine());
    }
    private void AttemptBufferJump()
    {
        if (Time.time < bufferJumpPressed + bufferJumpWindow)
        {
            bufferJumpPressed -= 1;
            Jump();
        }
    }
    private IEnumerator WallJumpCoroutine()
    {
        isWallJumping = true;
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;

    }
    private void RequestBufferJump()
    {
        if (isAirBorne) bufferJumpPressed = Time.time;
    }
    private void ActivateCoyoteJump() => coyoteJumpPressed = Time.time;
    private void CancelCoyoteJump() => coyoteJumpPressed -= Time.time -1;
    private void Jump() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    #endregion

    #region KnockBack Handling
    public void KnockBack(float sourceDamageXPosition)
    {
        float knockDir = 1;
        if(transform.position.x < sourceDamageXPosition) knockDir = -1;
        if (isHit) return;

        StartCoroutine(GetHitCoroutine());

        rb.velocity = new Vector2(knockBackForce.x * knockDir, knockBackForce.y);
        anim.SetTrigger("Hit");
    }

    public void GetPush(Vector2 direction, float duration)
    {
        StartCoroutine(PushCoroutine(direction, duration));
    }
    private IEnumerator PushCoroutine(Vector2 direction, float duration)
    {
        canBeControlled = false;

        Vector2 newVelocity = rb.velocity;
        if(newVelocity.y < 0 && direction.y > 0) 
        { 
            newVelocity.y = 0;
        }


        rb.velocity = newVelocity;
        rb.AddForce(direction, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        canBeControlled = true;
    }

    public void PlayerDeath()
    {
        Instantiate(DeathVFX, transform.position, Quaternion.identity);
        OnAnyPlayerDeath?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    private IEnumerator GetHitCoroutine()
    {
        isHit = true;
        yield return new WaitForSeconds(knockBackDuration);
        anim.SetTrigger("GetUp");
        isHit = false;
    }
    #endregion

    private void Run() => rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);

    private void Flip()
    {
        facingDirection = facingDirection * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((Vector2)transform.position + Vector2.down * groundCheckDistance , Size);
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallDetectDistance * facingDirection), transform.position.y));
    }
}
