using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("CharacterDetails")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float doubleJumpForce = 12f;
    [SerializeField] private int numberOfExtraJump = 1;

    [Header("Movement Details")]
    [SerializeField] private bool isAirBorne;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isWallDetected;
    [SerializeField] private int extraJump = 1;
    [SerializeField] private int facingDirection = 1;
    [SerializeField]private float xInput;
    [SerializeField]private float yInput;

    [Header("Collision")]
    [SerializeField] private float wallDetectDistance = 0.5f;
    [SerializeField] private float groundCheckDistance = 0.8f;
    [SerializeField] private LayerMask groundLayer;
  

    
    private bool facingRight = true;
   
    


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAirBorneStatus();
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
        }
    }

    private void HandleLanding()
    {
        isAirBorne = false;
        extraJump = numberOfExtraJump;
    }

    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, wallDetectDistance, groundLayer);
    }

    private void HandleInput() 
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PressJump();
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


        if(facingRight && xInput < 0 || !facingRight && xInput > 0) 
        {
            Flip();        
        }

        HandleWallSlide();

        Run();

    }

    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.velocity.y     < 0;
        float yModifier = yInput < 0 ? 1f : 0.5f; 
        if(!canWallSlide) { return; }

        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * yModifier);
    }

    private void PressJump()
    {
        if(isGrounded)
        {
            Jump();
        }else if(extraJump > 0)
        {
            DoubleJump();
        }
    }

    private void DoubleJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
        extraJump -= 1;
    }

    private void Jump() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    private void Run() => rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);

    private void Flip()
    {
        facingDirection = facingDirection * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallDetectDistance * facingDirection), transform.position.y));
    }
}
