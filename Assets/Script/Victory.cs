using UnityEngine;
using System.Collections;

public class VictoryTrigger : MonoBehaviour
{
    [Header("Next Destination")]
    public string nextSceneName; 
    public float delayBeforeTransition = 2f; 

    private bool levelCompleted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"=== VICTORY TRIGGER HIT ===");
        
        if (levelCompleted) return;

        // Check for PlayerInput component AND Player tag
        PlayerInput input = other.GetComponent<PlayerInput>();
        
        if (input != null && other.CompareTag("Player"))
        {
            levelCompleted = true;
            Debug.Log("<color=green>VictoryTrigger: Player detected!</color>");

            // --- AUDIO REVISION: CUT BGM ---
            // We find the GameObject named "AudioManager" and stop its AudioSource
            GameObject am = GameObject.Find("AudioManager");
            if (am != null)
            {
                AudioSource bgmSource = am.GetComponent<AudioSource>();
                if (bgmSource != null)
                {
                    bgmSource.Stop();
                    Debug.Log("BGM stopped successfully.");
                }
            }

            // --- AUDIO REVISION: PLAY VICTORY THEME ---
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlayVictoryTheme();
            }

            // Freeze Player Physics and Input
            input.canMove = false;
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null) 
            {
                rb.bodyType = RigidbodyType2D.Static;
                rb.linearVelocity = Vector2.zero;
            }

            // Trigger Victory Animation
            Animator anim = other.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetBool("isFinished", true);
                anim.SetTrigger("Victory");
            }

            StartCoroutine(VictorySequence());
        }
    }

    IEnumerator VictorySequence()
    {
        yield return new WaitForSeconds(delayBeforeTransition);

        LevelLoader loader = FindFirstObjectByType<LevelLoader>();
        if (loader != null)
        {
            loader.LoadSpecificLevel(nextSceneName);
        }
        else
        {
            Debug.LogError("VictoryTrigger: LevelLoader not found in scene!");
        }
    }
}