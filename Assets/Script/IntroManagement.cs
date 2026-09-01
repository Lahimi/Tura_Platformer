using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement; // Required to change levels

public class IntroManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI messageText;
    public GameObject dialoguePanel;

    [Header("Briefing Content")]
    public string speakerName = "Dr. Light";
    [TextArea(3, 10)]
    public string[] briefingLines;
    public string nextSceneName = "TutorialLevel";

    private int index = 0;
    private bool isWaiting = false;

    void Start()
    {
        if (briefingLines.Length > 0)
        {
            StartCoroutine(PlayBriefing());
        }
    }

    IEnumerator PlayBriefing()
    {
        dialoguePanel.SetActive(true);
        nameText.text = speakerName;

        for (index = 0; index < briefingLines.Length; index++)
        {
            messageText.text = briefingLines[index];
            isWaiting = true;

            // Wait for Spacebar or Mouse Click
            while (isWaiting)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    isWaiting = false;
                }
                yield return null;
            }
        }

        // Briefing finished, move to the game
        SceneManager.LoadScene(nextSceneName);
    }
}