using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    private float moveDir = 1f;

    void Start() => Destroy(gameObject, lifeTime);

    public void SetDirection(float dir)
    {
        moveDir = dir;
        transform.localScale = new Vector3(dir, 1, 1);
    }

    void Update() => transform.Translate(Vector2.right * moveDir * speed * Time.deltaTime);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // If we hit a circle with a large radius, it's just the detection zone. 
            // Keep flying!
            if (other is CircleCollider2D circle && circle.radius > 2f) 
            {
                return; 
            }

            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null) enemy.TakeDamage(1);
            
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}