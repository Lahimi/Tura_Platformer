using UnityEngine;

public class StageCamera : MonoBehaviour
{
    public Transform player;
    
    [Header("Thresholds")]
    public float followThresholdX = 0.6f; 
    public float followThresholdY = 2.0f; 
    public float yOffset = 0f;

    [Header("Smoothing")]
    public float smoothTime = 0.1f; // Try 0.1 for a tighter follow
    private Vector2 currentVelocity;

    // Use LateUpdate to prevent jittering with physics
    void LateUpdate() 
    {
        if (player != null)
        {
            // Calculate Target X
            float targetX = (player.position.x < followThresholdX) ? 0f : player.position.x;

            // Calculate Target Y
            float targetY = (Mathf.Abs(player.position.y) < followThresholdY) ? yOffset : player.position.y + yOffset;

            // Smooth independently
            float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref currentVelocity.x, smoothTime);
            float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref currentVelocity.y, smoothTime);

            transform.position = new Vector3(newX, newY, -10f);
        }
    }
}