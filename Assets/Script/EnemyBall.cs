using UnityEngine;

public class EnemyBall : MonoBehaviour
{
    public float speed = 8f;
    public float upwardForce = 5f;
    public float lifeTime = 4f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(float dir)
    {
        rb.linearVelocity = new Vector2(dir * speed, upwardForce);
        transform.localScale = new Vector3(dir, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}