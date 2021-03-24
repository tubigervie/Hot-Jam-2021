using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpText : MonoBehaviour
{
    [SerializeField] Image label;

    InteractionController interactControl;

    // Start is called before the first frame update
    void Start()
    {
        interactControl = FindObjectOfType<InteractionController>();
    }

    private void Update()
    {
        if (label.enabled)
        {
            label.transform.position = Camera.main.WorldToScreenPoint(interactControl.transform.position + Vector3.up * 3.5f);
        }
    }

    public void ToggleInteractIcon(bool flag)
    {
        label.transform.position = Camera.main.WorldToScreenPoint(interactControl.transform.position + Vector3.up * 3.5f);
        label.enabled = flag;
    }
}
