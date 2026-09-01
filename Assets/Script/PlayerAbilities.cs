using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public static bool hasDrill = false;

    [Header("Visuals")]
    public GameObject drillVisual; 

    void Start()
    {
        Debug.Log($"[Abilities] Scene Started. Persistent hasDrill: {hasDrill}");
        RefreshDrillVisual();
    }

    public void UnlockDrill()
    {
        hasDrill = true;
        Debug.Log("[Abilities] static hasDrill set to TRUE");
        RefreshDrillVisual();
    }

    public void RefreshDrillVisual()
    {
        if (drillVisual != null)
        {
            drillVisual.SetActive(hasDrill);
            Debug.Log($"[Abilities] Drill Visual set to: {hasDrill}");
        }
        else
        {
            Debug.LogError("[Abilities] ERROR: Drill Visual slot is EMPTY in the Inspector!");
        }

        PlayerInput input = GetComponent<PlayerInput>();
        if (input != null)
        {
            input.hasDrillAbility = hasDrill;
            Debug.Log($"[Abilities] PlayerInput.hasDrillAbility synced to: {input.hasDrillAbility}");
        }
        else
        {
            Debug.LogError("[Abilities] ERROR: Could not find PlayerInput component on this object!");
        }
    }
}