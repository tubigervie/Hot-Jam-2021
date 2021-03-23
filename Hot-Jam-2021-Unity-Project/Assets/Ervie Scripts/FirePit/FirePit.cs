using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePit : MonoBehaviour, IInteractable
{
    [SerializeField] Transform cauldronSlot;
    public void Interact(GameObject player)
    {
        InteractionController interactionController = player.GetComponent<InteractionController>();
        if (interactionController != null)
        {
            if(interactionController.cauldronContainer != null)
            {
                interactionController.cauldronContainer.GetComponent<CauldronAI>().Drop(player.transform);
                interactionController.cauldronContainer.GetComponent<CauldronAI>().SetOnFirePit();
                interactionController.cauldronContainer.transform.parent = cauldronSlot;
                interactionController.cauldronContainer.transform.localPosition = Vector3.zero;
                interactionController.cauldronContainer.transform.localEulerAngles = Vector3.zero;
                interactionController.SetCauldronSlot(null);
            }
        }
    }

}
