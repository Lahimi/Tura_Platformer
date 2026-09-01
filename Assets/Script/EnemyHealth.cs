using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Stats")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("State")]
    public bool canTakeDamage = true; 

    [Header("SFX")]
    public AudioClip shieldedHitSFX;

    [Header("Effects")]
    public GameObject deathEffectPrefab; 

    [Header("UI (Boss Only)")]
    public Slider healthSlider;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!canTakeDamage)
        {
            if (SoundManager.instance != null && shieldedHitSFX != null)
                SoundManager.instance.PlaySound(shieldedHitSFX);
            return;
        }

        if (SoundManager.instance != null)
            SoundManager.instance.PlaySound(SoundManager.instance.enemyHit);

        currentHealth -= damage;

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);

        // Check for BossManager to trigger Victory
        BossManager bossManager = Object.FindFirstObjectByType<BossManager>();
        
        if (bossManager != null && gameObject == bossManager.bossObject)
        {
            if (deathEffectPrefab != null)
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            
            bossManager.StartDefeatSequence();
            gameObject.SetActive(false); 
            return;
        }

        // Standard logic for Chests and Mobs
        ChestEnemy chest = GetComponent<ChestEnemy>();
        if (chest != null)
        {
            chest.Die();
            return;
        }

        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}