using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    private bool isGrounded;
    private bool wasGrounded; // Track previous frame to detect the moment of landing

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 1. Physical ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 2. Landing SFX Logic
        // If grounded now but wasn't last frame, and we are not moving upwards
        if (isGrounded && !wasGrounded && rb.linearVelocity.y <= 0.1f)
        {
            if (SoundManager.instance != null)
                SoundManager.instance.PlaySound(SoundManager.instance.playerLand);
        }
        wasGrounded = isGrounded;

        // 3. Animations
        animator.SetBool("isWalking", playerInput.moveInput != 0);
        animator.SetBool("isGrounded", isGrounded);

        // 4. Jump Logic
        if (playerInput.jumpPressed && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // 5. Sprite Flipping
        if (playerInput.moveInput > 0)
            spriteRenderer.flipX = false;
        else if (playerInput.moveInput < 0)
            spriteRenderer.flipX = true;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(playerInput.moveInput * speed, rb.linearVelocity.y);
    }

    // This allows PlayerInput to see the ground status without needing its own sensors
    public bool IsGrounded() 
    {
        return isGrounded;
    }
}