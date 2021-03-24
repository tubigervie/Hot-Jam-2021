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

}
