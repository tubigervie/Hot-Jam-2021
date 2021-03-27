using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueColliderTrigger : MonoBehaviour
{
    [SerializeField] Dialogue dialogue;
    bool hadTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && !hadTriggered)
        {
            hadTriggered = true;
            other.GetComponent<PlayerConversant>().QueueDialogue(dialogue, .5f);
        }
    }
}
