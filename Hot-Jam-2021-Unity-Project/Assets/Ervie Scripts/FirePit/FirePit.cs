using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePit : MonoBehaviour, IInteractable
{
    [SerializeField] Transform cauldronSlot;
    BoxCollider fireCollider;

    void Awake()
    {
        fireCollider = GetComponent<BoxCollider>();
    }

    public void Interact(GameObject player)
    {
        InteractionController interactionController = player.GetComponent<InteractionController>();
        if (interactionController != null)
        {
            if(interactionController.cauldronContainer != null)
            {
                fireCollider.enabled = false;
                interactionController.cauldronContainer.GetComponent<CauldronAI>().Drop(player.transform);
                interactionController.cauldronContainer.GetComponent<CauldronAI>().SetOnFirePit();
                interactionController.cauldronContainer.transform.parent = cauldronSlot;
                interactionController.cauldronContainer.transform.localPosition = Vector3.zero;
                interactionController.cauldronContainer.transform.localEulerAngles = Vector3.zero;
                interactionController.SetCauldronSlot(null);
            }
        }
    }

    public void EnableCollider()
    {
        fireCollider.enabled = true;
    }

}
