using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    
    [Header("Offsets")]
    public float yOffset = 2.0f; 
    public float xOffset = 0f;

    [Header("Smoothing")]
    public float smoothTime = 0.15f; 
    private Vector2 currentVelocity;

    [Header("Level Bounds")]
    public float minX = -50f; 
    public float maxX = 50f;  
    public float minY = 0f;   

    [Header("Boss Settings")]
    // By default, this is FALSE so it won't affect the Tutorial
    public bool isBossLocked = false; 
    private float bossLockX;

    void Start()
    {
        // Safety: Always start unlocked when a new scene loads
        isBossLocked = false;
        
        // Auto-find player if you forgot to drag it in
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void LateUpdate() 
    {
        if (player == null) return;

        float targetX;
        float targetY = player.position.y + yOffset;

        // logic: Only use the lock if the trigger was hit in this specific scene
        if (isBossLocked)
        {
            targetX = bossLockX;
        }
        else
        {
            targetX = player.position.x + xOffset;
        }

        // Clamp the values
        float clampedX = Mathf.Clamp(targetX, minX, maxX);
        float clampedY = Mathf.Max(targetY, minY); 

        float newX = Mathf.SmoothDamp(transform.position.x, clampedX, ref currentVelocity.x, smoothTime);
        float newY = Mathf.SmoothDamp(transform.position.y, clampedY, ref currentVelocity.y, smoothTime);

        transform.position = new Vector3(newX, newY, -10f);
    }

    public void TriggerBossLock(float xPos)
    {
        bossLockX = xPos;
        isBossLocked = true;
    }
}