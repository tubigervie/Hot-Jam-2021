using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowPit : MonoBehaviour
{
    [SerializeField] float slowDuration = 1f;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        PlayerController playerControl = other.gameObject.GetComponent<PlayerController>();
    //        playerControl.ApplyEffect(PlayerController.PlayerState.SLOWED, )
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController playerControl = other.gameObject.GetComponent<PlayerController>();
            if(!playerControl.IsStunned())
                playerControl.ApplyEffect(PlayerController.PlayerState.SLOWED, slowDuration);
        }
    }
}
