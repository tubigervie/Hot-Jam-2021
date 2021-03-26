using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    LevelManager currentLevel;
    [SerializeField] string loseScene = "Lose";
    string currentSceneName;
    private static bool tutorialComplete = false;

    public void SetCurrentLevel(LevelManager level)
    {
        if(currentLevel != level)
        {
            currentLevel = level;
            currentSceneName = SceneManager.GetActiveScene().name;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public bool TutorialComplete()
    {
        return tutorialComplete;
    }

    public void LoadLose()
    {
        FindObjectOfType<Fader>().GetComponentInChildren<Image>().color = Color.red;
        LoadScene(loseScene, 2, 3, 1);
    }

    public void RestartGame()
    {
        StartCoroutine(RestartCoroutine());
    }

    public void LoadFirstLevel()
    {
        tutorialComplete = true;
        RestartGame();
    }

    private IEnumerator RestartCoroutine()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager.StartFade(.5f, 0);
        yield return new WaitForSecondsRealtime(.25f);
        LoadScene(currentSceneName, .5f, .5f, .5f);
    }

    public void LoadScene(string nextScene, float fadeWaitTime = 1f, float fadeOutTime = 1f, float fadeInTime = 1f)
    {
        SceneManagement sceneManager = FindObjectOfType<SceneManagement>();
        currentSceneName = nextScene;
        sceneManager.LoadScene(nextScene, fadeWaitTime, fadeOutTime, fadeInTime);
    }
}
