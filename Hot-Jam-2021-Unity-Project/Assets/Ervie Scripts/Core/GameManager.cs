using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] LevelManager currentLevel;
    string currentSceneName;

    public void SetCurrentLevel(LevelManager level)
    {
        if(currentLevel != level)
        {
            currentLevel = level;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        StartCoroutine(RestartCoroutine());
    }

    private IEnumerator RestartCoroutine()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager.StartFade(.5f, 0);
        yield return new WaitForSecondsRealtime(.25f);
        LoadScene(currentSceneName);
    }

    public void LoadScene(string nextScene)
    {
        SceneManagement sceneManager = FindObjectOfType<SceneManagement>();
        currentSceneName = nextScene;
        sceneManager.LoadScene(nextScene);
    }
}
