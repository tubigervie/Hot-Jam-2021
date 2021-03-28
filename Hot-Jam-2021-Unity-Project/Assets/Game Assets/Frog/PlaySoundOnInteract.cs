using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnInteract : MonoBehaviour, IInteractable
{
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact(GameObject player)
    {
        audioSource.Play();
    }
}
