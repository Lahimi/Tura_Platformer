using UnityEngine;

public class AerialPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 3f;
    
    private Vector3 target;

    void Start()
    {
        // Start by heading toward Point B
        target = pointB.position;
    }

    void Update()
    {
        // 1. Move the enemy toward the current target
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // 2. Check if we reached the target
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            // Switch targets
            target = (target == pointA.position) ? pointB.position : pointA.position;
            Flip();
        }
    }

    void Flip()
    {
        // Flip the scale so the enemy faces the direction it is moving
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the flying enemy touches the player, deal damage
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null) player.TakeDamage(1);
        }
    }
}