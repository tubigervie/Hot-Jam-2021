using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    public void PlayGame()
    {
        SceneManagement sceneManager = FindObjectOfType<SceneManagement>();
        sceneManager.LoadScene(sceneToLoad);
    }
}
