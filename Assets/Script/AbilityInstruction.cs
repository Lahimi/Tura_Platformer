using UnityEngine;

public class InstructionTrigger : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject instructionObject; // Drag Instructions6_0 here

    private bool hasShown = false;

    void Start()
    {
        // Safety: Make sure the instruction is hidden when the scene starts
        if (instructionObject != null && !PlayerAbilities.hasDrill)
        {
            instructionObject.SetActive(false);
        }
    }

    void Update()
    {
        // If the player has the drill and we haven't turned the UI on yet
        if (PlayerAbilities.hasDrill && !hasShown)
        {
            if (instructionObject != null)
            {
                instructionObject.SetActive(true);
                hasShown = true; 
                Debug.Log("Drill acquired! Showing Instruction.");
            }
        }

        // Optional: Hide it again once they actually use the drill (C key)
        if (hasShown && Input.GetKeyDown(KeyCode.C))
        {
            if (instructionObject != null)
            {
                instructionObject.SetActive(false);
            }
        }
    }
}