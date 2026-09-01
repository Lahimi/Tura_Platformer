    using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Player Sounds")]
    public AudioClip playerShoot;
    public AudioClip playerHit;
    public AudioClip playerLand;
    public AudioClip playerDeath;

    [Header("Enemy Sounds")]
    public AudioClip enemyShoot;
    public AudioClip enemyHit;

    [Header("Themes")]
    public AudioClip victoryTheme;

    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayVictoryTheme()
    {
        // 1. Stop the current BGM immediately
        audioSource.Stop(); 
        
        // 2. Play the victory theme
        // We use PlayOneShot so it doesn't loop, or you can assign it to clip and .Play()
        if (victoryTheme != null)
        {
            audioSource.PlayOneShot(victoryTheme);
        }
    }
}