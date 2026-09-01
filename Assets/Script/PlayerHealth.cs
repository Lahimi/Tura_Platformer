using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Essential for the Slider

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth; // Changed to public so FullHealthPickup can see it
    private bool isDead = false;

    [Header("UI Reference")]
    public Slider healthSlider; // Drag your Slider here in the Inspector

    [Header("Death Effects")]
    public GameObject deathParticlesPrefab; 

    [Header("I-Frames")]
    public float invincibilityDuration = 1.5f;
    private float invincibilityTimer;
    private SpriteRenderer sprite;
    
    // Add reference to PlayerInput
    private PlayerInput playerInput;

    void Start()
    {
        currentHealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>(); // Get reference

        // Initialize the UI Bar
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Update()
    {
        // I-Frame Flashing Logic
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
            float flash = Mathf.PingPong(Time.time * 10, 1);
            sprite.color = new Color(1, 1, 1, flash);
        }
        else if (!isDead)
        {
            sprite.color = Color.white;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        // 1. PIT FALL (Lethal Tag)
        if (other.CompareTag("Lethal"))
        {
            // Empty the bar visually before dying
            if (healthSlider != null) healthSlider.value = 0; 
            Die();
        }

        // 2. ENEMY CONTACT
        if (other.CompareTag("Enemy"))
        {
            // Ignore the large detection/aggro circles
            if (other is CircleCollider2D circle && circle.radius > 2f) return;

            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        // CRITICAL ADDITION: Check PlayerInput invincibility first
        if (isDead) return;
        
        // Check if player is invincible from drill or other effects
        if (playerInput != null && playerInput.IsInvincible())
        {
            Debug.Log("Player is invincible - no damage taken!");
            return;
        }
        
        // Check regular invincibility timer
        if (invincibilityTimer > 0) return;

        if (SoundManager.instance != null)
        SoundManager.instance.PlaySound(SoundManager.instance.playerHit);

        currentHealth -= damage;
        
        // Update the bar UI
        if (healthSlider != null) healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            invincibilityTimer = invincibilityDuration;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Trigger Death Sound
        if (SoundManager.instance != null)
        SoundManager.instance.PlaySound(SoundManager.instance.playerDeath);

        // Ensure bar is empty on death
        if (healthSlider != null) healthSlider.value = 0;

        // --- DEATH SEQUENCE ---
        if (deathParticlesPrefab != null)
        {
            GameObject effect = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; 
            rb.bodyType = RigidbodyType2D.Static; 
        }

        sprite.enabled = false;
        
        PlayerInput input = GetComponent<PlayerInput>();
        if (input != null) input.canMove = false;

        Invoke("RestartLevel", 1.2f);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;
        
        // Update the slider/UI if you have one
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }
}