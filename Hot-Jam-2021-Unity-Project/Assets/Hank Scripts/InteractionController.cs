﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    // Using Collider over raycast due to the small size of the pickup colliders
    [SerializeField] Collider interactionCollider;

    IPickable pickup = null;
    IInteractable interactable = null;

    public Ingredient currentIngredient { get { return _currentIngredient; } }

    [SerializeField] Ingredient _currentIngredient = null;

    [SerializeField] HelpText helpText;
    [SerializeField] DebugPanelController debugPanel;

    // Start is called before the first frame update
    void Start()
    {
        helpText = GetComponent<HelpText>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (pickup != null)     // If current focus is IPickable
            {
                if (_currentIngredient != null) {
                    DropIngredient();
                }
                SetCurrentItem(pickup.PickUp());
                pickup = null;
            }
            else if (interactable != null)  // If current focus is IInteractable
            {
                interactable.Interact(this.gameObject);
            }
            else if (_currentIngredient != null)
            {
                DropIngredient();
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            debugPanel.TogglePanel();
        }

        // Debug Panel Updates
        if (_currentIngredient != null)
        {
            debugPanel.UpdateHeldObj(_currentIngredient.displayName);
        }
        else
        {
            debugPanel.UpdateHeldObj("N/A");
        }
        if (pickup != null)
        {
            helpText.SetText("Press SPACE to pickup");
            debugPanel.UpdateDetectedObj("pickup");
        }
        else if (interactable != null)
        {
            helpText.SetText("Press SPACE to interact");
            debugPanel.UpdateDetectedObj("interactable");
        }
        else
        {
            helpText.SetText("");
            debugPanel.UpdateDetectedObj("N/A");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickable item = other.gameObject.GetComponent<IPickable>();
        IInteractable tool = other.gameObject.GetComponent<IInteractable>();
        if (item != null)   // If collided obj has IPickable
        {
            pickup = item;
            interactable = null;
        }
        else if (tool != null) // If collided obj has IInteractable
        {
            interactable = tool;
            pickup = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IPickable item = other.gameObject.GetComponent<IPickable>();
        IInteractable tool = other.gameObject.GetComponent<IInteractable>();
        if (item != null)   // If collided obj has IPickable
        {
            pickup = null;
        }
        else if (tool != null)  // If collided obj has IInteractable
        {
            interactable = null;
        }
    }

    public void SetCurrentItem(Ingredient item)
    {
        _currentIngredient = item;
    }
    private void DropIngredient()
    {
        _currentIngredient.SpawnPickup(transform.position + transform.forward);
        _currentIngredient = null;
    }
}
