using UnityEngine;

public class SentryEnemy : MonoBehaviour
{
    [Header("Settings")]
    public float attackCooldown = 2f;
    public GameObject ballPrefab;
    public Transform throwPoint;
    
    private float timer;
    private bool playerInRange;
    private Animator anim;
    private Transform playerTransform;

    void Start()
    {
        anim = GetComponent<Animator>();
        timer = attackCooldown;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (playerInRange)
        {
            LookAtPlayer();
            if (timer >= attackCooldown)
            {
                timer = 0;
                anim.SetTrigger("Attack");
            }
        }
    }

    void LookAtPlayer()
    {
        if (playerTransform == null) return;

        // Check if player is to the Right or Left of the enemy
        if (playerTransform.position.x > transform.position.x)
        {
            // Player is to the RIGHT
            // Since original sprite faces Right, set Scale X to 1
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            // Player is to the LEFT
            // Set Scale X to -1 to flip the whole object
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // THIS FUNCTION IS CALLED BY ANIMATION EVENT
    // Must be PUBLIC
    public void ThrowProjectile() 
    {
        if (SoundManager.instance != null)
            SoundManager.instance.PlaySound(SoundManager.instance.enemyShoot);

        GameObject ball = Instantiate(ballPrefab, throwPoint.position, Quaternion.identity);
        float dir = transform.localScale.x; 
        ball.GetComponent<EnemyBall>().SetDirection(dir);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}