using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreenMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup blackPanelCanvas;
    [SerializeField] CanvasGroup creditsPanelCanvas;
    [SerializeField] Image bgImage;

    bool isFading = false;
    float _timer = 0;

    private void Start()
    {
        blackPanelCanvas.alpha = 0;
        blackPanelCanvas.interactable = false;
        blackPanelCanvas.blocksRaycasts = false;

        creditsPanelCanvas.alpha = 0;
        creditsPanelCanvas.interactable = false;
        creditsPanelCanvas.blocksRaycasts = false;
    }


    private IEnumerator FadeInVictoryMenu()
    {
        isFading = true;

        _timer = 0;

        // FADE OUT ENDING
        while (bgImage.color.r > 0)
        {
            bgImage.color = Color.Lerp(Color.white, Color.black, _timer);
            yield return null; // run this on the next opportunity of the next frame
        }

        yield return new WaitForSeconds(1f);
        
        // FADE IN CREDITS
        while (creditsPanelCanvas.alpha < 1) //alpha is not 1
        {
            creditsPanelCanvas.alpha += Time.deltaTime / 1f;
            yield return null; // run this on the next opportunity of the next frame
        }
        creditsPanelCanvas.alpha = 1;
        creditsPanelCanvas.interactable = true;
        creditsPanelCanvas.blocksRaycasts = true;

        yield return new WaitForSeconds(7.5f);

        // FADE OUT CREDITS
        while (creditsPanelCanvas.alpha > 0) //alpha is not 0
        {
            creditsPanelCanvas.alpha -= Time.deltaTime / 1f;
            yield return null; // run this on the next opportunity of the next frame
        }
        creditsPanelCanvas.alpha = 0;
        creditsPanelCanvas.interactable = false;
        creditsPanelCanvas.blocksRaycasts = false;

        yield return new WaitForSeconds(1f);

        // FADE IN VICTORY
        while (blackPanelCanvas.alpha < 1) //alpha is not 1
        {
            blackPanelCanvas.alpha += Time.deltaTime / 1f;
            yield return null; // run this on the next opportunity of the next frame
        }
        blackPanelCanvas.alpha = 1;
        blackPanelCanvas.interactable = true;
        blackPanelCanvas.blocksRaycasts = true;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0) || _timer > 7.5f)
        {
            if (!isFading)
            {
                StartCoroutine(FadeInVictoryMenu());
            }
        }
    }
}
