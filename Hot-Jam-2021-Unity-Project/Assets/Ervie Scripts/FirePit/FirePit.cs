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
        fireCollider.enabled = false;
    }

    public void Interact(GameObject player)
    {
        InteractionController interactionController = player.GetComponent<InteractionController>();
        if (interactionController != null)
        {
            if(interactionController.cauldronContainer != null)
            {
                fireCollider.enabled = false;
                Transform cauldronTransform = interactionController.cauldronContainer.transform;
                interactionController.cauldronContainer.GetComponent<CauldronAI>().Drop(player.transform);
                interactionController.cauldronContainer.GetComponent<CauldronAI>().SetOnFirePit();
                cauldronTransform.parent = cauldronSlot;
                cauldronTransform.localPosition = Vector3.zero;
                cauldronTransform.localEulerAngles = Vector3.zero;
                interactionController.SetCauldronSlot(null);
            }
        }
    }

    public void EnableCollider()
    {
        fireCollider.enabled = true;
    }

}
