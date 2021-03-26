using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientMenu : MonoBehaviour
{
    [SerializeField] GameObject recipeControlsText;
    CanvasGroup ingredientMenuCanvas;
    [SerializeField] PlayerController playerController;
    LevelManager currentLevel;
    GameManager gameManager;
    bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = FindObjectOfType<LevelManager>();
        gameManager = FindObjectOfType<GameManager>();
        currentLevel.onLevelStart += ToggleRecipeControlsText;
        ingredientMenuCanvas = GetComponent<CanvasGroup>();
        ToggleMenuImmediate(isActive);
        recipeControlsText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && currentLevel != null && playerController.CanAct() && gameManager.TutorialComplete() && Time.timeScale > 0)
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

    private void ToggleRecipeControlsText()
    {
        recipeControlsText.SetActive(true);
    }

    private void ToggleMenuImmediate(bool pauseFlag)
    {
        ingredientMenuCanvas.alpha = (pauseFlag) ? 1 : 0;
        ingredientMenuCanvas.interactable = pauseFlag;
        ingredientMenuCanvas.blocksRaycasts = pauseFlag;
    }

    private IEnumerator FadeInMenu(float time)
    {
        recipeControlsText.SetActive(isActive);
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
        recipeControlsText.SetActive(isActive);
        while (ingredientMenuCanvas.alpha > 0) //alpha is not 1
        {
            ingredientMenuCanvas.alpha -= Time.unscaledDeltaTime / time;
            yield return null; // run this on the next opportunity of the next frame
        }
        isActive = false;
        ToggleMenuImmediate(isActive);
    }
}
