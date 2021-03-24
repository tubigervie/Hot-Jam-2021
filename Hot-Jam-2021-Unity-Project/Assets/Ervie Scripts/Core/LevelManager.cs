using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] string nextLevelName;
    [SerializeField] CauldronContainer cauldron;
    [SerializeField] GameObject player;
    [SerializeField] Dialogue startDialogue;
    [SerializeField] Dialogue completeDialogue;
    [SerializeField] float totalBoilTimer = 120f;

    private void Awake()
    {
        if(instance != null)
            Destroy(this.gameObject);
        else
            instance = this;

        cauldron = FindObjectOfType<CauldronContainer>();
        player = GameObject.FindGameObjectWithTag("Player");
        cauldron.onRecipeComplete += OnRecipeComplete;
    }

    

    private void Start()
    {
        FindObjectOfType<GameManager>().SetCurrentLevel(this);
        if(player != null && startDialogue != null)
        {
            player.GetComponent<PlayerController>().ApplyEffect(PlayerController.PlayerState.INDIALOGUE);
            player.GetComponent<PlayerConversant>().QueueDialogue(startDialogue, 1f);
        }
        FindObjectOfType<AudioManager>().StartLevelTheme();
    }

    public void OnRecipeComplete()
    {
        player.GetComponent<PlayerConversant>().QueueDialogue(completeDialogue, 1f);
    }

    public void OnLevelCompleteDialogueEnd()
    {
        FindObjectOfType<AudioManager>().StartFade(1.5f, 0);
        SceneManagement sceneManager = FindObjectOfType<SceneManagement>();
        sceneManager.LoadScene(nextLevelName);
    }

    public void OnLevelStart()
    {
        cauldron.GetComponent<CauldronAI>().OnStartLevel(totalBoilTimer);
    }


}
