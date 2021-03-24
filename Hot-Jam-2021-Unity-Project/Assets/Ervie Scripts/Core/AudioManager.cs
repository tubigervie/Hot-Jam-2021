using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip levelStartTheme;
    [SerializeField] AudioClip levelTheme;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator StartFadeCoroutine(float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public void StartFade(float duration, float targetVolume)
    {
        StartCoroutine(StartFadeCoroutine(duration, targetVolume));
    }

    public void StartLevelTheme()
    {
        StartCoroutine(LevelThemeCoroutine());
    }

    private IEnumerator LevelThemeCoroutine()
    {
        audioSource.loop = false;
        audioSource.clip = levelStartTheme;
        audioSource.volume = 0;
        audioSource.Play();
        yield return StartFadeCoroutine(3f, .8f);
        yield return new WaitForSeconds(audioSource.clip.length - 3f);
        audioSource.clip = levelTheme;
        audioSource.loop = true;
        audioSource.Play();
    }
}
