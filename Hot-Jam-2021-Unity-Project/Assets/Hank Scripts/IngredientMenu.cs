using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientMenu : MonoBehaviour
{
    CanvasGroup ingredientMenuCanvas;

    LevelManager currentLevel;
    bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = FindObjectOfType<LevelManager>();
        ingredientMenuCanvas = GetComponent<CanvasGroup>();
        ToggleMenuImmediate(isActive);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && currentLevel != null)
        {
            if (isActive)
            {
                StartCoroutine(FadeOutMenu(.25f));
            }
            else
            {
                StartCoroutine(FadeInMenu(.25f));
            }
        }
    }

    private void ToggleMenuImmediate(bool pauseFlag)
    {
        ingredientMenuCanvas.alpha = (pauseFlag) ? 1 : 0;
        ingredientMenuCanvas.interactable = pauseFlag;
        ingredientMenuCanvas.blocksRaycasts = pauseFlag;
    }

    private IEnumerator FadeInMenu(float time)
    {
        while (ingredientMenuCanvas.alpha < 1) //alpha is not 1
        {
            ingredientMenuCanvas.alpha += Time.unscaledDeltaTime / time;
            yield return null; // run this on the next opportunity of the next frame
        }
        isActive = true;
        ToggleMenuImmediate(isActive);
    }

    private IEnumerator FadeOutMenu(float time)
    {
        while (ingredientMenuCanvas.alpha > 0) //alpha is not 1
        {
            ingredientMenuCanvas.alpha -= Time.unscaledDeltaTime / time;
            yield return null; // run this on the next opportunity of the next frame
        }
        isActive = false;
        ToggleMenuImmediate(isActive);
    }
}
