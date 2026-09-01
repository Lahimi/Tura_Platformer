using UnityEngine;

public class ThrowingEnemy : MonoBehaviour
{
    [Header("Detection")]
    public float range = 8f;
    
    [Header("References")]
    public GameObject pickaxePrefab;
    public Transform firePoint;
    
    private Animator anim;
    private Transform player;
    private float nextFireTime;
    public float fireRate = 2f;

    void Start()
    {
        anim = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // If player is in range and we aren't cooling down
        if (dist <= range && Time.time >= nextFireTime)
        {
            // Trigger the animation
            anim.SetTrigger("Throw");
            nextFireTime = Time.time + fireRate;
        }
    }

    // THIS IS CALLED BY THE ANIMATION EVENT
    public void LaunchPickaxe()
    {
        if (player == null) return;

        if (SoundManager.instance != null)
        SoundManager.instance.PlaySound(SoundManager.instance.enemyShoot);

        // 1. Get the player's position but FORCE the Y to match the firePoint
        Vector3 targetPosition = new Vector3(player.position.x, firePoint.position.y, 0);
        
        // 2. Calculate direction to that adjusted horizontal point
        Vector2 direction = ((Vector2)targetPosition - (Vector2)firePoint.position).normalized;

        // 3. Spawn and Launch
        GameObject go = Instantiate(pickaxePrefab, firePoint.position, Quaternion.identity);
        go.transform.SetParent(null);

        Pickaxe script = go.GetComponent<Pickaxe>();
        if (script != null)
        {
            script.Launch(direction);
        }
    }
}