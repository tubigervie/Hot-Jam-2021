using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    public void StopFades()
    {
        StopAllCoroutines();
    }

    public IEnumerator FadeOut(float time)
    {
        while (canvasGroup.alpha < 1) //alpha is not 1
        {
            canvasGroup.alpha += Time.deltaTime / time;
            yield return null; // run this on the next opportunity of the next frame
        }
        canvasGroup.alpha = 1;
    }

    public void FadeOutImmediate()
    {
        canvasGroup.alpha = 1;
    }

    public IEnumerator FadeIn(float time)
    {
        while (canvasGroup.alpha > 0) //alpha is not 1
        {
            canvasGroup.alpha -= Time.deltaTime / time;
            yield return null; // run this on the next opportunity of the next frame
        }
        canvasGroup.alpha = 0;
    }
}
