using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpText : MonoBehaviour
{
    [SerializeField] Text label;

    InteractionController interactControl;

    // Start is called before the first frame update
    void Start()
    {
        interactControl = GetComponent<InteractionController>();
    }

    public void SetText(string desc)
    {
        label.text = desc;
    }
}
