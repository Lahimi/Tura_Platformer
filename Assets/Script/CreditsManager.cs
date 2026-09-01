using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject creditsContent; // The object containing your list of names
    public GameObject thankYouText;   // The "Thank You" message
    public float scrollSpeed = 50f;   // How fast the text moves up
    public float finishDelay = 3f;    // How long to wait on the "Thank You" screen
    
    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu";

    private bool creditsFinished = false;

    void Start()
    {
        if (thankYouText != null) thankYouText.SetActive(false);
        StartCoroutine(RunCredits());
    }

    IEnumerator RunCredits()
    {
        // 1. Optional: Wait a second before starting
        yield return new WaitForSeconds(1f);

        // 2. Scroll Logic (If you want it to move)
        // This moves the creditsContent upward until it's off-screen
        // Adjust the '1000' based on how long your text is
        while (creditsContent.transform.localPosition.y < 1000) 
        {
            creditsContent.transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
            
            // Allow skip
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) break;
            
            yield return null;
        }

        // 3. Show "Thank You"
        if (creditsContent != null) creditsContent.SetActive(false);
        if (thankYouText != null) thankYouText.SetActive(true);

        yield return new WaitForSeconds(finishDelay);

        // 4. Return to Menu
        SceneManager.LoadScene(mainMenuSceneName);
    }
}