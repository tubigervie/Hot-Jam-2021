using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip levelStartTheme;
    [SerializeField] AudioClip levelThemeLoop;

    [SerializeField] AudioClip rushStartTheme;
    [SerializeField] AudioClip rushThemeLoop;

    [SerializeField] float ostVolume = .6f;

    Coroutine startLevelCoroutine;
    Coroutine rushCoroutine;

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
            currentTime += Time.unscaledDeltaTime;
            if (currentTime > duration)
                currentTime = duration;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public void PauseInGameAudio()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in audioSources)
        {
            if (audio != this.audioSource)
                audio.Pause();
        }
    }

    public void ResumeInGameAudio()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in audioSources)
        {
            if (audio != this.audioSource)
                audio.UnPause();
        }
    }

    public void StartFade(float duration, float targetVolume)
    {
        StartCoroutine(StartFadeCoroutine(duration, targetVolume));
    }

    public void StartLevelTheme()
    {
        if (startLevelCoroutine != null) StopCoroutine(startLevelCoroutine);
        startLevelCoroutine = StartCoroutine(LevelThemeCoroutine());
    }

    public void StartLevelRushTheme()
    {
        if (rushCoroutine != null) StopCoroutine(rushCoroutine);
        rushCoroutine = StartCoroutine(RushThemeCoroutine());
    }

    public void PlayOneShot(AudioClip clip, float volume)
    {
        audioSource.PlayOneShot(clip, volume);
    }

    private IEnumerator RushThemeCoroutine()
    {
        yield return StartFadeCoroutine(2f, 0f);
        audioSource.loop = false;
        audioSource.clip = rushStartTheme;
        audioSource.volume = 0;
        audioSource.Play();
        yield return StartFadeCoroutine(1f, ostVolume);
        yield return new WaitForSecondsRealtime(audioSource.clip.length - 1f);
        audioSource.clip = rushThemeLoop;
        audioSource.loop = true;
        audioSource.Play();
    }

    private IEnumerator LevelThemeCoroutine()
    {
        yield return StartFadeCoroutine(2f, 0f);
        audioSource.loop = false;
        audioSource.clip = levelStartTheme;
        audioSource.volume = 0;
        audioSource.Play();
        yield return StartFadeCoroutine(2f, ostVolume);
        yield return new WaitForSecondsRealtime(audioSource.clip.length - 3f);
        audioSource.clip = levelThemeLoop;
        audioSource.loop = true;
        audioSource.Play();
    }
}
