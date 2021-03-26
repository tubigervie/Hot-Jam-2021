using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelHook : MonoBehaviour
{
    public void LevelCompleteDialogueHook()
    {
        LevelManager level = FindObjectOfType<LevelManager>();
        if (level == null)
        {
            Debug.Log("Level was not found");
            return;
        }
        level.OnLevelCompleteDialogueEnd();
    }

    public void LevelStartTimersHook()
    {
        LevelManager level = FindObjectOfType<LevelManager>();
        if (level == null)
        {
            Debug.Log("Level was not found");
            return;
        }
        level.onLevelStart.Invoke();
    }

    public void TutorialEndHook()
    {
        GameManager game = FindObjectOfType<GameManager>();
        Debug.Log("getting here");
        game.LoadFirstLevel();
    }
}
