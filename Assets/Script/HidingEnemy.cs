using UnityEngine;

public class HidingEnemy : MonoBehaviour
{
    [Header("Settings")]
    public float detectionRange = 5f;
    public bool isHidden = false;

    private Transform player; 
    private SpriteRenderer sr;
    private EnemyHealth health;
    private Animator anim; // New reference for the Animator

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        health = GetComponent<EnemyHealth>();
        anim = GetComponent<Animator>(); // Get the Animator component
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) 
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Logic: Hide if player is close
        if (distance <= detectionRange)
        {
            SetHiddenState(true);
        }
        else
        {
            SetHiddenState(false);
        }
    }

    void SetHiddenState(bool hide)
    {
        // Optimization: Only update if the state actually changed
        if (isHidden == hide) return;

        isHidden = hide;
        
        // 1. Tell the Animator to switch clips
        // Make sure the name "IsHidden" matches exactly what you typed in the Animator window!
        if (anim != null)
        {
            anim.SetBool("isHidden", isHidden);
        }

        // 2. Tell the Health script if we can take damage
        if (health != null)
        {
            health.canTakeDamage = !isHidden; 
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}