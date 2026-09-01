using UnityEngine;

public class GuideCollectible : MonoBehaviour
{
    [Header("Optional: Instruction to Show")]
    public GameObject instructionToShow;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the thing touching us is the Player
        if (other.CompareTag("Player"))
        {
            // If you want this guide to turn on a specific instruction:
            if (instructionToShow != null)
            {
                instructionToShow.SetActive(true);
            }

            Debug.Log("Guide collected and destroyed!");
            
            // Destroy this collectible
            Destroy(gameObject);
        }
    }
}