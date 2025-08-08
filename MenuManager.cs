using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScenePanelController : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup scenePanelGroup; // Use CanvasGroup instead of just GameObject
    public float fadeDuration = 0.3f;

    [Header("Scene Names")]
    public string scene1Name = "Scene1";
    public string scene2Name = "Scene2";

    private bool isFading = false;

    void Start()
    {
        // Make sure the panel starts hidden
        scenePanelGroup.alpha = 0;
        scenePanelGroup.interactable = false;
        scenePanelGroup.blocksRaycasts = false;
    }

    public void OpenPanel()
    {
        if (!isFading)
            StartCoroutine(FadeCanvasGroup(scenePanelGroup, 0, 1, fadeDuration, true));
    }

    public void ClosePanel()
    {
        if (!isFading)
            StartCoroutine(FadeCanvasGroup(scenePanelGroup, 1, 0, fadeDuration, false));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration, bool setInteractable)
    {
        isFading = true;
        float elapsed = 0f;

        cg.interactable = setInteractable;
        cg.blocksRaycasts = setInteractable;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        cg.alpha = end;

        // Disable interactions if hidden
        if (end == 0)
        {
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        isFading = false;
    }

    public void LoadScene1()
    {
        SceneManager.LoadScene(scene1Name);
    }

    public void LoadScene2()
    {
        SceneManager.LoadScene(scene2Name);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop in Editor
#else
        Application.Quit(); // Quit in Build
#endif
    }
}
