using UnityEngine;

public class DrillHitbox : MonoBehaviour
{
    private PlayerInput player;
    
    // Optional: Make damage configurable per enemy type
    public bool canHitMultipleEnemies = false; // Set to true if you want to hit multiple enemies in one drill

    void Start()
    {
        // Find the PlayerInput component on the parent
        player = GetComponentInParent<PlayerInput>();
        
        if (player == null)
        {
            Debug.LogError("DrillHitbox: PlayerInput component not found on parent!");
        }
        
        // Ensure collider is set as trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("DrillHitbox: Collider should be set as Trigger!");
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only trigger if the player is currently in the drill state
        if (player != null && player.IsDrilling())
        {
            // Check for Enemy or Boss tags
            if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
            {
                // Try to get EnemyHealth component
                EnemyHealth health = other.GetComponent<EnemyHealth>();
                if (health != null)
                {
                    // Deal damage through PlayerInput
                    player.DealDrillDamage(other.gameObject);
                    Debug.Log("<color=green>DrillPoint hit: </color>" + other.name);
                }
                else
                {
                    Debug.LogWarning($"Enemy {other.name} has tag Enemy but no EnemyHealth component!");
                    // Fallback for special enemies like Chests
                    other.GetComponent<ChestEnemy>()?.Die();
                }
            }
        }
    }
    
    // Optional: Visual debugging
    private void OnDrawGizmos()
    {
        if (GetComponent<Collider2D>() != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, GetComponent<Collider2D>().bounds.extents.x);
        }
    }
}