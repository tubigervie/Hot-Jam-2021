using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpText : MonoBehaviour
{
    [SerializeField] Image interactLabel;
    [SerializeField] float interactIconFloatHeight = 2.5f;
    [SerializeField] Image pickableLabel;
    [SerializeField] float pickableIconFloatHeight = 1.5f;

    InteractionController interactControl;

    GameObject _currentInteractSpawnObject = null;
    GameObject _currentPickableSpawnObject = null;

    // Start is called before the first frame update
    void Start()
    {
        interactControl = FindObjectOfType<InteractionController>();
    }

    private void FixedUpdate()
    {
        if (interactLabel.enabled && _currentInteractSpawnObject != null)
        {
            interactLabel.transform.position = Camera.main.WorldToScreenPoint(_currentInteractSpawnObject.transform.position + Vector3.up * interactIconFloatHeight);
        }
        else
        {
            interactLabel.enabled = false;
        }
        if (pickableLabel.enabled && _currentPickableSpawnObject != null)
        {
            pickableLabel.transform.position = Camera.main.WorldToScreenPoint(_currentPickableSpawnObject.transform.position + Vector3.up * pickableIconFloatHeight);
        }
        else
        {
            pickableLabel.enabled = false;
        }
    }

    public void ToggleInteractIcon(bool flag, GameObject spawnObject)
    {
        _currentInteractSpawnObject = spawnObject;
        if(spawnObject != null)
        {
            interactLabel.transform.position = Camera.main.WorldToScreenPoint(spawnObject.transform.position + Vector3.up * interactIconFloatHeight);
        }
        interactLabel.enabled = flag;
    }

    public void TogglePickableIcon(bool flag, GameObject spawnObject)
    {
        _currentPickableSpawnObject = spawnObject;
        if(spawnObject != null)
        {
            if(spawnObject.tag != "Player")
            {
                pickableLabel.sprite = spawnObject.GetComponent<IngredientPickup>().ingredient.sprite;
                pickableLabel.transform.position = Camera.main.WorldToScreenPoint(spawnObject.transform.position + Vector3.up * pickableIconFloatHeight);
            }
            else
            {
                pickableLabel.sprite = spawnObject.GetComponent<InteractionController>().currentIngredient.sprite;
                pickableLabel.transform.position = Camera.main.WorldToScreenPoint(spawnObject.transform.position + Vector3.up * interactIconFloatHeight * 1.5f);
            }
        }
        pickableLabel.enabled = flag;
    }
}
