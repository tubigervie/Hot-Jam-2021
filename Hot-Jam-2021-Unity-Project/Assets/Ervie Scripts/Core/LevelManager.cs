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
    }

    public void OnRecipeComplete()
    {
        player.GetComponent<PlayerConversant>().QueueDialogue(completeDialogue, 1f);
    }

    public void OnLevelCompleteDialogueEnd()
    {
        SceneManagement sceneManager = FindObjectOfType<SceneManagement>();
        sceneManager.LoadScene(nextLevelName);
    }


}
