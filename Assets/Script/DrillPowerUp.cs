using UnityEngine;

public class DrillPowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[Pickup] Touched Player tag.");
            
            // Try to find the script on the object touched OR its parent
            PlayerAbilities abilities = other.GetComponentInParent<PlayerAbilities>();
            
            if (abilities != null)
            {
                abilities.UnlockDrill();
                Debug.Log("[Pickup] Successfully called UnlockDrill via Parent check.");
            }
            else
            {
                Debug.LogError("[Pickup] ERROR: Player (and its parents) does not have PlayerAbilities script!");
            }

            Destroy(gameObject);
        }
    }
}