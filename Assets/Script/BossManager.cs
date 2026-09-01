using UnityEngine;
using System.Collections;
using TMPro; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement; // REQUIRED for scene transitions

public class BossManager : MonoBehaviour
{
    [Header("Detection Settings")]
    public Transform player;
    public float triggerX = 37f;
    public float lockCameraX = 47f;

    [Header("Boss References")]
    public GameObject bossObject;
    public GameObject bossHealthBar;
    
    [Header("Arena Walls")]
    public GameObject entranceWall; 

    [Header("Boss Intro & Audio")]
    public AudioClip bossBGM;
    public string bossName = "The Guardian";
    [TextArea(3, 10)]
    public string[] bossDialogue;

    [Header("Internal Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Button nextButton; 

    [Header("Victory Settings")]
    public AudioClip victoryBGM; 
    [TextArea(3, 10)]
    public string[] defeatDialogue;
    public string victoryAnimationTrigger = "isVictory";
    public float victoryDelay = 6f; // How long to wait before credits
    public string creditsSceneName = "Credits"; // Ensure this matches your scene name exactly

    private CameraFollow camScript;
    private bool bossTriggered = false;
    private bool isWaitingForInput = false;
    private int dialogueIndex = 0;
    private AudioSource localSource;

    void Start()
    {
        camScript = Camera.main.GetComponent<CameraFollow>();
        localSource = GetComponent<AudioSource>();

        if (bossObject != null) bossObject.SetActive(false);
        if (bossHealthBar != null) bossHealthBar.SetActive(false);
        if (entranceWall != null) entranceWall.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (nextButton != null)
            nextButton.onClick.AddListener(AdvanceDialogue);
    }

    void Update()
    {
        if (player == null) return;

        if (!bossTriggered && player.position.x >= triggerX)
        {
            StartCoroutine(BossIntroSequence());
        }

        if (bossTriggered && isWaitingForInput)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AdvanceDialogue();
            }
        }
    }

    IEnumerator BossIntroSequence()
    {
        bossTriggered = true;

        if (camScript != null) camScript.TriggerBossLock(lockCameraX);
        if (entranceWall != null) entranceWall.SetActive(true);

        GameObject am = GameObject.Find("AudioManager");
        if (am != null && am.GetComponent<AudioSource>() != null)
            am.GetComponent<AudioSource>().Stop();

        FreezePlayer(true);

        yield return StartCoroutine(RunDialogue(bossDialogue));

        if (bossBGM != null)
        {
            AudioSource sourceToUse = (localSource != null) ? localSource : (am != null ? am.GetComponent<AudioSource>() : null);
            if (sourceToUse != null)
            {
                sourceToUse.clip = bossBGM;
                sourceToUse.loop = true;
                sourceToUse.Play();
            }
        }

        if (bossObject != null) bossObject.SetActive(true);
        if (bossHealthBar != null) bossHealthBar.SetActive(true);
        
        FreezePlayer(false);
    }

    public void StartDefeatSequence()
    {
        StartCoroutine(DefeatSequence());
    }

    IEnumerator DefeatSequence()
    {
        FreezePlayer(true);
        
        if (localSource != null) localSource.Stop();

        yield return StartCoroutine(RunDialogue(defeatDialogue));

        // TRIGGER VICTORY MUSIC
        if (victoryBGM != null && localSource != null)
        {
            localSource.clip = victoryBGM;
            localSource.loop = false; 
            localSource.Play();
        }

        // TRIGGER VICTORY ANIMATION
        Animator playerAnim = player.GetComponent<Animator>();
        if (playerAnim != null)
        {
            playerAnim.SetTrigger(victoryAnimationTrigger);
        }

        Debug.Log("Victory! Waiting for scene transition...");

        // WAIT BEFORE LOADING CREDITS
        yield return new WaitForSeconds(victoryDelay);

        // LOAD THE CREDITS SCENE
        SceneManager.LoadScene(creditsSceneName);
    }

    IEnumerator RunDialogue(string[] lines)
    {
        if (dialoguePanel != null && lines.Length > 0)
        {
            dialoguePanel.SetActive(true);
            nameText.text = bossName;
            
            for (int i = 0; i < lines.Length; i++)
            {
                dialogueText.text = lines[i];
                isWaitingForInput = true;
                while (isWaitingForInput) yield return null;
            }
            dialoguePanel.SetActive(false);
        }
    }

    public void AdvanceDialogue()
    {
        isWaitingForInput = false;
    }

    void FreezePlayer(bool freeze)
    {
        PlayerInput input = player.GetComponent<PlayerInput>();
        if (input != null) input.canMove = !freeze;
        
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null && freeze) rb.linearVelocity = Vector2.zero;
    }
}