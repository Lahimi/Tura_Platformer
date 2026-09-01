using UnityEngine;

public class Pickaxe : MonoBehaviour
{
    public float speed = 12f; // Increased speed for a flatter trajectory
    public int damage = 1;
    private Rigidbody2D rb;

    public void Launch(Vector2 direction)
    {
        rb = GetComponent<Rigidbody2D>();
        
        // 1. COMPLETELY reset physics state
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0;
        
        // 2. Set Interpolation to None (This stops the "guessing" dive)
        rb.interpolation = RigidbodyInterpolation2D.None;
        
        // 3. Apply the straight velocity
        rb.linearVelocity = direction * speed;

        // 4. Point the front of the pickaxe at the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Ignore the enemy who threw it and any detection triggers
        if (other.CompareTag("Enemy")) return;
        
        // 2. NEW: Ignore the DrillPoint or other utility triggers on the player
        // We check if the name is "DrillPoint" or if it's a trigger that isn't the main player body
        if (other.isTrigger && other.name == "DrillPoint") return;

        // 3. Hit the Player
        if (other.CompareTag("Player"))
        {
            // Use GetComponentInParent to ensure we find the health script 
            // even if the collision happens on a child object
            PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        
        // 4. Hit the Ground
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}