using UnityEngine;
using System.Collections;

public class GoldBoss : MonoBehaviour
{
    [Header("Room Boundaries")]
    public float roomLeft = 36f;
    public float roomRight = 59f;
    
    [Header("Teleport Spots")]
    public float leftSpot = 37f;
    public float rightSpot = 59f;

    [Header("Attack Settings")]
    public GameObject boulderPrefab;
    public Transform firePoint;
    public float chargeTime = 1.5f; 
    public float idleDuration = 2f;

    private Animator anim;
    private Transform player;
    private bool playerInRoom = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        StartCoroutine(BossLoop());
    }

    void Update()
    {
        if (player == null) return;

        // 1. ROOM DETECTION & CAMERA LOCK
        if (player.position.x >= roomLeft && player.position.x <= roomRight)
        {
            if (!playerInRoom)
            {
                playerInRoom = true;
                // Locks camera to center (47) using your CameraFollow.cs
                Camera.main.GetComponent<CameraFollow>()?.TriggerBossLock(47f);
            }
        }

        // 2. CONSTANT TRACKING
        // Boss faces player during Idle and Charge phases
        if (playerInRoom)
        {
            FlipTowardsPlayer();
        }
    }

    IEnumerator BossLoop()
    {
        while (true)
        {
            if (!playerInRoom) yield return new WaitUntil(() => playerInRoom);

            // --- 1. IDLE ---
            yield return new WaitForSeconds(idleDuration);

            // --- 2. CRUMBLE ---
            anim.SetTrigger("Crumble");
            // Wait for the crumble clip to finish (adjust this to your clip's length)
            yield return new WaitForSeconds(0.8f); 

            // --- 3. THE TELEPORT (WHILE INVISIBLE) ---
            float targetX = (transform.position.x > (leftSpot + rightSpot) / 2) ? leftSpot : rightSpot;
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
            
            // --- 4. REASSEMBLE (FORCED PLAY) ---
            // Using Play() instead of SetTrigger() ensures it starts the clip NOW
            anim.Play("BossReassemble"); 
            
            // Wait for the reassemble clip to finish before moving to attack
            yield return new WaitForSeconds(0.8f); 

            // --- 5. ATTACK ---
            if (playerInRoom) yield return StartCoroutine(ChargeAndFire());
        }
    }

    IEnumerator ChargeAndFire()
    {
        // Create boulder and parent it so it follows the hand
        GameObject boulderGO = Instantiate(boulderPrefab, firePoint.position, Quaternion.identity);
        boulderGO.transform.SetParent(firePoint); 
        
        Boulder bScript = boulderGO.GetComponent<Boulder>();

        anim.SetTrigger("Charge");
        yield return new WaitForSeconds(chargeTime);

        if (bScript != null)
        {
            // STRAIGHT LINE LOGIC:
            // Check current scale to determine left (-1) or right (1)
            float dirX = transform.localScale.x > 0 ? 1f : -1f;
            Vector2 launchDir = new Vector2(dirX, 0); 

            bScript.Launch(launchDir);
        }

        yield return new WaitForSeconds(1f);
    }

    void FlipTowardsPlayer()
    {
        if (player == null) return;

        // Faces player based on X position
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }
}