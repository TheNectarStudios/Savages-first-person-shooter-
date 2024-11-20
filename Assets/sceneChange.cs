using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    public float transitionDelay = 0f; // Optional delay before switching scenes.

    /// <summary>
    /// Loads the next scene based on the current scene index.
    /// If the index exceeds the available scenes, it loops back to the first scene.
    /// </summary>
    public void LoadNextScene()
    {
        // Get the current active scene index.
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Calculate the next scene index.
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;

        // Load the next scene with optional delay.
        if (transitionDelay > 0)
        {
            StartCoroutine(DelayedSceneLoad(nextSceneIndex));
        }
        else
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    /// <summary>
    /// Coroutine to delay the scene loading.
    /// </summary>
    private System.Collections.IEnumerator DelayedSceneLoad(int sceneIndex)
    {
        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene(sceneIndex);
    }
}
