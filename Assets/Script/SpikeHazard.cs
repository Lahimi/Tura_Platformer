using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1; // Set to 99 for instant kill
    public float knockbackForce = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object hitting the spikes is the Player
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            
            if (health != null)
            {
                // Apply damage
                health.TakeDamage(damageAmount);

                // Optional: Add a little "bounce" so they don't get stuck in the spikes
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, knockbackForce); 
                }
            }
        }
    }
}