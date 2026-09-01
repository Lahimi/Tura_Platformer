using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object touching us is the Player
        if (other.CompareTag("Player"))
        {
            // Find the PlayerInput component on the player
            PlayerInput input = other.GetComponent<PlayerInput>();

            if (input != null)
            {
                // Unlock the ability!
                input.hasShootingAbility = true;

                // Optional: Play a sound or a small "pickup" particle effect here
                Debug.Log("Shooting Unlocked!");

                // Destroy the sprite so it's a one-time interaction
                Destroy(gameObject);
            }
        }
    }
}