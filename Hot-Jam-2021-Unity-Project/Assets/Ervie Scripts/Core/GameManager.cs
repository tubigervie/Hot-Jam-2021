using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] LevelManager currentLevel;

    public void SetCurrentLevel(LevelManager level)
    {
        if(currentLevel != level)
        {
            currentLevel = level;
        }
    }
}
