using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] CauldronContainer cauldron;
    [SerializeField] GameObject player;

    private void Awake()
    {
        if(instance != null)
            Destroy(this.gameObject);
        else
            instance = this;

        cauldron = FindObjectOfType<CauldronContainer>();
        cauldron.onRecipeComplete += OnLevelComplete;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        FindObjectOfType<GameManager>().SetCurrentLevel(this);
    }

    public void OnLevelComplete()
    {
        Debug.Log("level complete! transition to main menu");
    }


}
