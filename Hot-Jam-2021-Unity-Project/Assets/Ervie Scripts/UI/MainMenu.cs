using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    public void PlayGame()
    {
        SceneManagement sceneManager = FindObjectOfType<SceneManagement>();
        FindObjectOfType<Fader>().GetComponentInChildren<Image>().color = Color.white;
        sceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
