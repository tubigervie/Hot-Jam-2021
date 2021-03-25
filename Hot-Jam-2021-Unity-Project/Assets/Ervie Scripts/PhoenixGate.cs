using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoenixGate : MonoBehaviour
{
    BossAI bossAI;

    void Start()
    {
        bossAI = FindObjectOfType<BossAI>();
        if(bossAI != null)
        {
            bossAI.onDie += HideWalls;
        }
    }

    void HideWalls()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
