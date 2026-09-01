using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    [Header("Movement Settings")]
    [HideInInspector] public float moveInput;
    [HideInInspector] public bool jumpPressed;
    public bool canMove = false; 
    public float normalMoveSpeed = 7f;

    [Header("Abilities")]
    public bool hasShootingAbility = false;
    public bool hasDrillAbility = false; 

    [Header("Drill Settings")]
    public float drillSpeed = 25f; 
    public float drillDuration = 0.3f; 
    public float drillCooldown = 1.0f;
    public int drillDamage = 5;
    private bool isDrilling = false; 
    private float nextDrillTime; 
    private bool hasDealtDamageThisDrill = false;

    [Header("Invincibility Settings")]
    public bool isInvincible = false;
    public float invincibilityDuration = 0.5f; 
    private float invincibilityTimer = 0f;
    private SpriteRenderer[] allSpriteRenderers;
    private Color originalColor;

    [Header("Animation Timings")]
    [SerializeField] private float shootDuration = 0.3f;
    private float shootTimer;

    // Components
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Movement movement; // Added reference to Movement script
    
    private bool isFacingLeft = false;
    
    private Vector3 originalFirePointLocalPos;
    private Vector3 originalDrillPointLocalPos;
    private Transform firePoint;
    private Transform drillPoint;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movement = GetComponent<Movement>(); // Initialize movement reference
        
        allSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
        
        firePoint = transform.Find("FirePoint");
        drillPoint = transform.Find("DrillPoint");
        
        if (firePoint != null)
            originalFirePointLocalPos = firePoint.localPosition;
        if (drillPoint != null)
            originalDrillPointLocalPos = drillPoint.localPosition;
        
        isFacingLeft = false;
        UpdateFacingDirection();
        SetupDrillHitbox();
    }
    
    void SetupDrillHitbox()
    {
        if (drillPoint != null)
        {
            if (drillPoint.GetComponent<Collider2D>() == null)
            {
                CircleCollider2D collider = drillPoint.gameObject.AddComponent<CircleCollider2D>();
                collider.isTrigger = true;
                collider.radius = 0.5f;
            }
            
            if (drillPoint.GetComponent<DrillHitbox>() == null)
            {
                drillPoint.gameObject.AddComponent<DrillHitbox>();
            }
        }
    }

    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0) SetInvincible(false);
            if (isInvincible)
            {
                float blinkRate = 0.1f;
                bool visible = Mathf.FloorToInt(Time.time / blinkRate) % 2 == 0;
                SetSpriteVisibility(visible);
            }
        }
        
        if (shootTimer > 0) shootTimer -= Time.deltaTime;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerVictory"))
        {
            moveInput = 0;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return; 
        }

        if (!canMove) 
        {
            moveInput = 0;
            jumpPressed = false;
            UpdateAnimator();
            return;
        }
        
        if (hasDrillAbility && Input.GetKeyDown(KeyCode.C) && !isDrilling && Time.time > nextDrillTime)
        {
            StartCoroutine(ExecuteDrill());
        }

        if (hasShootingAbility && Input.GetKeyDown(KeyCode.X)) 
        {
            shootTimer = shootDuration;
            GetComponent<Shooter>()?.Fire(); 
        }

        if (!isDrilling) 
        {
            moveInput = Input.GetAxisRaw("Horizontal");
            jumpPressed = Input.GetButtonDown("Jump");

            if (moveInput > 0.01f) isFacingLeft = false;
            else if (moveInput < -0.01f) isFacingLeft = true;

            rb.linearVelocity = new Vector2(moveInput * normalMoveSpeed, rb.linearVelocity.y);
        }

        UpdateAnimator();
    }

    void LateUpdate()
    {
        UpdateFacingDirection();
    }

    void UpdateFacingDirection()
    {
        if (spriteRenderer != null) spriteRenderer.flipX = isFacingLeft;
        
        if (firePoint != null)
        {
            Vector3 firePointPos = originalFirePointLocalPos;
            firePointPos.x = Mathf.Abs(firePointPos.x) * (isFacingLeft ? -1f : 1f);
            firePoint.localPosition = firePointPos;
        }
        
        if (drillPoint != null)
        {
            Vector3 drillPointPos = originalDrillPointLocalPos;
            drillPointPos.x = Mathf.Abs(drillPointPos.x) * (isFacingLeft ? -1f : 1f);
            drillPoint.localPosition = drillPointPos;
        }
    }

    IEnumerator ExecuteDrill()
    {
        isDrilling = true; 
        hasDealtDamageThisDrill = false;
        nextDrillTime = Time.time + drillCooldown; 
        SetInvincible(true);
        
        float originalGravity = rb.gravityScale; 
        rb.gravityScale = 0; 
        float direction = isFacingLeft ? -1f : 1f;
        rb.linearVelocity = new Vector2(direction * drillSpeed, 0); 
        anim.SetTrigger("Drill");
        
        if (drillPoint != null)
        {
            Collider2D col = drillPoint.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;
        }
        
        yield return new WaitForSeconds(drillDuration); 
        
        if (drillPoint != null)
        {
            Collider2D col = drillPoint.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }
        
        rb.gravityScale = originalGravity; 
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
        isDrilling = false;
        
        yield return new WaitForSeconds(invincibilityDuration);
        SetInvincible(false);
    }
    
    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
        if (!invincible) SetSpriteVisibility(true);
    }
    
    public void StartInvincibility(float duration)
    {
        SetInvincible(true);
        invincibilityTimer = duration;
    }
    
    void SetSpriteVisibility(bool visible)
    {
        if (allSpriteRenderers != null)
        {
            foreach (SpriteRenderer sr in allSpriteRenderers)
            {
                if (sr != null)
                {
                    Color color = sr.color;
                    color.a = visible ? 1f : 0.3f;
                    sr.color = color;
                }
            }
        }
    }
    
    public void DealDrillDamage(GameObject enemy)
    {
        if (!hasDealtDamageThisDrill)
        {
            hasDealtDamageThisDrill = true;
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null) health.TakeDamage(drillDamage);
        }
    }
    
    public bool IsInvincible() => isInvincible;
    public bool IsDrilling() => isDrilling;
    public bool IsFacingLeft() => isFacingLeft;
    public int GetDrillDamage() => drillDamage;

    void UpdateAnimator()
    {
        if (anim == null) return;
        
        bool isWalking = Mathf.Abs(moveInput) > 0.1f;
        bool isShooting = shootTimer > 0;
        
        // Use the Grounded state from the Movement script
        bool isGrounded = (movement != null) ? movement.IsGrounded() : false;

        if (anim.GetBool("isFinished")) return;
        
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isShooting", isShooting);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isDrilling", isDrilling);
    }

    public void EnableMovement() { canMove = true; }
    
    public void SetFacingDirection(bool faceLeft)
    {
        isFacingLeft = faceLeft;
        UpdateFacingDirection();
    }
    
    void OnDestroy() { SetSpriteVisibility(true); }
}