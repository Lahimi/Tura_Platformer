using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class ChestEnemy : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 5f;
    
    [Header("Movement")]
    public float jumpForceX = 5f;
    public float jumpForceY = 8f;
    public float attackCooldown = 2.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Loot")]
    public GameObject itemToDrop;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;
    
    private bool isTransformed = false;
    private bool isJumping = false;
    private bool canAttack = false; 
    private float lastAttackTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        // Setup Rigidbody for Physics
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
        rb.gravityScale = 2.5f; // Makes the jump feel less "floaty"

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Standard Ground Check
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 1. WAKE UP: Transition from chest to mimic
        if (!isTransformed && Vector2.Distance(transform.position, player.position) < detectionRange)
        {
            StartCoroutine(WakeUpSequence());
        }

        // 2. JUMP ATTACK: Only if ready and touching ground
        if (canAttack && isGrounded && !isJumping && Time.time > lastAttackTime + attackCooldown)
        {
            if (Vector2.Distance(transform.position, player.position) < detectionRange)
            {
                StartCoroutine(PerformJump());
            }
        }

        // 3. LANDING DETECTION:
        // We only trigger 'Land' if we are FALLING (y < 0) and hit the floor.
        // This stops the animator from skipping the 'Jump' animation instantly.
        if (isJumping && rb.linearVelocity.y < 0.1f && isGrounded)
        {
            isJumping = false;
            anim.ResetTrigger("Jump"); // Clear the jump trigger so it doesn't loop
            anim.SetTrigger("Land");
        }
    }

    IEnumerator WakeUpSequence()
    {
        isTransformed = true;
        anim.SetTrigger("WakeUp");
        
        // Wait for the transformation clip to finish (adjust based on your clip length)
        yield return new WaitForSeconds(1.0f); 
        
        canAttack = true; 
    }

    IEnumerator PerformJump()
    {
        // 1. Prepare Animator
        anim.ResetTrigger("Land");
        anim.SetTrigger("Jump"); 

        // 2. Face the player
        float dir = (player.position.x < transform.position.x) ? -1 : 1;
        transform.localScale = new Vector3(dir, 1, 1);

        // 3. TINY DELAY (0.05s) 
        // Gives the Animator 1-2 frames to switch to the 'Attack' sprite before moving
        yield return new WaitForSeconds(0.05f);

        // 4. Launch into the air
        rb.linearVelocity = new Vector2(dir * jumpForceX, jumpForceY);
        isJumping = true; 
        lastAttackTime = Time.time;
    }

    // Call this from your health script when HP = 0
    public void Die()
    {
        if (itemToDrop != null)
        {
            // 1. Instantiate the prefab
            GameObject loot = Instantiate(itemToDrop, transform.position, Quaternion.identity);
            
            // 2. FORCE the loot to be active (Fixes the "unchecked" issue)
            loot.SetActive(true);

            // 3. Give it a physical "pop" out of the chest
            Rigidbody2D lootRb = loot.GetComponent<Rigidbody2D>();
            if (lootRb != null) 
            {
                // Ensure the Rigidbody isn't asleep or static
                lootRb.bodyType = RigidbodyType2D.Dynamic; 
                lootRb.linearVelocity = new Vector2(Random.Range(-2f, 2f), 5f);
            }
        }
        Destroy(gameObject);
    }

    // Trigger damage if the player touches the chest
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // other.GetComponent<PlayerHealth>()?.TakeDamage(1);
            Debug.Log("Mimic Damage Dealt!");
        }
    }
}