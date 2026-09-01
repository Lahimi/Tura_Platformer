using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public void LoadSpecificLevel(string sceneName)
    {
        Debug.Log("<color=cyan>LevelLoader: Received request to load: </color>" + sceneName);
        StartCoroutine(LoadLevelRoutine(sceneName));
    }

    IEnumerator LoadLevelRoutine(string sceneName)
    {
        if (transition != null)
        {
            Debug.Log("<color=cyan>LevelLoader: Triggering 'StartFade' Animator parameter.</color>");
            transition.SetTrigger("StartFade");
        }
        else
        {
            Debug.LogError("LevelLoader: ANIMATOR MISSING! Drag the Overlay into the Transition slot.");
        }

        yield return new WaitForSeconds(transitionTime);

        Debug.Log("<color=yellow>LevelLoader: yield return finished. Attempting SceneManager.LoadScene...</color>");
        
        // Final check: Does the scene exist in Build Settings?
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"LevelLoader: CANNOT LOAD '{sceneName}'. Is it spelled correctly and added to Build Settings?");
        }
    }
}