using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    private bool movingRight = true;

    [Header("Detection")]
    public Transform edgeCheck; 
    public float checkDistance = 0.5f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private HidingEnemy hidingScript; // Reference to the hiding logic

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hidingScript = GetComponent<HidingEnemy>(); // Link to the hiding script
    }

    void Update()
    {
        // 1. Check if we should be frozen in hiding
        if (hidingScript != null && hidingScript.isHidden)
        {
            return; // STOP HERE: Don't move, don't check for edges
        }

        // 2. Move the enemy
        // We use Vector2.right. The rotation from Flip() handles the actual direction.
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // 3. Check for the edge or a wall
        RaycastHit2D groundInfo = Physics2D.Raycast(edgeCheck.position, Vector2.down, checkDistance, groundLayer);

        // 4. If there is NO ground below the edgeCheck, flip around
        if (groundInfo.collider == false)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;

        if (movingRight)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
        }
    }

    private void OnDrawGizmos()
    {
        if (edgeCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(edgeCheck.position, edgeCheck.position + Vector3.down * checkDistance);
        }
    }
}