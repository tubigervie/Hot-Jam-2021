using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPickUpController : MonoBehaviour
{
    [SerializeField] Ingredient _currentPickedUpItem = null;

    public Ingredient currentPickedUpItem { get { return _currentPickedUpItem; } }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            Debug.DrawRay(ray.origin, ray.direction * 50, Color.green);


            if (Physics.Raycast(ray.origin, ray.direction * 50, out hit))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                IPickable pickup = hit.collider.GetComponent<IPickable>();
                if(interactable != null)
                {
                    interactable.Interact(this.gameObject);
                    return;
                }
                if(pickup != null)
                {
                    SetCurrentPickedItem(pickup.PickUp());
                    return;
                }
                else
                {
                    _currentPickedUpItem = null;
                }
            }
        }
    }

    public void SetCurrentPickedItem(Ingredient newIngredient)
    {
        _currentPickedUpItem = newIngredient;
    }
}
