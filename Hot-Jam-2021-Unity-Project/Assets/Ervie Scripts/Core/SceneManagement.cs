using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void LoadScene(string sceneName, float fadeWaitTime = 1f, float fadeOutTime = 1f, float fadeInTime = 1f)
    {
        Debug.Log("scene name: " + sceneName);
        StartCoroutine(LoadSceneCoroutine(sceneName, fadeWaitTime, fadeOutTime, fadeInTime));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, float fadeWaitTime = 1f, float fadeOutTime = 1f, float fadeInTime = 1f)
    {
        Fader fader = FindObjectOfType<Fader>();
        yield return fader.FadeOut(fadeOutTime);
        if (string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(fadeWaitTime);
        yield return fader.FadeIn(fadeInTime);
    }
}
