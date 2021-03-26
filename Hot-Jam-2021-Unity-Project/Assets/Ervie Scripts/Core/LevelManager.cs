using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] string nextLevelName;
    [SerializeField] CauldronContainer cauldron;
    [SerializeField] GameObject player;
    [SerializeField] Dialogue tutorialDialogue;
    [SerializeField] Dialogue startDialogue;
    [SerializeField] Dialogue afterTutorialDialogue;
    [SerializeField] Dialogue completeDialogue;
    [SerializeField] float totalBoilTimer = 120f;

    public Action onLevelStart;

    private void Awake()
    {
        if(instance != null)
            Destroy(this.gameObject);
        else
            instance = this;

        cauldron = FindObjectOfType<CauldronContainer>();
        player = GameObject.FindGameObjectWithTag("Player");
        cauldron.onRecipeComplete += OnRecipeComplete;
        onLevelStart += OnLevelStart;
    }

    

    private void Start()
    {
        FindObjectOfType<GameManager>().SetCurrentLevel(this);
        if (FindObjectOfType<GameManager>().TutorialComplete())
        {
            player.GetComponent<PlayerController>().ApplyEffect(PlayerController.PlayerState.INDIALOGUE);
            player.GetComponent<PlayerConversant>().QueueDialogue(startDialogue, 2f);
        }
        else if (player != null && tutorialDialogue != null)
        {
            player.GetComponent<PlayerController>().ApplyEffect(PlayerController.PlayerState.INDIALOGUE);
            player.GetComponent<PlayerConversant>().QueueDialogue(tutorialDialogue, 2f);
        }
        else
            onLevelStart.Invoke();
        FindObjectOfType<AudioManager>().StartLevelTheme();
    }

    public void OnRecipeComplete()
    {
        player.GetComponent<PlayerConversant>().QueueDialogue(completeDialogue, 1f);
    }

    public void OnLevelCompleteDialogueEnd()
    {
        FindObjectOfType<AudioManager>().StartFade(1.5f, 0);
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.LoadScene(nextLevelName);
    }

    public void OnLevelStart()
    {
        cauldron.GetComponent<CauldronAI>().OnStartLevel(totalBoilTimer);
    }

    public void OnTutorialComplete()
    {
        player.GetComponent<PlayerController>().ApplyEffect(PlayerController.PlayerState.INDIALOGUE);
        player.GetComponent<PlayerConversant>().QueueDialogue(afterTutorialDialogue, 1f);
    }
}
