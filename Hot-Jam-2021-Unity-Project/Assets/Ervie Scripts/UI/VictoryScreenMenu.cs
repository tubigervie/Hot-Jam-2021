using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScreenMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup blackPanelCanvas;

    bool isFading = false;
    float _timer = 0;

    private void Start()
    {
        blackPanelCanvas.alpha = 0;
        blackPanelCanvas.interactable = false;
        blackPanelCanvas.blocksRaycasts = false;
    }


    private IEnumerator FadeInVictoryMenu()
    {
        isFading = true;
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
