using UnityEngine;

public class FullHealthPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Check if the object touching the heart is the Player
        if (other.CompareTag("Player"))
        {
            // 2. Look for the PlayerHealth script (Updated from 'Health' to 'PlayerHealth')
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                // 3. Check if the player actually needs healing
                if (playerHealth.currentHealth < playerHealth.maxHealth)
                {
                    playerHealth.RestoreFullHealth();
                    
                    Debug.Log("Health fully restored!");
                    
                    // 4. Destroy the pickup after use
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Health already full!");
                }
            }
        }
    }
}