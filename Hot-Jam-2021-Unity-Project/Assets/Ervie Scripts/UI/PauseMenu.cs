using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    CanvasGroup pauseMenuCanvas;
    [SerializeField] GameObject quitMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject restartMenu;
    bool isPaused = false;


    LevelManager currentLevel;
    //public Action onPause;
    //public Action onResume;

    private void Awake()
    {
        pauseMenuCanvas = GetComponent<CanvasGroup>();
        isPaused = false;
        ToggleMenuImmediate(isPaused);
    }

    private void Start()
    {
        currentLevel = FindObjectOfType<LevelManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && currentLevel != null)
        {
            TogglePause(!isPaused);
        }
    }

    public void Resume()
    {
        TogglePause(true);
    }

    public void TogglePause(bool flag)
    {
        quitMenu.SetActive(false);
        restartMenu.SetActive(false);
        pauseMenu.SetActive(true);
        isPaused = flag;
        if (!isPaused)
        {
            StartCoroutine(PauseCoroutine());
        }
        else
        {
            StartCoroutine(ResumeCoroutine());
        }
    }

    public void RestartLevel()
    {
        StartCoroutine(RestartCoroutine());
    }

    private IEnumerator RestartCoroutine()
    {
        ToggleMenuImmediate(false);
        yield return ResumeCoroutine();
        FindObjectOfType<GameManager>().RestartGame();
    }

    private IEnumerator PauseCoroutine()
    {
        Time.timeScale = 0;
        FindObjectOfType<AudioManager>().PauseInGameAudio();
        yield return FadeInMenu(.25f);
        pauseMenuCanvas.interactable = true;
        pauseMenuCanvas.blocksRaycasts = true;
    }

    private IEnumerator ResumeCoroutine()
    {
        yield return FadeOutMenu(.25f);
        Time.timeScale = 1;
        FindObjectOfType<AudioManager>().ResumeInGameAudio();
        pauseMenuCanvas.interactable = false;
        pauseMenuCanvas.blocksRaycasts = false;
    }

    private IEnumerator FadeInMenu(float time)
    {
        while (pauseMenuCanvas.alpha < 1) //alpha is not 1
        {
            pauseMenuCanvas.alpha += Time.unscaledDeltaTime / time;
            yield return null; // run this on the next opportunity of the next frame
        }
        pauseMenuCanvas.alpha = 1;
    }

    private void ToggleMenuImmediate(bool pauseFlag)
    {
        quitMenu.SetActive(false);
        restartMenu.SetActive(false);
        pauseMenu.SetActive(true);
        pauseMenuCanvas.alpha = (pauseFlag) ? 1 : 0;
        pauseMenuCanvas.interactable = pauseFlag;
        pauseMenuCanvas.blocksRaycasts = pauseFlag;
    }

    private IEnumerator FadeOutMenu(float time)
    {
        while (pauseMenuCanvas.alpha > 0) //alpha is not 1
        {
            pauseMenuCanvas.alpha -= Time.unscaledDeltaTime / time;
            yield return null; // run this on the next opportunity of the next frame
        }
        pauseMenuCanvas.alpha = 0;
    }

    public void QuitButton()
    {
        FindObjectOfType<GameManager>().QuitGame();
    }
}
