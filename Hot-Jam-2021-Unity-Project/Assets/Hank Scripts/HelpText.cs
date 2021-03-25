using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpText : MonoBehaviour
{
    [SerializeField] Image label;

    InteractionController interactControl;

    GameObject _currentSpawnObject = null;

    // Start is called before the first frame update
    void Start()
    {
        interactControl = FindObjectOfType<InteractionController>();
    }

    private void FixedUpdate()
    {
        if (label.enabled && _currentSpawnObject != null)
        {
            label.transform.position = Camera.main.WorldToScreenPoint(_currentSpawnObject.transform.position + Vector3.up * 1.5f);
        }
    }

    public void ToggleInteractIcon(bool flag, GameObject spawnObject)
    {
        _currentSpawnObject = spawnObject;
        if(spawnObject != null)
        {
            label.transform.position = Camera.main.WorldToScreenPoint(spawnObject.transform.position + Vector3.up * 1.5f);
        }
        label.enabled = flag;
    }
}
