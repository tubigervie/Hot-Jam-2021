using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfTutorialComplete : MonoBehaviour
{
    void Start()
    {
        if (FindObjectOfType<GameManager>().TutorialComplete())
            Destroy(this.gameObject);
    }
}
