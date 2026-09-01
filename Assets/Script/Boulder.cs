using UnityEngine;

public class Boulder : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 2;
    
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Stay still while forming
        rb.gravityScale = 0; 
        rb.linearVelocity = Vector2.zero;
    }

    public void Launch(Vector2 direction)
    {
        // 1. Detach from Boss
        transform.SetParent(null); 

        // 2. Move in the straight line provided by the Boss script
        rb.linearVelocity = direction * speed;

        // 3. Visual rotation
        rb.angularVelocity = 120f;

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Replace 'PlayerHealth' with the actual name of your health script
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }

        // Destroys boulder if it hits walls or floor
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}