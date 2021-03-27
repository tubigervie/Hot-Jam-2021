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

    [SerializeField] Image deathLabel;


    GameObject _currentInteractSpawnObject = null;
    GameObject _currentPickableSpawnObject = null;
    GameObject _currentDeathSpawnObject = null;

    // Start is called before the first frame update
    void Start()
    {
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
        if (deathLabel.enabled && _currentDeathSpawnObject != null)
        {
            deathLabel.transform.position = Camera.main.WorldToScreenPoint(_currentDeathSpawnObject.transform.position + Vector3.up * 4);
        }
        else
        {
            deathLabel.enabled = false;
        }
    }

    public void SpawnDeathIcon(bool flag, GameObject spawnObject)
    {
        _currentDeathSpawnObject = spawnObject;
        if (spawnObject != null)
        {
            deathLabel.transform.position = Camera.main.WorldToScreenPoint(spawnObject.transform.position + Vector3.up * interactIconFloatHeight);
        }
        deathLabel.enabled = flag;
        deathLabel.GetComponent<Animation>().Play();
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
